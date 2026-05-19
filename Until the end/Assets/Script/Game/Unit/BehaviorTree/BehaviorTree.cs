using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BTNode
{ 
    protected Blackboard blackboard;
    public BTNode(Blackboard bb) => blackboard = bb;
    public abstract BTStatus Tick();//行为
}
public abstract class CompositeNode : BTNode 
{
    protected List<BTNode> children;//子节点列表
    public CompositeNode(Blackboard bb,List<BTNode> nodes) : base(bb)//=>BTNode.BTNode(bb)
    {
        children = nodes;
    }//当前组合节点拥有并记录它的所有子节点列表
}//组合节点

public class SelectorNode : CompositeNode
{
    public SelectorNode(Blackboard bb,List<BTNode> node) : base(bb, node) { }
    public override BTStatus Tick()
    {
        foreach(var child in children)
        {
            var status = child.Tick();
            if (status == BTStatus.Success)
                return BTStatus.Success;
            if(status == BTStatus.Running)
                return BTStatus.Running;

            // 如果Failure 则继续下一个
        }
        return BTStatus.Failure;
    }//依次执行子节点，第一个返回 成功 或 运行中 就停止，全部 失败 才返回 失败
}//选择器
public class SequenceNode : CompositeNode 
{
    public SequenceNode(Blackboard bb,List<BTNode> node): base(bb, node) { }
    public override BTStatus Tick()
    {
        foreach(var child in children)
        {
            var status = child.Tick();
            if(status==BTStatus.Failure)
                return BTStatus.Failure;
            if (status == BTStatus.Running)
                return BTStatus.Running;

            //如果Success 则继续下一个
        }//依次执行子节点，所有都 成功 才返回 成功，任意 失败 则返回 失败，运行中 则暂停
        return BTStatus.Success;
    }//序列节点
}
public class ConditionNode : BTNode
{
    public System.Func<bool> condition;
    public BTNode child;
    public ConditionNode(Blackboard bb, System.Func<bool> cond, BTNode chilnode) : base(bb)
    {
        condition = cond;
        child = chilnode;
    }
    public override BTStatus Tick()
    {
        bool condResult = condition();
        return condition() ? child.Tick() : BTStatus.Failure;
    }  
}//条件节点 根据条件只有 成功/失败

public abstract class ActionNode : BTNode 
{
    public ActionNode(Blackboard bb):base(bb) { }
}//动作节点 

