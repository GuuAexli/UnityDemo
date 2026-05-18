using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryFearState : IInfantryState
    //²½±ø¿Ö¾å×´Ì¬
{

    public void Enter(InfantryAttribute unit)
    {
        //unit.unitMove.actualMoveSpeed = unit._moveSpeed * 0.5f;
        unit.actualUnitVolume *= 0.5f;
        //Debug.Log("fear");
    }
    public void Update(InfantryAttribute unit)
    {
        

        //if (unit.fear >= 80)
        //    unit.UnitStateMachine.ChangeState<InfantrySuppressState>();
        //if (unit.fear < 5)
        //    unit.UnitStateMachine.ChangeState<InfantryNormalState>();
        
    }
    public void Exit(InfantryAttribute unit)
    {
        //unit.unitMove.actualMoveSpeed = unit.unitMove.moveSpeed;
        unit.actualUnitVolume = unit._unitVolume;
        //Debug.Log("exit fear");
    }
}
