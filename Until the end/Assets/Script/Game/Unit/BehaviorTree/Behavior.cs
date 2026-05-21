using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
///  闲置行为
/// </summary>
public class IdleBehavior : ActionNode 
{
    public IdleBehavior(Blackboard bb): base(bb) { }

    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>("attribute");
        UnitNavMove move = blackboard.Get<UnitNavMove>("navMove");
        UnitCombat combat = blackboard.Get<UnitCombat>("combat");

        if (attr == null || move == null) return BTStatus.Failure;

        attr._canAttack = true;
        attr._canMove = true;

        return BTStatus.Running;
    }
}//闲置 正常移动与攻击

/// <summary>
/// 巡逻攻击行为
/// </summary>
public class PatrolAttackBehavior : ActionNode 
{
    public PatrolAttackBehavior(Blackboard bb) : base(bb) { }

    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>("attribute");
        UnitNavMove move = blackboard.Get<UnitNavMove>("navMove");
        UnitCombat combat = blackboard.Get<UnitCombat>("combat");

        if (attr==null||move==null) return BTStatus.Failure;

        if (!blackboard.HasKey("patrolPos"))
            return BTStatus.Failure;
        //获取巡逻目标
        attr._canAttack = true;
        move.SetMovePos(blackboard.Get<Vector3>("patrolPos"));
        if (move.isMove == false || move.targetPos != blackboard.Get<Vector3>("patrolPos"))
        {
            Debug.Log("取消巡逻行为");
            blackboard.Remove("patrolPos");
            return BTStatus.Success;
        }//已经不需要移动 （移动终止 到达目标点 更改位置）

        if (combat != null && combat.ClosestTarget() != null)
        {
            attr._canMove = false;
            attr._canAttack = true;
            return BTStatus.Running;
        }//发现敌人 停止移动直到敌人消失
        else
        {
            attr._canMove= true;
            attr._canAttack = true;            
            return BTStatus.Running;
        }//没有发现敌人继续移动
    }
}//战斗巡逻 发现敌人停止运动 直到敌人消失
public class ForcedMoveBehavior : ActionNode 
{
    public ForcedMoveBehavior(Blackboard bb):base(bb) { }
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>("attribute");
        UnitNavMove move = blackboard.Get<UnitNavMove>("navMove");
        UnitCombat combat = blackboard.Get<UnitCombat>("combat");

        if(attr == null||combat==null||move==null) return BTStatus.Failure;

        if (!blackboard.HasKey("forcedMove")){ Debug.Log("路径错误");return BTStatus.Failure; }

        if (!move.isMove || move.targetPos != blackboard.Get<Vector3>("forcedMove"))
        {
            Debug.Log("不在需要强制移动");
            return BTStatus.Success;
        }//不在移动||目标改变

        attr._canAttack = false;
        attr._canMove = true;
        return BTStatus.Running;
    }
}//强制移动
public class MoveToRangeBehavior : ActionNode 
{
    public MoveToRangeBehavior(Blackboard bb):base(bb) { }
    float distance;
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>("attribute");
        UnitNavMove move = blackboard.Get<UnitNavMove>("navMove");
        Item item=blackboard.Get<Item>("moveToRange");

        if(attr == null||move==null||item==null) return BTStatus.Failure;

        if (!blackboard.HasKey("moveToRange")) { Debug.Log("已经不需要移动到范围"); return BTStatus.Failure; }

        move.SetMovePos(item.target.transform.position);
        distance = Vector3.Distance(attr.transform.position, item.target.transform.position);
        if(distance <= item._useRange) 
        {
            Debug.Log("到达范围内");
            attr.SetMove();
            blackboard.Remove("moveToRange");       
            return BTStatus.Success;
        }
        
        return BTStatus.Running;
    }
}

/// <summary>
/// 使用道具行为
/// </summary>
public class UseItemBehavior : ActionNode
{
    public UseItemBehavior(Blackboard bb):base(bb) { }
    float time=0f;
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>("attribute");
        UnitNavMove move = blackboard.Get<UnitNavMove>("navMove");
        UnitCombat combat = blackboard.Get<UnitCombat>("combat");
        Item item = blackboard.Get<Item>("useItem");

        if (attr == null || move == null||combat==null) return BTStatus.Failure;
        if (!blackboard.HasKey("useItem")|| item.target == null) 
        { 
            Debug.Log("已经无法使用道具");
            blackboard.Remove("useItem");
            return BTStatus.Failure;
        }//目标被销毁 行为失败
        if ((time+=Time.deltaTime)<item._delay)
        {
            attr._canAttack = false;
            attr._canMove = false;
            attr.SetMove();
            attr.transform.rotation=RotateHelper.RotateToUnit(attr.transform,item.target.transform,attr.rotateSpeed);
            return BTStatus.Running;
        }//使用道具 停止攻击和移动
        item.Use();
        blackboard.Remove("useItem");
        return BTStatus.Success;
    }//使用道具
}