using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;
public enum ControlType
{
    idle,//闲置
    unit,//单位
    attack,//攻击
    weapon,//武器
    building,//建筑
    supply//支援
}//当前控制
public class GameController : MonoBehaviour
    //游戏管理
{
    public static  GameController Instance;
    //单例模式
    public ControlType currentControlType = ControlType.idle;

    UnitAttribute hitUnit;//触碰的单位
    public UnitAttribute selectedUnit;

    public int cost=0;
    public int Inning;

    public int CommandUnitValue { get; private set; }
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

        
        ManagerEvent.BattleEnd += GameEnd;
        ManagerEvent.DefenseValueLoss += DefenseValueLoss;
    }
    private void OnDestroy()
    {
        ManagerEvent.BattleEnd -= GameEnd;
        ManagerEvent.DefenseValueLoss -= DefenseValueLoss;
    }
    void Start()
    {
        cost = 0;
        UIEvent.UpdateSupplyInfo?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        mouseSelected();
        //执行 鼠标选择
        //DrawCircleRangeLine(hitUnit);//绘制单位攻击范围
        if(Input.GetKeyDown(KeyCode.T)) 
        {
            GameEnd();
        }
    }
    public void mouseSelected()
    //鼠标选择 
    {
        if ((currentControlType==ControlType.idle||currentControlType==ControlType.unit)&&Input.GetMouseButtonDown(0)  )            
        {
            if (EventSystem.current.IsPointerOverGameObject())   
            {
                return;
            }//判断是否是UI
  
            HandleSelection();
            //执行 处理选择 
        }//选择单位时 需要闲置状态或单位状态
        if (currentControlType==ControlType.unit&&Input.GetMouseButtonDown(1) && selectedUnit != null ) {
            //鼠标右键且选择模型不是空的  不能 在选择装载单位的状态 和 指挥状态
            MoveSelectedUnit();
            //执行 移动选择的模型 
        }
    }
    /// <summary>
    /// 处理选择+显示选择的单位UI+选择单位的描述
    /// </summary>
    void HandleSelection() {
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //二维量 鼠标位置 主摄像头 在世界位置 获取当前鼠标的位置
        Collider2D hit = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask("unit")) ;
        //Debug.Log(hit + "位置" + mousePos);

        if (hit != null)
        {
            hitUnit = hit.GetComponent<UnitAttribute>();
            //  如果有 hitBlue=hit，否则hitBlue=null           
            LineEvent.ShowUnitVisualEvent?.Invoke(new ShowUnitVisualEvent { unit=hitUnit,show=true});
            if (hitUnit != null)
            {
                //不为空

                UIEvent.OnUnitInfo?.Invoke(hitUnit);//显示单位UI  可显示敌我单位
                UIEvent.UpdateDescriptionInfo?.Invoke(hitUnit.unitData);
                if (hitUnit.faction==Faction.Blue)
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
        
    }//选择判断
    void ToggleSelection(UnitAttribute newSelection)
    //修改选择    
    {
        if (selectedUnit != null)
        {
            selectedUnit.UnitSelected(false);
        }//取消之前 单位 的 选择

        if (selectedUnit != newSelection) 
        {
            selectedUnit = newSelection;
            selectedUnit.UnitSelected(true);
            ChangeControlType(ControlType.unit);
        }//不是现在的选择
        else
        {
            
            UIEvent.HideUnitInfo.Invoke();
            LineEvent.ShowUnitVisualEvent?.Invoke(new ShowUnitVisualEvent {unit=selectedUnit,show=false });
            selectedUnit = null;
            ChangeControlType (ControlType.idle);
        }//选择相同的单位 取消选择
    }//切换选择
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
        ChangeControlType(ControlType.idle);
    }//取消选择
    void MoveSelectedUnit() {
        //移动选择的模型
        Vector2 targetPos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //目标位置 鼠标的位置


        selectedUnit.SetUnitMovePos(targetPos);
        LineEvent.ShowUnitVisualEvent?.Invoke(new ShowUnitVisualEvent 
                            { unit = selectedUnit,show=true });
        //选择的模型 返回 需要移动到的位置
    }//设置单位移动
    public void setCost(int Cost)
    {
        cost += Cost;
        if (cost < 0) cost = 0;
        UIEvent.UpdateSupplyInfo?.Invoke();
    }//设置补给
    private void GameEnd()
    {
        UIEvent.OnMessageText?.Invoke("战斗结束");
        UIEvent.GameEnd?.Invoke(Inning);
        Time.timeScale = 0;
    }//游戏结束
    private void DefenseValueLoss(int value)
    {
        defenseValue -= value;
        if (defenseValue <= 0)
            GameEnd();
    }//减少防御值
    public void ChangeControlType(ControlType newType)
    {
        currentControlType = newType;
    }
    public void SetCommandUnit(int value)
    {
        CommandUnitValue += value;
    }
    
}
