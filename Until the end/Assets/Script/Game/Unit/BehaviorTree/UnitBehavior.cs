using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using TuanjieMuse.Chat.Schema;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class UnitBehavior : MonoBehaviour
{
    [Header("控制组件")]
    public UnitAttribute attr;
    public UnitCombat unitCombat;
    public UnitNavMove unitNavMove;
    public ItemList itemList;
    private IUnitState unitPosture;//单位姿态

    private Blackboard blackboard;//黑板
    public BTNode behaviorNode;//行为节点

    public Vector3? movePos;
    public void Awake()
    {
        attr= GetComponent<UnitAttribute>();
        unitCombat= GetComponent<UnitCombat>();
        unitNavMove= GetComponent<UnitNavMove>();
        itemList= GetComponent<ItemList>();

        UIEvent.OnActiveButton += AttackBehavior;
    }
    public void OnDisable()
    {
        UIEvent.OnActiveButton -= AttackBehavior;
    }
    public void Start()
    {
        blackboard = new Blackboard();
        blackboard.Set("attribute", attr);
        blackboard.Set("navMove", unitNavMove);
        blackboard.Set("combat", unitCombat);
        if (itemList != null) blackboard.Set("itemList", itemList);

        ChangePosture(new StandingPosture(attr));
        BuildBehaviorTree();
    }
    public void Update()
    {
        HandleInput();
        UpdatePostureByFear();//更新姿态
        unitPosture?.OnUpdate();//姿态状态机
        if(behaviorNode != null)
        {
            behaviorNode.Tick();
        }//行为树
    }
    public void UpdatePostureByFear()
    {
        var posture = new List<(Func<float, bool> condition, Type currentType,
                                    Func<UnitAttribute, IUnitState> nextFactory)>
        {
            // 判断值      判断的姿态   切换姿态
            (fear=>fear>=90,null,attr=>new SuppressdPosture(attr)),
            //从任何姿态 转换为 被压制（如果fear大于90）
            (fear=>fear>=40,typeof(StandingPosture),attr=>new PronePosture(attr)),
            //从站立 转换为 匍匐 (如果fear大于40)
            (fear=>fear<60,typeof(SuppressdPosture),attr=>new PronePosture(attr)),
            //从压制 转换为 匍匐 (如果fear小于60)
            (fear=>fear<=30,null,attr=>new StandingPosture(attr))
            //从任何姿态 转换为 站立
        };

        foreach(var(condition,requiredType,nextFactory)in posture)
        {
            if (condition(attr.fear) && (requiredType == null || unitPosture.GetType() == requiredType))
            {
                var newPosture = nextFactory(attr);
                if(newPosture.GetType()!=unitPosture.GetType())//姿态不相同
                    ChangePosture(newPosture);
                break; //姿态相同跳出
            }
        }
    }//更新单位姿 根据恐惧值
    public void ChangePosture(IUnitState newState)
    {

        unitPosture?.OnExit();
        unitPosture = newState;
        unitPosture.OnEnter();
    }//改变姿态
    public void BuildBehaviorTree()
    {
        var root = new SelectorNode(blackboard, new List<BTNode>
        {
            //优先级1 使用主动道具
            new SequenceNode(blackboard,new List<BTNode>
            {
                new ConditionNode(blackboard,()=>blackboard.HasKey(BlackboardKeys.ManualUseItem)),
                    new SelectorNode(blackboard,new List<BTNode>
                    {
                        new SequenceNode(blackboard,new List<BTNode>
                        {
                            new ConditionNode(blackboard,()=>
                                        blackboard.HasKey(BlackboardKeys.ManualMoveToItemRange)),
                            new ManualMoveToItemRangeBehavior(blackboard),
                            new ManualUseItemBehavior(blackboard)
                        }),
                        new ManualUseItemBehavior(blackboard)
                    })
            }),//主动道具行为

            //优先级2 攻击指定单位
            new SequenceNode(blackboard,new List<BTNode>
            {
                new ConditionNode(blackboard,
                        ()=>blackboard.HasKey(BlackboardKeys.AttackPriorityTarget)),
                new AttackPriorityTaregtBehavior(blackboard)        
            }),//攻击指定目标

            //优先级3 巡逻移动
            new SequenceNode(blackboard,new List<BTNode>
            { 
                new ConditionNode(blackboard,()=>blackboard.HasKey(BlackboardKeys.PatrolPos)),
                    new PatrolAttackBehavior(blackboard)
            }),//巡逻移动

            //优先级4 使用自动道具
            new SequenceNode(blackboard,new List<BTNode>
            {
                new ConditionNode(blackboard,()=>blackboard.HasKey(BlackboardKeys.AssistantItem)),
                    new SelectorNode(blackboard,new List<BTNode>
                    {
                        new SequenceNode(blackboard,new List<BTNode>
                        {
                           new ConditionNode(blackboard,
                                ()=>blackboard.HasKey(BlackboardKeys.ManualUseItem)),
                           new MoveToItemRangeBehavior(blackboard),
                           new UseItemBehavior(blackboard)
                        }),//不在使用范围 顺序节点 先移动后使用
                        new UseItemBehavior(blackboard)
                    })//在使用范围 直接使用
            }),//自动使用道具 顺序节点=>选择节点(判断在不在范围)

            //优先级5 闲置
            new IdleBehavior(blackboard)
            //闲置
        });//选择行为
        behaviorNode = root;
    }//构建行为树
    private void HandleInput()
    {
        if (attr.isSelected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ClearAllBehavior();
            }//右键移动 打断所有行为
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine(WaitForPatrolPos());
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                attr.GetComponent<IFear>()?.AddFear(10);
            }
        }//选择中    
    }//判断输入
    private IEnumerator WaitForPatrolPos()
    {
        Debug.Log("选择巡逻位置");
        while(!Input.GetMouseButtonDown(0)) 
        yield return null;

        Vector3 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        unitNavMove.SetMovePos(pos);


        if (unitNavMove.path == null) { Debug.Log("巡逻位置无效"); yield break; }

        movePos = unitNavMove.targetPos;
        Debug.Log("开始巡逻,巡逻位置：" + movePos);
        blackboard.Set(BlackboardKeys.PatrolPos, movePos.Value);
    }//设置巡逻位置

    public IEnumerator WaitForUseItem(float range,Faction faction,Item item)
    {
        if (item.isCooling) { Debug.Log("道具还在冷却");yield break; }
        Debug.Log("选择目标");
        while (!Input.GetMouseButtonDown(0))
            yield return null;//等待输入

        Vector3 pos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(pos, LayerMask.GetMask("unit"));
        if(col==null) { Debug.Log("没有目标"); yield break; }

        UnitAttribute target = col.GetComponent<UnitAttribute>();
        if (target == null) { Debug.Log("没有目标类型错误");yield break; }

        if(target.faction != faction) { Debug.Log("选择单位不能成为目标"); yield break; }

        //否则是目标
        item.target = target;
        float distance=Vector2.Distance(transform.position, pos);
        if (distance < range)
        {
            blackboard.Set(BlackboardKeys.ManualUseItem, item);
            Debug.Log("正在使用" + item.name);
        }//可以使用道具
        else
        {

            unitNavMove.SetMovePos(pos);
            if (unitNavMove.path == null) { Debug.Log("没有可移动路线"); yield break; }

            movePos = unitNavMove.targetPos;
            Debug.Log("移动到使用范围中，位置：" + movePos);
            blackboard.Set(BlackboardKeys.ManualMoveToItemRange,item );
            blackboard.Set(BlackboardKeys.ManualUseItem, item);
        }//需要移动
    }//使用道具

    public IEnumerator AssistantItemBehavior(AssistantItem item)
    {
        while (item != null && item.activeItem)
        {
            while(blackboard.HasKey(BlackboardKeys.ManualUseItem)) 
            {
                Debug.Log("正在使用主动道具，暂停辅助道具");
                yield return new WaitForSeconds(5f);
            }
            if (!blackboard.HasKey(BlackboardKeys.AssistantItem))
            {
                UnitAttribute unit = null;
                while(unit == null) 
                {
                    unit = GetRangeObj.GetMinHealthUnit
                            (item.searchRange, item._targetFaction, transform.position);
                    if(unit == null)
                        Debug.Log(item._itemName + "暂时没有可选择目标"); 
                    yield return new WaitForSeconds(3f);
                }
                //暂时没有目标等待1秒
                Debug.Log(item._itemName + "找到可以选择目标" + unit.unitData.prefabName);
                item.target= unit;
                
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (distance <= item._useRange)
                {
                    blackboard.Set(BlackboardKeys.AssistantItem, item);
                    Debug.Log("正在执行"+item._itemName);
                }//范围内直接使用
                else
                {
                    attr.SetMove(true);
                    unitNavMove.SetMovePos(unit.transform.position);
                    blackboard.Set(BlackboardKeys.MoveToItemRange, item);
                    blackboard.Set(BlackboardKeys.AssistantItem, item);
                    Debug.Log("正在移动并执行" + item._itemName+"位置"+unitNavMove.targetPos);
                }//范围外 需要移动
            }//没有执行辅助行为
            yield return new WaitForSeconds(3f);
        }
    }//自动道具
    public IEnumerator WaitForAttactTarget()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButton(1)) 
            {
                GameController.Instance.ChangeControlType(ControlType.unit);
                UIEvent.OnMessageText?.Invoke("取消选择攻击目标");
                yield break;
            }//取消选择
            yield return null;
        }

        Vector3 mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(mousePos);
        UnitAttribute target = col.GetComponent<UnitAttribute>();
        if (col == null||target==null)
        {
            UIEvent.OnMessageText(attr.unitName + "将巡逻指定位置");
            blackboard.Set(BlackboardKeys.PatrolPos, mousePos);
            blackboard.Remove(BlackboardKeys.AttackPriorityTarget);
        }//巡逻移动
        else
        {
            
            if (target.faction != Faction.Red) { UIEvent.OnMessageText?.Invoke("目标不能成为优先目标"); }
            else
            {
                unitCombat.SetPriorityTarget(target);
                UIEvent.OnMessageText(attr.unitName + "将攻击" + target.unitName);
                blackboard.Set(BlackboardKeys.AttackPriorityTarget, target);
                blackboard.Remove(BlackboardKeys.PatrolPos);
            }
        }//攻击指定目标
        UIEvent.RemoveAllButtonActive?.Invoke();
        UIEvent.ResetButtonState?.Invoke();
        GameController.Instance.ChangeControlType(ControlType.unit);
    }
    public void AttackBehavior(UnitButtonUI.ActiveButtonType type,UnitAttribute unit)
    {
        if (type == UnitButtonUI.ActiveButtonType.Attack && unit == attr)
        {
            UIEvent.OnMessageText?.Invoke("选择" + attr.unitName + "的攻击目标");
            GameController.Instance.ChangeControlType(ControlType.attack);
            StartCoroutine(WaitForAttactTarget());
        }
    }
    public void ClearAllBehavior()
    {
        blackboard.Remove(BlackboardKeys.ForcedMove);
        blackboard.Remove(BlackboardKeys.PatrolPos);
        blackboard.Remove(BlackboardKeys.AssistantItem);
        blackboard.Remove(BlackboardKeys.MoveToItemRange);
        blackboard.Remove(BlackboardKeys.ManualMoveToItemRange);
        blackboard.Remove(BlackboardKeys.ManualUseItem);
        blackboard.Remove(BlackboardKeys.AttackPriorityTarget);
    }//清除所有行为
    public void ClearAssistantItemBehavior()
    {
        blackboard.Remove(BlackboardKeys.AssistantItem);
        blackboard.Remove(BlackboardKeys.MoveToItemRange);
    }
}
