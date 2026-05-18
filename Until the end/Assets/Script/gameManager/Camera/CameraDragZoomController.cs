using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 2D摄像机拖拽缩放控制器
/// 功能：
/// 1. 中键拖拽移动摄像机
/// 2. 滚轮控制缩放（正交摄像机大小）
/// 3. 移动范围限制在指定Collider2D的边界内
/// 4. 放大到最大时摄像机自动定位到边界中心，并且视角范围覆盖整个边界
/// 5. 当鼠标在UI面板上时忽略摄像机操作
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraDragZoomController : MonoBehaviour
{
    [Header("边界设置")]
    [Tooltip("带有Collider2D的游戏对象，用于定义摄像机移动边界。如不指定，将使用当前游戏对象的Collider2D。")]
    public GameObject boundsObject;
    
    [Header("缩放设置")]
    [Tooltip("最小正交大小（放大极限）")]
    public float minOrthographicSize = 1f;
    [Tooltip("最大正交大小（缩小极限）")]
    public float maxOrthographicSize = 10f;
    [Tooltip("缩放速度")]
    public float zoomSpeed = 1f;
    [Tooltip("是否在放大到最大时自动居中到边界中心，并且调整摄像机大小以覆盖整个边界")]
    public bool centerOnMaxZoom = true;
    
    [Header("移动平滑度")]
    [Tooltip("移动平滑时间（秒），0为无平滑")]
    public float smoothTime = 0.1f;
    
    private Camera cam;
    private Collider2D boundaryCollider;
    private Bounds bounds;
    private Vector3 targetPosition;
    private float targetOrthographicSize;
    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;
    
    // 拖拽相关变量
    private Vector3 dragOrigin;
    private bool isDragging = false;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraDragZoomController需要挂载在带有Camera组件的游戏对象上");
            enabled = false;
            return;
        }
        
        // 初始化目标位置和大小
        targetPosition = transform.position;
        targetOrthographicSize = cam.orthographicSize;
        
        // 获取边界Collider
        FindBoundaryCollider();
        UpdateBounds();
        
        // 如果启用放大到最大时居中功能，确保最大缩放值能覆盖整个边界
        if (centerOnMaxZoom)
        {
            float sizeToCoverBounds = GetOrthographicSizeToCoverBounds();
            if (sizeToCoverBounds > maxOrthographicSize)
            {
                maxOrthographicSize = sizeToCoverBounds;
            }
            // 确保当前目标大小不超过最大缩放值
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);
        }
    }
    
    void Update()
    {
        HandleMouseInput();
        ApplySmoothMovement();
        ApplySmoothZoom();
        ClampCameraPosition();
    }
    
    /// <summary>
    /// 检查鼠标是否在UI元素上
    /// </summary>
    private bool IsMouseOverUI()
    {
        // 检查EventSystem是否存在且鼠标是否在UI上
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 计算覆盖整个边界所需的正交摄像机大小
    /// </summary>
    private float GetOrthographicSizeToCoverBounds()
    {
        if (boundaryCollider == null || cam == null)
            return minOrthographicSize;
        
        // 更新边界确保获取最新尺寸
        UpdateBounds();
        
        // 对于正交摄像机，可见区域大小：
        // 高度 = 2 * orthographicSize
        // 宽度 = 2 * orthographicSize * aspect
        float boundsHeight = bounds.size.y;
        float boundsWidth = bounds.size.x;
        
        // 确保aspect不为零
        float aspect = Mathf.Max(cam.aspect, 0.001f);
        
        // 计算所需的大小：需要同时满足高度和宽度要求
        float requiredSizeByHeight = boundsHeight / 2f;
        float requiredSizeByWidth = boundsWidth / (2f * aspect);
        
        // 取最大值以确保整个边界都可见
        float requiredSize = Mathf.Max(requiredSizeByHeight, requiredSizeByWidth);
        
        // 确保不小于最小缩放值
        return Mathf.Max(requiredSize, minOrthographicSize);
    }
    
    /// <summary>
    /// 处理鼠标输入：拖拽和缩放
    /// </summary>
    void HandleMouseInput()
    {
        // 鼠标滚轮缩放（当鼠标不在UI上时）
        if (!IsMouseOverUI())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetOrthographicSize -= scroll * zoomSpeed;
                targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);
                
                // 如果放大到最大且启用居中功能
                if (centerOnMaxZoom && Mathf.Approximately(targetOrthographicSize, maxOrthographicSize))
                {
                    // 计算覆盖整个边界所需的大小
                    float sizeToCoverBounds = GetOrthographicSizeToCoverBounds();
                    
                    // 确保最大缩放值至少能覆盖整个边界
                    if (sizeToCoverBounds > maxOrthographicSize)
                    {
                        maxOrthographicSize = sizeToCoverBounds;
                    }
                    
                    // 使用能够覆盖整个边界的大小（确保不超过最大缩放限制）
                    targetOrthographicSize = Mathf.Clamp(sizeToCoverBounds, minOrthographicSize, maxOrthographicSize);
                    
                    // 将目标位置设置为边界中心（只更新x和y坐标，保持z轴不变）
                    targetPosition = new Vector3(bounds.center.x, bounds.center.y, targetPosition.z);
                }
            }
        }
        
        // 鼠标中键拖拽
        if (Input.GetMouseButtonDown(2) && !IsMouseOverUI())
        {
            dragOrigin = GetMouseWorldPosition();
            isDragging = true;
        }
        
        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
        
        if (isDragging && Input.GetMouseButton(2) && !IsMouseOverUI())
        {
            Vector3 currentMousePos = GetMouseWorldPosition();
            Vector3 difference = dragOrigin - currentMousePos;
            // 只更新x和y坐标，保持z轴不变
            targetPosition.x += difference.x;
            targetPosition.y += difference.y;
            dragOrigin = GetMouseWorldPosition();
        }
    }
    
    /// <summary>
    /// 应用平滑移动
    /// </summary>
    void ApplySmoothMovement()
    {
        if (smoothTime > 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = targetPosition;
        }
    }
    
    /// <summary>
    /// 应用平滑缩放
    /// </summary>
    void ApplySmoothZoom()
    {
        if (smoothTime > 0)
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetOrthographicSize, ref zoomVelocity, smoothTime);
        }
        else
        {
            cam.orthographicSize = targetOrthographicSize;
        }
    }
    
    /// <summary>
    /// 限制摄像机位置在边界内
    /// </summary>
    void ClampCameraPosition()
    {
        if (boundaryCollider == null)
            return;
        
        // 更新边界（处理动态移动的边界对象）
        UpdateBounds();
        
        // 计算摄像机可见区域的一半尺寸
        float verticalExtent = cam.orthographicSize;
        float horizontalExtent = verticalExtent * cam.aspect;
        
        // 计算边界限制
        float minX = bounds.min.x + horizontalExtent;
        float maxX = bounds.max.x - horizontalExtent;
        float minY = bounds.min.y + verticalExtent;
        float maxY = bounds.max.y - verticalExtent;
        
        // 如果边界小于摄像机可见区域，则居中
        if (minX > maxX)
        {
            minX = maxX = bounds.center.x;
        }
        
        if (minY > maxY)
        {
            minY = maxY = bounds.center.y;
        }
        
        // 限制目标位置
        Vector3 clampedTarget = targetPosition;
        clampedTarget.x = Mathf.Clamp(clampedTarget.x, minX, maxX);
        clampedTarget.y = Mathf.Clamp(clampedTarget.y, minY, maxY);
        targetPosition = clampedTarget;
        
        // 也限制当前摄像机位置（防止平滑运动超出边界）
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }
    
    /// <summary>
    /// 获取鼠标在世界坐标系中的位置
    /// </summary>
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePos);
    }
    
    /// <summary>
    /// 查找边界Collider
    /// </summary>
    void FindBoundaryCollider()
    {
        if (boundsObject != null)
        {
            boundaryCollider = boundsObject.GetComponent<Collider2D>();
            if (boundaryCollider == null)
            {
                Debug.LogWarning("指定的边界对象没有Collider2D组件，将尝试在当前对象上查找");
            }
        }
        
        if (boundaryCollider == null)
        {
            boundaryCollider = GetComponent<Collider2D>();
            if (boundaryCollider == null)
            {
                Debug.LogWarning("未找到Collider2D组件，摄像机将没有移动限制");
            }
        }
    }
    
    /// <summary>
    /// 更新边界信息
    /// </summary>
    void UpdateBounds()
    {
        if (boundaryCollider != null)
        {
            bounds = boundaryCollider.bounds;
        }
        else
        {
            // 如果没有边界，使用一个非常大的边界
            bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 0));
        }
    }
    
    /// <summary>
    /// 在编辑器中绘制边界Gizmo
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (boundaryCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundaryCollider.bounds.center, boundaryCollider.bounds.size);
        }
    }
    
    /// <summary>
    /// 公开方法：手动设置边界对象
    /// </summary>
    public void SetBoundsObject(GameObject newBoundsObject)
    {
        boundsObject = newBoundsObject;
        FindBoundaryCollider();
        UpdateBounds();
    }
    
    /// <summary>
    /// 公开方法：立即跳转到边界中心
    /// </summary>
    public void JumpToBoundsCenter()
    {
        UpdateBounds();
        // 只更新x和y坐标，保持z轴不变
        targetPosition = new Vector3(bounds.center.x, bounds.center.y, targetPosition.z);
        transform.position = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
    }
    
    /// <summary>
    /// 公开方法：重置缩放
    /// </summary>
    public void ResetZoom()
    {
        targetOrthographicSize = Mathf.Lerp(minOrthographicSize, maxOrthographicSize, 0.5f);
    }
}