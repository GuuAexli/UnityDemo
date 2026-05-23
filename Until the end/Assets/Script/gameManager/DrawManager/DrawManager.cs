using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public struct LineStyle
{
    public float width;//宽度
    public Color color;//颜色
    public Material material;//材质
    public bool loop;//闭环
}//线条样式
public class LineBinding 
{
    public LineRenderer line;
    public object key;
    public System.Action<LineRenderer, object> updateAction;
}//绑定线条

public class ShowUnitVisualEvent
{
    public UnitAttribute unit;
    public bool show;
}//显示单位视觉辅助
public class ShowExplosionEvent
{
    public Explosion explosion;
    public Vector3 pos;
    public bool show;
}

public class ClearAllShowEvent { }

public class DrawManager : MonoBehaviour
{
    [SerializeField] private GameObject LinePrefab;//挂载LineRenderer的预制体
    [SerializeField] private int initialPoolSize = 5;//初始对象池大小
    
    [SerializeField] private Stack<LineRenderer> pool= new Stack<LineRenderer>();//对象池

    [SerializeField] private LineStyle attackLineStyle;
    [SerializeField] private LineStyle moveLineStyle;

    public UnitAttribute unitShow;
    [SerializeField] private Dictionary<UnitAttribute, LineRenderer> 
                                activeAttackRange = new Dictionary<UnitAttribute, LineRenderer>();
    [SerializeField] private Dictionary<UnitAttribute,LineRenderer> 
                                activeMovePath= new Dictionary<UnitAttribute, LineRenderer>();
    [SerializeField] private Dictionary<Explosion,LineRenderer>
                                activeExplosion=new Dictionary<Explosion,LineRenderer>();
    private void OnEnable()
    {
        LineEvent.ShowUnitVisualEvent += HandleShowUnitVisual;
        LineEvent.UpdateMovePathEvent += UpdateMovePath;
        LineEvent.HideDestroyUnitEvent += RecycleUnitLine;
        LineEvent.ShowExplosEvent += HandleShowExplosion;
    }
    private void OnDestroy()
    {
        LineEvent.ShowUnitVisualEvent -= HandleShowUnitVisual;
        LineEvent.UpdateMovePathEvent-= UpdateMovePath;
        LineEvent.HideDestroyUnitEvent -= RecycleUnitLine;
        LineEvent.ShowExplosEvent-= HandleShowExplosion;
    }
    private void Start()
    {
        for(int i=0;i<initialPoolSize;i++)
        {
            CreateNewLine();
        }
    }
    private void Update()
    {
        if(unitShow!=null)
        {
            activeAttackRange.TryGetValue(unitShow, out LineRenderer lr);
            DrawUnitAttackRangeOnLine(unitShow, lr);
        }
    }
    #region 事件处理
    private void HandleShowUnitVisual(ShowUnitVisualEvent evt)
    {
        if (evt.show)
        {
            if (unitShow != null && unitShow != evt.unit)
            {
                HideAttackRange(unitShow);
                HideMovePathLine(unitShow);
            }//如果之前显示了其他单位 隐藏其他单位
            unitShow = evt.unit;
            ShowAttackRange(evt.unit);
            if (unitShow._unitNavMove.path != null)
                ShowMovePathLine(unitShow);
        }
        else
        {
            unitShow = null;
            HideAttackRange(evt.unit);
            HideMovePathLine(evt.unit);
        }
    }
    private void HandleShowExplosion(ShowExplosionEvent evt)
    {
        if (evt.show)
        {
            DrawExplosionRange(evt.explosion, evt.pos);
        }
        else
        {
            HideExplosionRange(evt.explosion);
        }
    }
    #endregion
    #region 显示/隐藏
    private void ShowAttackRange(UnitAttribute unit)
    {
        if (unit._unitCombat == null||unit._unitCombat.weapon==null) { Debug.Log(unit.unitName + "没有武器"); return; }

        if (activeAttackRange.TryGetValue(unit, out LineRenderer lr))
            RecycleLineKey(lr, activeAttackRange, unit);
        //回收线条 如果单位之前拥有
        DrawUnitAttackRange(unit);     
    } //显示攻击范围
    private void HideAttackRange(UnitAttribute unit)
    {
        if (activeAttackRange.TryGetValue(unit, out LineRenderer lr))
            RecycleLineKey(lr, activeAttackRange, unit);
    }//隐藏攻击范围
    private void ShowMovePathLine(UnitAttribute unit)
    {
        if(unit==null) return;

        if (activeMovePath.TryGetValue(unit, out LineRenderer existing))
            RecycleLineKey(existing,activeMovePath,unit);
        //如果之前有先回收
        List<Vector3> path=unit._unitNavMove.path;
        DrawMovePath(unit, path);
    }
    private void HideMovePathLine(UnitAttribute unit)
    {
        if (activeMovePath.TryGetValue(unit, out LineRenderer lr))
            RecycleLineKey(lr, activeMovePath, unit);
    }
    #endregion
    private void CreateNewLine()
    {
        GameObject a =Instantiate(LinePrefab,transform);
        a.SetActive(false);//关闭激活状态
        LineRenderer lr=a.GetComponent<LineRenderer>();
        pool.Push(lr);//堆入对象池
    }//创建新的线条渲染器
    private LineRenderer GetLine()
    {
        if(pool.Count == 0)
        {
            CreateNewLine() ;
        }
        LineRenderer lr=pool.Pop();
        lr.gameObject.SetActive(true);
        return lr;
    }//取出渲染器
    public void ApplyStyle(LineRenderer lr, LineStyle style)
    {
        lr.startWidth = style.width;
        lr.endWidth = style.width;

        lr.startColor = style.color;
        lr.endColor = style.color;
        if (style.material != null) lr.material = style.material; else Debug.Log("材质缺失");
        lr.loop = style.loop;
    }//应用线条样式
    public void DrawUnitAttackRange(UnitAttribute unit)
    {
        LineRenderer lr = GetLine();
        ApplyStyle(lr, attackLineStyle);
        DrawUnitAttackRangeOnLine(unit, lr);
        activeAttackRange[unit] = lr;
    }//调用绘制圆
    private void DrawUnitAttackRangeOnLine(UnitAttribute unit, LineRenderer lr)
    {
        if (lr == null) return;

        float range = unit._unitCombat.weapon.attackRange;
        int segments = 45;
        float angleStep = 360 / segments;
        lr.positionCount = segments;

        for(int i = 0; i < segments; i++)
        {
            float rad = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset=new Vector3(Mathf.Cos(rad),Mathf.Sin(rad),0)*range;
            lr.SetPosition(i, unit.transform.TransformPoint(offset));
        }
    }//绘制圆 
    private void UpdateMovePath(UnitAttribute unit)
    {
        if (unitShow == null || unit != unitShow) return;
        activeMovePath.TryGetValue(unit, out LineRenderer lr);
        DrawMovePathOnLine(unit,lr ,unit._unitNavMove.path);
    }
    private void DrawMovePath(UnitAttribute unit,List<Vector3> path)
    {
        LineRenderer lr=GetLine();
        ApplyStyle(lr, moveLineStyle);
        DrawMovePathOnLine(unit, lr,path);
        activeMovePath[unit] = lr;
    }
    private void DrawMovePathOnLine(UnitAttribute unit,LineRenderer lr,List<Vector3> path)
    {
        int startIndex = unit._unitNavMove.pathIndex;//绘制路径的起点
        int lrCount = path.Count - startIndex;
        lr.positionCount = lrCount;//剩余路径
        for(int i = 0; i < lr.positionCount; i++)
        {
            lr.SetPosition(i, path[startIndex+i]);
        }
    }
    private void DrawExplosionRange(Explosion explosion,Vector3 pos)
    {
        LineRenderer lr=GetLine();
        ApplyStyle(lr, attackLineStyle);
        DrawExplosionRangeOnLine(explosion, lr, pos);
        activeExplosion[explosion] = lr;
    }
    private void DrawExplosionRangeOnLine(Explosion explosion,LineRenderer lr,Vector3 pos)
    {
        if (lr == null) return;
        float range = explosion._range;
        int segment = 45;
        float angleStep = 360 / segment;
        lr.positionCount = segment;

        for(int i = 0;i < lr.positionCount;i++)
        {
            float rad = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * range;
            lr.SetPosition(i, explosion.transform.TransformPoint(offset));
        }
    }   
    private void HideExplosionRange(Explosion explosion)
    {
        if(activeExplosion.TryGetValue(explosion,out LineRenderer lr))
            RecycleLineKey(lr,activeExplosion, explosion);
    }
    /// <summary>
    /// 回收字典键
    /// </summary>
    /// <param name="lr"> 需要回收的线条</param>
    /// <param name="dict">字典名</param>
    /// <param name="key">对象</param>
    private void RecycleLineKey(LineRenderer lr,IDictionary dict,object key)
    {
        if (dict.Contains(key)) dict.Remove(key);
        RecycleLine(lr);
    }//回收字典key和线条
    private void RecycleLine(LineRenderer lr)
    {
        if(lr== null) return;
        lr.gameObject.SetActive(false);
        lr.positionCount = 0;
        pool.Push(lr);
    }//回收指定线条 通过字典确认
    public void RecycleUnitLine(UnitAttribute unit)
    {
        if (unit==unitShow)
        {
            unitShow = null;
        }
        activeAttackRange.TryGetValue(unit, out var lr);
        RecycleLineKey(lr, activeAttackRange, unit);
        activeMovePath.TryGetValue(unit, out lr);
        RecycleLineKey(lr,activeMovePath, unit);
    }//
    public void RecycleAllLine()
    {

        foreach(var lr in activeAttackRange.Values) RecycleLine(lr);
        foreach (var lr in activeMovePath.Values) RecycleLine(lr);
        activeAttackRange.Clear();
        activeMovePath.Clear();

    }//回收所有线条
}
