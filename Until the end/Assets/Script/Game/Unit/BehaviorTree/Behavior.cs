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
public class UseItmeBehavior : ActionNode
{
    public UseItmeBehavior(Blackboard bb):base(bb) { }

    public override BTStatus Tick()
    {
        UnitAttribute attr = blackboard.Get<UnitAttribute>("attribute");
        UnitNavMove move = blackboard.Get<UnitNavMove>("navMove");
        UnitCombat combat = blackboard.Get<UnitCombat>("combat");
        Item item = blackboard.Get<Item>("useItem");

        if (attr == null || move == null) return BTStatus.Failure;
        if (!blackboard.HasKey("useItem")|| item.target == null) 
        { 
            Debug.Log("已经无法使用道具");
            blackboard.Remove("useItem");
            return BTStatus.Failure;
        }//目标被销毁 行为失败

        if (item.move)
        {
            attr._canAttack = false;
            attr._canMove = true;
            return BTStatus.Running;
        }//需要移动         
        else
        {
            if (item.use)
            {
                Debug.Log(item._itemName + "使用完成");
                blackboard.Remove("useItem");
                return BTStatus.Success;
            }//使用成功 行为完成
            attr._canAttack = false;
            attr._canMove = false;
            return BTStatus.Running;//继续
        }//停止移动与攻击 专注使用道具
    }//使用道具
}