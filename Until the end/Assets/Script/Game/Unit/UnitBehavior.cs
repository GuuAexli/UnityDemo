using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehavior : MonoBehaviour
{
    [Header("控制组件")]
    public UnitAttribute attr;
    public UnitCombat unitCombat;
    public UnitNavMove unitNavMove;

    private IUnitState unitPosture;//单位姿态

    private Blackboard blackboard;//黑板
    public BTNode behaviorNode;//行为节点

    public Vector3? patrolPos;
    public void Awake()
    {
        attr= GetComponent<UnitAttribute>();
        unitCombat= GetComponent<UnitCombat>();
        unitNavMove= GetComponent<UnitNavMove>();
    }
    public void Start()
    {
        blackboard = new Blackboard();
        blackboard.Set("attribute", attr);
        blackboard.Set("navMove", unitNavMove);
        blackboard.Set("combat", unitCombat);

        ChangePosture(new StandingPosture(attr));
        BuildBehaviorTree();
    }
    public void Update()
    {
        HandleInput();
        unitPosture?.OnUpdate(Time.deltaTime);
        if(behaviorNode != null)
        {
            behaviorNode.Tick();
        }
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
                ()=>blackboard.HasKey("patrolPos"),
                new PatrolAttackBehavior(blackboard)
            //巡逻
            ),
            //优先级2
            new IdleBehavior(blackboard)
            //闲置
        });
        behaviorNode = root;
    }
    private void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.P)) 
        { 
            StartCoroutine(WaitForPatrolPos());
        }
        if (Input.GetMouseButtonDown(1))
        {
            CancelPatrol();
        }
        if(Input.GetKeyDown(KeyCode.A)) 
        {
            ChangePosture(new PronePosture(attr));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangePosture(new StandingPosture(attr));
        }
    }
    private System.Collections.IEnumerator WaitForPatrolPos()
    {
        Debug.Log("选择巡逻位置");
        while(!Input.GetMouseButtonDown(0)) 
        yield return null;

        Vector3 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        unitNavMove.SetMovePos(pos);


        if (unitNavMove.path == null) { Debug.Log("巡逻位置无效"); yield break; }

        Vector2Int CellPos = GridManager.Instance.WorldToCell(pos);
        patrolPos=GridManager.Instance.CellToWorld(CellPos);
        Debug.Log("开始巡逻,巡逻位置：" + patrolPos);
        blackboard.Set("patrolPos", patrolPos.Value);
    }
    private void CancelPatrol()
    {
        if (patrolPos.HasValue)
        {
            patrolPos = null;
            blackboard.Remove("patrolPos");
            Debug.Log("取消巡逻");
        }
    }
    private void OnPatrolReached()
    {
        patrolPos = null;
        blackboard.Remove("patrolPos");
    }
}
