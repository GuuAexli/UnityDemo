using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;
public class GameController : MonoBehaviour
    //游戏管理
{
    public static  GameController Instance;
    //单例模式
    public LineRenderer lineRenderer;
    public int segments = 45;//平滑

    UnitAttribute hitUnit;//触碰的单位
    public UnitAttribute selectedUnit;
    public LayerMask unitLayer;

    public int Supply;
    public int Inning;
    [Header("状态")]
    public bool isSelectedLoadUnit;
    //正在选择装载单位
    public bool isCommand;
    //正在指挥
    public int canCommandValue;
    //指挥类型单位 的数量
    [Header("防御值")]
    [SerializeField]private int defenseValue;
    public DefenseZone defenseZone;
    private void Awake()
    //初始化
    {
        if (Instance == null)
            //场景中没有单例
            Instance = this;
            //这个就是单例
        else
            Destroy(gameObject);
        //否则消除这个
        //确保管理器唯一

        
        UnitEvent.resetUnitAllBehavior +=ResetUnitAllBehavior;
        UnitEvent.resetUnitCoroutine += ResetUnitCoroutine;
        ManagerEvent.BattleEnd += BattleEnd;
        ManagerEvent.DefenseValueLoss += DefenseValueLoss;
    }
    private void OnDestroy()
    {
        UnitEvent.resetUnitAllBehavior-=ResetUnitAllBehavior;
        UnitEvent.resetUnitCoroutine -= ResetUnitCoroutine;
        ManagerEvent.BattleEnd -= BattleEnd;
        ManagerEvent.DefenseValueLoss -= DefenseValueLoss;
    }
    void Start()
    {
        Supply = 0;
        UIEvent.UpdateSupplyInfo?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        mouseSelected();
        //执行 鼠标选择
        //DrawCircleRangeLine(hitUnit);//绘制单位攻击范围
    }
    public void mouseSelected()
    //鼠标选择 
    {
        if (Input.GetMouseButtonDown(0) && !isCommand && !isSelectedLoadUnit )
            //在选择单位时 不能 在选择装载单位的状态 和 指挥状态
        {
            if (EventSystem.current.IsPointerOverGameObject())
                //判断是否是UI
            {
                return;
            }
           //鼠标左键 
            HandleSelection(); 
            //执行 处理选择 
        }
        if (Input.GetMouseButtonDown(1) && selectedUnit != null && !isSelectedLoadUnit && !isCommand) {
            //鼠标右键且选择模型不是空的  不能 在选择装载单位的状态 和 指挥状态
            MoveSelectedObject();
            //执行 移动选择的模型 
        }
    }
    /// <summary>
    /// 处理选择+显示选择的单位UI+选择单位的描述
    /// </summary>
    void HandleSelection() {
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //二维量 鼠标位置 主摄像头 在世界位置 获取当前鼠标的位置
        Collider2D hit = Physics2D.OverlapPoint(mousePos, unitLayer) ;
        //Debug.Log(hit + "位置" + mousePos);

        if (hit != null)
        //射线形参获取的碰撞体不为空
        {
            hitUnit = hit.GetComponent<UnitAttribute>();
            //                     射线碰撞体获取碰撞体上的blue脚本 如果没有则为null
            //  如果有 hitBlue=hit，否则hitBlue=null
           
            LineEvent.ShowUnitVisualEvent?.Invoke(new ShowUnitVisualEvent { unit=hitUnit,show=true});
            if (hitUnit != null)
            {
                //不为空

                UIEvent.OnUnitInfo?.Invoke(hitUnit);//显示单位UI  可显示敌我单位
                UIEvent.UpdateDescriptionInfo?.Invoke(hitUnit.unitData);
                if (hitUnit.tag == "blue_tag")
                {
                    ToggleSelection(hitUnit);
                    //修改选择函数  包含选择相同单位取消选择
                }
                else Debug.Log("不能控制非友方单位");

                return;
                //结束这个函数
                //选择结束当前函数
            }
        }
        else
        {
            if (hitUnit != null)
            {
                LineEvent.ShowUnitVisualEvent?.Invoke(new ShowUnitVisualEvent { unit = hitUnit, show = false });
                hitUnit = null;
            }
            Debug.Log("没有碰撞到单位");
        }
        ClearSelection();
        //取消选择
        UIEvent.HideUnitInfo?.Invoke();
        UIEvent.HideWeaponInfo?.Invoke();
        
    }
    void ToggleSelection(UnitAttribute newSelection)
    //修改选择    
    {
        if (selectedUnit != null)
        //选择不为空
        {
            selectedUnit.UnitSelected(false);
        //取消之前 单位 的 正在选择 
        }

        if (selectedUnit != newSelection)
        //不是现在的选择
        {
            selectedUnit = newSelection;
        //选择现在的选择
            selectedUnit.UnitSelected(true);
        //设置选择的单位为 正在选择单位
        }

        else
        //选择相同的单位 取消选择
        {
            selectedUnit = null;
            UIEvent.HideUnitInfo.Invoke();
        }
    }
    public void ClearSelection()
        //取消选择函数
    {
        if (selectedUnit != null) {
            //选择不为空
            selectedUnit.UnitSelected(false);
            //选择的物体 里面脚本 的 SetSelected函数为假
            selectedUnit = null;
            //取消当前选择
        }
    }
    void MoveSelectedObject() {
        //移动选择的模型
        Vector2 targetPos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //目标位置 鼠标的位置

        UnitEvent.resetUnitAllBehavior?.Invoke(selectedUnit);//执行移动需要取消其他行动状态
        UnitEvent.resetUnitCoroutine?.Invoke(selectedUnit);//取消选择单位正在执行的协程

        selectedUnit.SetUnitMovePos(targetPos);
        LineEvent.ShowUnitVisualEvent?.Invoke(new ShowUnitVisualEvent 
                            { unit = selectedUnit,show=true });
        //选择的模型 返回 需要移动到的位置
    }
    public void setSupply(int supply)
    {
        Supply += supply;
        if (Supply < 0) Supply = 0;
        UIEvent.UpdateSupplyInfo?.Invoke();
    }
    public void ResetUnitAllBehavior(UnitAttribute unit)
    {

        unit.isAttack = false;//正在攻击
        unit.underAttack = false;//正在被攻击
        unit.isMove = false;//正在移动
        unit.isAction = false;//正在行动
        unit._canAttack = true;
        unit._isUseItem = false;//正在使用道具
        
    }//重置单位行为
    public void ResetUnitCoroutine(UnitAttribute unit)
    {
        unit.GetComponent<FunctionItemList>()?.IsUseItem(null);
    }
    private void BattleEnd()
    {
        Debug.Log("战斗结束");
        Time.timeScale = 0;
    }
    private void DefenseValueLoss(int value)
    {
        defenseValue -= value;
        if (defenseValue <= 0)
            BattleEnd();
    }
    /// <summary>
    /// 绘制单位攻击范围
    /// </summary>
    /// <param name="unit"></param>
    protected void DrawCircleRangeLine(UnitAttribute unit)
    {
        if (unit==null || lineRenderer == null)
        {
            lineRenderer.enabled = false;
            return;
        }
        else lineRenderer.enabled = true;

        UnitCombat unitCombat = unit._unitCombat;
        if (unitCombat == null|| unitCombat.weapon==null)
        {
            Debug.LogWarning("单位没有武器/战斗模块");
            hitUnit = null;
            return;
        }//没有武器/战斗模块
        lineRenderer.positionCount = segments;
        float angle = 0f;
        float angleStep = 360 / segments;
        float range = unitCombat.weapon.attackRange;
        for (int i = 0; i < segments; i++)
        {
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(rad) * range,
                            Mathf.Sin(rad) * range, 0f);

            lineRenderer.SetPosition(i, unit.transform.TransformPoint(pos));
            angle += angleStep;
        }
    }//绘制单位攻击范围
}
