
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickState : InState
    //具体状态
{
   public  PlayState play;
    //目标

    public  ClickState(PlayState play)
        //关联状态目标
    {
        this.play = play;
    }
   public void Enter()
        //进入
    {
        Debug.Log("Enter Click");
    }
    public void Exit()
        //退出
    {
        Debug.Log("Exit Click");
    }
    public void Update()
        //运行
    {
        if (!Input.GetMouseButton(0))
            play.StateMachine.ChangeState<IdleState>();
    }

}
