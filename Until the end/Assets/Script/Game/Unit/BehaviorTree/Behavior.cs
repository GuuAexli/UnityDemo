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
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        UnitCombat combat = blackboard.Get<UnitCombat>(BlackboardKeys.Combat);

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
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        UnitCombat combat = blackboard.Get<UnitCombat>(BlackboardKeys.Combat);

        if (attr==null||move==null) return BTStatus.Failure;

        if (!blackboard.HasKey(BlackboardKeys.PatrolPos))
            return BTStatus.Failure;
        //获取巡逻目标
        attr._canAttack = true;
        move.SetMovePos(blackboard.Get<Vector3>(BlackboardKeys.PatrolPos));
        if (move.isMove == false || move.targetPos != blackboard.Get<Vector3>(BlackboardKeys.PatrolPos))
        {
            Debug.Log("取消巡逻行为");
            blackboard.Remove(BlackboardKeys.PatrolPos);
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
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        UnitCombat combat = blackboard.Get<UnitCombat>(BlackboardKeys.Combat);

        if(attr == null||combat==null||move==null) return BTStatus.Failure;

        if (!blackboard.HasKey(BlackboardKeys.ForcedMove)){ Debug.Log("路径错误");return BTStatus.Failure; }

        if (!move.isMove || move.targetPos != blackboard.Get<Vector3>(BlackboardKeys.ForcedMove))
        {
            Debug.Log("不在需要强制移动");
            return BTStatus.Success;
        }//不在移动||目标改变

        attr._canAttack = false;
        attr._canMove = true;
        return BTStatus.Running;
    }
}//强制移动
public class MoveToItemRangeBehavior : ActionNode 
{
    public MoveToItemRangeBehavior(Blackboard bb):base(bb) { }
    float distance;
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        Item item=blackboard.Get<Item>(BlackboardKeys.MoveToItemRange);

        if(attr == null||move==null||item==null) return BTStatus.Failure;

        if (!blackboard.HasKey(BlackboardKeys.MoveToItemRange) ||item.target==null) { Debug.Log("已经不需要移动到范围"); return BTStatus.Failure; }

        move.SetMovePos(item.target.transform.position);
        distance = Vector3.Distance(attr.transform.position, item.target.transform.position);
        if(distance <= item._useRange) 
        {
            Debug.Log("到达范围内");
            attr.SetMove();
            blackboard.Remove(BlackboardKeys.MoveToItemRange);   
            distance= 0f;
            return BTStatus.Success;
        }
        
        return BTStatus.Running;
    }
}//移动到范围（自动）

/// <summary>
/// 自动使用道具行为
/// </summary>
public class UseItemBehavior : ActionNode
{
    public UseItemBehavior(Blackboard bb):base(bb) { }
    float time=0f;
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        UnitCombat combat = blackboard.Get<UnitCombat>(BlackboardKeys.Combat);
        Item item = blackboard.Get<Item>(BlackboardKeys.AssistantItem);

        if (attr == null || move == null||combat==null) return BTStatus.Failure;
        if (!blackboard.HasKey(BlackboardKeys.AssistantItem) || item.target == null) 
        { 
            Debug.Log("已经无法使用道具");
            blackboard.Remove(BlackboardKeys.AssistantItem);
            return BTStatus.Failure;
        }//目标被销毁 行为失败
        time += 1 * Time.deltaTime;
        if (time<item._delay)
        {
            attr._canAttack = false;
            attr._canMove = false;
            attr.SetMove();
            attr.transform.rotation=RotateHelper.RotateToUnit(attr.transform,item.target.transform,attr.rotateSpeed);
            return BTStatus.Running;
        }//使用道具 停止攻击和移动
        item.Use();
        time = 0f;
        attr._canAttack = true;
        attr._canMove = true;
        blackboard.Remove(BlackboardKeys.AssistantItem);
        return BTStatus.Success;
    }//使用道具
}//使用道具（自动）
public class ManualMoveToItemRangeBehavior : ActionNode
{
    public ManualMoveToItemRangeBehavior(Blackboard bb) : base(bb) { }
    float distance;
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        Item item = blackboard.Get<Item>(BlackboardKeys.ManualMoveToItemRange);

        if (attr == null || move == null || item == null) return BTStatus.Failure;
        attr._canAttack = true;
        attr._canMove = true;
        if (!blackboard.HasKey(BlackboardKeys.ManualMoveToItemRange)) { Debug.Log("已经不需要移动到范围"); return BTStatus.Failure; }

        move.SetMovePos(item.target.transform.position);
        distance = Vector3.Distance(attr.transform.position, item.target.transform.position);
        if (distance <= item._useRange)
        {
            Debug.Log("到达范围内");
            attr.SetMove();
            blackboard.Remove(BlackboardKeys.ManualMoveToItemRange);
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}//主动移动到范围

/// <summary>
/// 主动使用道具行为
/// </summary>
public class ManualUseItemBehavior : ActionNode
{
    public ManualUseItemBehavior(Blackboard bb) : base(bb) { }
    float time = 0f;
    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>(BlackboardKeys.Attribute);
        UnitNavMove move = blackboard.Get<UnitNavMove>(BlackboardKeys.NavMove);
        UnitCombat combat = blackboard.Get<UnitCombat>(BlackboardKeys.Combat);
        Item item = blackboard.Get<Item>(BlackboardKeys.ManualUseItem);

        if (attr == null || move == null || combat == null) return BTStatus.Failure;
        if (!blackboard.HasKey(BlackboardKeys.ManualUseItem) || item.target == null)
        {
            Debug.Log("已经无法使用道具");
            blackboard.Remove(BlackboardKeys.ManualUseItem);
            return BTStatus.Failure;
        }//目标被销毁 行为失败
        time += 1 * Time.deltaTime;
        if (time  < item._delay)
        {
            attr._canAttack = false;
            attr._canMove = false;
            attr.SetMove();
            attr.transform.rotation = RotateHelper.RotateToUnit(attr.transform, item.target.transform, attr.rotateSpeed);
            return BTStatus.Running;
        }//使用道具 停止攻击和移动
        item.Use();
        time = 0f;
        attr._canAttack = true;
        attr._canMove = true;
        blackboard.Remove(BlackboardKeys.ManualUseItem);
        return BTStatus.Success;
    }//使用道具
}//主动使用道具