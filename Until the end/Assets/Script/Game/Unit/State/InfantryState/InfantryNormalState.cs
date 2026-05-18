using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryNormalState : IInfantryState
{

    public void Enter(InfantryAttribute unit)
    {
        //Debug.Log("enter Normal");
    }
    public void Update(InfantryAttribute unit)
    {
        //if (unit.fear >= 5)
        //    unit.UnitStateMachine.ChangeState<InfantryFearState>();

    }
    public void Exit(InfantryAttribute unit)
    {

    }
}
