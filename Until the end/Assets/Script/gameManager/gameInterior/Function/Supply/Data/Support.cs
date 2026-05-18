using System.Collections;
using UnityEngine;

public class Support : MonoBehaviour
{
    [SerializeField] protected SupportData supplyData;

    [SerializeField] protected string supplyName;//名字
    [SerializeField] protected GameObject shell;//炮弹
    [SerializeField] protected int eachTimeNumber;//单次攻击数
    [SerializeField] protected int number;//攻击次数
    [SerializeField] protected int currentNumber = 0;
    [SerializeField] protected float delay;
    [SerializeField] protected float interval;

    [SerializeField] protected bool canSetStartPos;//可以控制起始位置
    [SerializeField] protected bool canSetRotate;//可以控制方向
    [SerializeField] protected bool canSetExtend;//可以控制延伸

    [SerializeField] protected Vector2 startPos;//起始点
    [SerializeField] protected float radiusRange;//半径范围
    [SerializeField] protected Vector2 center;//中心位置
    [SerializeField] protected float rotateAngle = 0;//方向
    [SerializeField] protected Vector2[] point = new Vector2[4]; //记录相对位置

    [SerializeField] protected LineRenderer lineRenderer;//绘制线条
    [SerializeField] protected Vector3[] lineVector = new Vector3[4];//记录世界位置


    private void Start()
    {
        ApplyData();
        ApplyLineState();

        point = new Vector2[4]
        {
                new Vector2(-radiusRange,0),//左下
                new Vector2(-radiusRange,0),//左上
                new Vector2(radiusRange,0),//右上
                new Vector2(radiusRange,0)//右下
        };

        StartCoroutine(SpawnSupport());

    }
    IEnumerator SpawnSupport()
    {
        while (!canSetStartPos)
        {
            yield return null;
            if (Input.GetMouseButton(0))
            {
                //pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //pos2=new Vector2(pos1.x+3, pos1.y);
                //UpdateBox(pos1, pos2, rotateAngle);
            }
            if (Input.GetMouseButtonUp(0))
            {
                break;
            }//取消选择
        }//不能绘制区域

        while (canSetStartPos)
        {
            yield return null;
            SelectedStartPos();
        }//设置区域
        while (canSetRotate)
        {
            yield return null;
            SelectedRotate();
        }//设置方向
        while (canSetExtend)
        {
            yield return null;
            SelectedExtend();
        }//设置延伸

        center = (lineVector[0] + lineVector[2]) * 0.5f;//计算中心位置
        yield return new WaitForSeconds(delay);
        Debug.Log("支援开始");
        for (int i = 0; i < number; i++)
        {
            yield return new WaitForSeconds(interval);
            for (int j = 0; j < eachTimeNumber; j++)
            {
                Instantiate(shell, SpawnPos(), Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(0f,1f ));
            }
            currentNumber++;
        }
        Debug.Log("支援结束");
        Destroy(gameObject);
    }
    void SelectedStartPos()
    {
        startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        for (int i = 0; i < 4; i++)
        {
            lineVector[i] = startPos+point[i];          
        }//记录对应的世界位置

        transform.position = startPos;//视觉辅助
        lineRenderer.SetPositions(lineVector);
        //绘制

        if (Input.GetMouseButtonDown(0))
        {
            canSetStartPos = false;
        }//确认起始点
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("取消选择");
            Destroy(gameObject);
            return;
        }//取消选择
    }//选择范围
    void SelectedRotate()
    {
        Vector2 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        rotateAngle =-90+ Mathf.Atan2(mousePos.y - startPos.y,
                                            mousePos.x - startPos.x) * Mathf.Rad2Deg;

        Quaternion angle = Quaternion.Euler(0, 0, rotateAngle);

        
        for(int i = 0; i < 4; i++)
        {
            Vector2 rotated = angle * point[i];
            //计算相对位置的旋转
            lineVector[i] =startPos+rotated;
            //记录旋转后的世界位置
        }

        transform.rotation = angle;//视觉辅助
        lineRenderer.SetPositions(lineVector);
        if (Input.GetMouseButtonDown(0))
        {
            canSetRotate = false;
        }//确认角度
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("取消选择");
            Destroy(gameObject);
            return;
        }//取消选择
    }//选择方向
    void SelectedExtend()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float extend=Vector2.Distance(startPos, mousePos);//根据起始点计算延伸的距离

        point[1].y = extend;
        point[2].y = extend;//只需要改变左上和右上的y值
        
        for(int i=0; i<4; i++) 
        {
            lineVector[i] = startPos + point[i];
        }//重新记录世界位置

        Quaternion angle = Quaternion.Euler(0, 0, rotateAngle);
        for (int i = 0; i < 4; i++)
        {
            Vector2 rotated = angle * point[i];
            lineVector[i] = startPos + rotated;
        }//重新计算旋转后的位置

        lineRenderer.SetPositions(lineVector);
        //绘制线条

        if (Input.GetMouseButtonDown(0))
        {
            canSetExtend = false;
        }//确认延伸
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("取消选择");
            Destroy(gameObject);
            return;
        }//取消选择 
    }//选择延伸
    public virtual Vector2 SpawnPos()
    {
       Vector2 size = new Vector2(
            Mathf.Abs(point[0].x - point[2].x),
            Mathf.Abs(point[0].y - point[2].y)
        );//确认大小

       Vector2 spawnPos = new Vector2(Random.Range(-size.x * 0.5f, size.x * 0.5f), 
                                        Random.Range(-size.y * 0.5f, size.y * 0.5f));
       
        Quaternion angle = Quaternion.Euler(0, 0, rotateAngle);
        Vector2 afterRotatePos = angle * spawnPos;
        //旋转位置后的生成位置

        return center + afterRotatePos;
    }

    void ApplyData()
    {
        supplyName = supplyData.prefabName;

        canSetStartPos = supplyData.canDraw;
        canSetRotate = supplyData.canRotate;
        canSetExtend= supplyData.canExtend;

        shell = supplyData.shell;//炮弹

        eachTimeNumber = supplyData.eachTimeNumber;//每次数量
        number = supplyData.number;//射击次数
        interval = supplyData.interval;//延迟
        delay = supplyData.delay;//间隔
        radiusRange = supplyData.radiusRange;

    }
    void ApplyLineState()
    {
        lineRenderer=GetComponent<LineRenderer>();
        lineRenderer.positionCount = 4;//4个角+闭合点
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;

        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.02f;
    }
}

