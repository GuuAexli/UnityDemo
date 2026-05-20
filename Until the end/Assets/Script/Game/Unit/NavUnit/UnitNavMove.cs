using System.Collections.Generic;
using UnityEngine;

public class UnitNavMove : MonoBehaviour
{
    [SerializeField] protected UnitAttribute attr;
    //[Header("移动参数")]
    public float moveSpeed => attr.moveSpeed;
    public float rotateSpeed => attr.rotateSpeed;
    public float defaultAngle = 90f;
    public float repathRate = 0.5f;//重新寻路间隔
    public float lostRepathTime;

    public bool isMove=>attr !=null &&attr.isMove;
    public bool isAttack=>attr !=null &&attr.isAttack;

    public float stoppingDistance = 0.1f;      // 距离目标点多近时停止


    public LayerMask unitLayerMask;              // 单位所在的层

    public Collider2D unitCol;
    public List<Vector2Int> occupiedGrid = new List<Vector2Int>();//占用的网格

    private Rigidbody2D rb;
    public List<Vector3> path { get; protected set; }//路径组
    public int pathIndex { get; protected set; }//目标路径序列
    public Vector3 currentPathPos;//现在路径位置
    public Vector3 targetPos { get; protected set; }
    void Awake()
    {
        attr=GetComponent<UnitAttribute>();
        rb = GetComponent<Rigidbody2D>();
        unitCol= GetComponent<Collider2D>();
        if (unitCol == null)
        {
            Debug.LogError("没有Collide");
        }

        
    }
    void Start()
    {
        UpdateOccupiedGrid();//初始占用

        if (attr.faction==Faction.Red)
        {
            SetMovePos(GameController.Instance.defenseZone.GetRandomDefensePos());
        }
    }
    protected virtual void Update()
    {
        if (!isMove && !isAttack && transform.rotation != Quaternion.Euler(0, 0, defaultAngle))
        {
            transform.rotation = RotateHelper.ObjectRotate(transform, defaultAngle, rotateSpeed);
        }//不需要移动 不在攻击 单位没有恢复默认朝向
        if (path == null || path.Count == 0 || pathIndex >= path.Count)
            return;//没有路径||路径数==0||目标路径序列大于路径数
        if(attr._canMove)
            NavMove();
    }
    public void NavMove()
    {
        if (!isMove) return;
        if (Time.time - lostRepathTime > repathRate)
        {
            lostRepathTime = Time.time;
            if (!IsPathStillValid())
            {
                SetMovePos(targetPos);
            }//路径不在可用
        }//检查间隔

        Move();
        float distance = Vector2.Distance(transform.position, currentPathPos);
        //距离目标点的距离
        if (distance <= stoppingDistance)
        {
            UpdateOccupiedGrid();
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                attr.isMove = false;
                return;
            }//到达最后一个路径点
            LineEvent.UpdateMovePathEvent?.Invoke(attr);
            Vector2Int pos = GridManager.Instance.WorldToCell(path[pathIndex]);
            currentPathPos = path[pathIndex];
        }//到达最小停止距离
    }
    public virtual void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentPathPos,
                                                    moveSpeed * Time.deltaTime);
        if (!attr.isAttack)
            transform.rotation = RotateHelper.RotateToPos(transform, currentPathPos,
                                                    rotateSpeed);
    }//移动方式
    public void SetMovePos(Vector3 target)
    {
        path = AStarPathfinder.FindPath(transform.position, target,this);
        if (path != null && path.Count > 0)
        {
            targetPos = target;
            pathIndex = 0;
            currentPathPos = path[pathIndex];
            attr.isMove=true;
        }//设置路径序列
        else
        {
            Debug.Log("无法到达目标位置");
        }
    }// 外部调用：设置目的地
    void UpdateOccupiedGrid()
    {
        List<Vector2Int> newOccupied = GridManager.Instance.GetOccupiedGrid(unitCol);
        if (!AreGridListsEqual(occupiedGrid, newOccupied))
        {
            GridManager.Instance.UpdateOccupied(occupiedGrid,newOccupied);
            occupiedGrid = newOccupied;
            //Debug.Log("单位：" + unit.name + "占用格子数；" + occupiedGrid.Count);
        }
    }//更新占用
    public bool IsOccupiedGrid(Vector2Int cellPos)
    {
        return occupiedGrid.Contains(cellPos);
    }//目标位置是否是自己在占用网格内
    bool IsPathStillValid()
    {
        if (path == null || path.Count == 0) return false;
        
        Vector2Int targetCell = GridManager.Instance.WorldToCell(targetPos);
        if (!GridManager.Instance.IsWalkable(targetCell,this)) 
            return false;
        // 检查最终目标点

        int checkCount = Mathf.Min(3, path.Count - pathIndex);
        for (int i = 0; i < checkCount; i++)
        {
            Vector2Int cell = GridManager.Instance.WorldToCell(path[pathIndex + i]);
            if (!GridManager.Instance.IsWalkable(cell,this)) return false;
        }// 检查后续几个路径点（最多检查前方3个）
        return true;
    }//检查前方路径是否被占用
    bool AreGridListsEqual(List<Vector2Int> oldList,List<Vector2Int> newList)
    {
        if(oldList.Count!=newList.Count) return false;
        //格子占用数不一样
        for(int i = 0; i < oldList.Count; i++)
        {
            if (!newList.Contains(oldList[i])) return false;
        }//占用格子不一样
        return true;
        //完全一样
    }//判断两个占用组是否一致
    private void OnDestroy()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.UpdateOccupied(occupiedGrid, new List<Vector2Int>());
    }//销毁时取消占用
    void OnDrawGizmosSelected()
    {
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.color = (i == pathIndex) ? Color.green : Color.yellow;
                Gizmos.DrawSphere(path[i], 0.2f);
                if (i > 0)
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}