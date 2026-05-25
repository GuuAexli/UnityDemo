using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IUnitState 
{
    public void OnEnter();//进入激活
    public void OnUpdate();//持续激活
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
        unit.actualUnitVolume = unit._unitVolume;
    }
    public void OnUpdate()
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
        unit.SetVolumeFactor(VolumeFactorType.Posture, 0.5f);//设置体积因子 姿态
        unit.SetMoveEfficiency(.5f);
        unit.SetCombatEfficiency(.5f);
        unit.posture.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {
        unit.RemoveVolumeFactor(VolumeFactorType.Posture);//移除体积因子 姿态
        unit.SetMoveEfficiency();
        unit.SetCombatEfficiency();
        unit.posture.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
public class SuppressdPosture : IUnitState 
{
    UnitAttribute unit;
    public SuppressdPosture(UnitAttribute u) { unit = u; }
    public void OnEnter()
    {
        unit.posture.GetComponent<SpriteRenderer>().color = Color.red;
        unit.SetVolumeFactor(VolumeFactorType.Posture, 0.5f);
        unit.SetMoveEfficiency(0);
        unit.SetCombatEfficiency(0);
    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {
        unit.posture.GetComponent<SpriteRenderer>().color = Color.white;
        unit.RemoveVolumeFactor(VolumeFactorType.Posture);
        unit.SetMoveEfficiency();
        unit.SetCombatEfficiency();
    }
}//被压制



