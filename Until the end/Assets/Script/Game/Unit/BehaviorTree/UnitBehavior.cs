using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        Debug.Log("3");
        return BTStatus.Running;
    }
}//闲置 正常移动与攻击

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

            if (move.isMove == false)
            {
                return BTStatus.Success;
            }//已经不需要移动 （移动终止 到达目标点）
            else
            {
                return BTStatus.Running;
            }//继续移动到目标点
        }//没有发现敌人继续移动
    }
}//战斗巡逻 发现敌人停止运动 直到敌人消失