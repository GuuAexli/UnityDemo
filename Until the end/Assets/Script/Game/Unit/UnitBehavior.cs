using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
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

    public Vector3? patrolPos;
    public Vector3? forcedMovePos;
    public void Awake()
    {
        attr= GetComponent<UnitAttribute>();
        unitCombat= GetComponent<UnitCombat>();
        unitNavMove= GetComponent<UnitNavMove>();
        itemList= GetComponent<ItemList>();
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
        unitPosture?.OnUpdate(Time.deltaTime);//姿态状态机
        if(behaviorNode != null)
        {
            behaviorNode.Tick();
        }//行为树
    }
    public void ChangePosture(IUnitState newState)
    {
        unitPosture?.OnExit();
        unitPosture = newState;
        unitPosture.OnEnter();

    }
    public void BuildBehaviorTree()
    {
        var root = new SelectorNode(blackboard, new List<BTNode>
        {
            //优先级1
            new ConditionNode(blackboard,
                ()=>blackboard.HasKey("useItem"),
                new UseItmeBehavior(blackboard)
            ),
            //优先级2
            new ConditionNode(blackboard,
                ()=>blackboard.HasKey("patrolPos"),
                new PatrolAttackBehavior(blackboard)
            //巡逻
            ),
            //优先级3
            new IdleBehavior(blackboard)
            //闲置
        });
        behaviorNode = root;
    }
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
        }//选择中
        

        if(Input.GetKeyDown(KeyCode.A)) 
        {
            ChangePosture(new PronePosture(attr));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangePosture(new StandingPosture(attr));
        }
    }
    private IEnumerator WaitForPatrolPos()
    {
        Debug.Log("选择巡逻位置");
        while(!Input.GetMouseButtonDown(0)) 
        yield return null;

        Vector3 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        unitNavMove.SetMovePos(pos);


        if (unitNavMove.path == null) { Debug.Log("巡逻位置无效"); yield break; }

        patrolPos = unitNavMove.targetPos;
        Debug.Log("开始巡逻,巡逻位置：" + patrolPos);
        blackboard.Set("patrolPos", patrolPos.Value);
    }//设置巡逻位置
    public IEnumerator WaitForUseItem(float range,Faction faction,Item item)
    {
        Debug.Log("选择目标");
        item.use = false;
        while (!Input.GetMouseButtonDown(0))
            yield return null;//等待输入

        Vector3 pos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(pos, LayerMask.GetMask("unit"));
        UnitAttribute target = col.GetComponent<UnitAttribute>();
        if (target == null) { Debug.Log("没有目标");yield break; }
        if(target.faction != faction) { Debug.Log("选择单位不能成为目标"); yield break; }

        //否则是目标
        item.target = target;
        float distance=Vector2.Distance(transform.position, pos);
        if (distance < range)
        {
            blackboard.Set("useItem", item);
            Debug.Log("正在使用" + item.name);
        }//可以使用道具
        else
        {
            unitNavMove.SetMovePos(pos);
            if (unitNavMove.path == null) { Debug.Log("没有可移动路线"); yield break; }

            forcedMovePos = unitNavMove.targetPos;
            item.move = true;
            Debug.Log("强制移动到使用范围中，位置：" + forcedMovePos);
            blackboard.Set("useItem", item);
            item.StartCoroutine(item.UseItem());
        }//需要移动
    }

    public void ClearAllBehavior()
    {
        blackboard.Remove("useItem");
        blackboard.Remove("patrolPos");
    }
}
