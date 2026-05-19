using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitState 
{
    public void OnEnter();//进入激活
    public void OnUpdate(float deltaTime);//持续激活
    public void OnExit();//离开激活
}
public class StandingPosture : IUnitState
{
    private UnitAttribute unit;
    public StandingPosture(UnitAttribute u) {  unit=u; }
    //构造函数 公开方法
    public void OnEnter()
    {
        Debug.Log("姿态：站立");
    }
    public void OnUpdate(float dt)
    { 
    
    }
    public void OnExit()
    {

    }
}
public class PronePosture : IUnitState 
{
    private UnitAttribute unit;
    public  PronePosture(UnitAttribute u) { unit = u; }

    public void OnEnter()
    {
        Debug.Log("姿态：匍匐");
    }
    public void OnUpdate(float dt)
    {

    }
    public void OnExit()
    {

    }
}


