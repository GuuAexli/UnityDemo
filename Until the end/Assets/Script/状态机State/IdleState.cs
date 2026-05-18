using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : InState
{
    public PlayState play;
    //目标

    public  IdleState(PlayState play)
        //关联状态目标
    {
        this.play = play;
    }
    public void Enter()
        //进入
    {
        Debug.Log("Enter Idle");
    }
    public void Exit()
        //退出
    {
        Debug.Log("Exit Idle");
    }
    public void Update()
        //运行
    {
        if (Input.GetMouseButton(0))
            play.StateMachine.ChangeState<ClickState>();
    }
}
