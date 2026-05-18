using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantrySuppressionState : IInfantryState
{

    public void Enter(InfantryAttribute unit)
    {
        //unit.unitMove.actualMoveSpeed = 0;
        unit.GetComponent<InfantryAttribute>()._canAttack=false;

        //Debug.Log("suppress");
    }
    public void Update(InfantryAttribute unit)
    {
        if(unit.isAttack!=false)
            unit.isAttack = false;//处于攻击状态会导致不能旋转 

    }
    public void Exit(InfantryAttribute unit)
    {
        //unit.unitMove.actualMoveSpeed = unit.unitMove.moveSpeed;
        unit.GetComponent<InfantryAttribute>()._canAttack = true;
        //Debug.Log("Exit suppress");
    }
}
