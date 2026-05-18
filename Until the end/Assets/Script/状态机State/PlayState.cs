using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine=new StateMachine();

        StateMachine.AddState(new IdleState(this));
        //添加并关联状态目标
        StateMachine.AddState(new ClickState(this));
        
        StateMachine.ChangeState<IdleState>();
        //改变状态             进入 闲置状态

    }

    void Start()
    {
        
    }

    void Update()
    {
        StateMachine.Update();
        //运行状态机  根据状态机的具体来控制单位
    }
}
