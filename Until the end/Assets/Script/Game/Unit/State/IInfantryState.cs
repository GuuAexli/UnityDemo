using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInfantryState
{
    public  void Enter(InfantryAttribute unit);
    public  void Update(InfantryAttribute unit);
    public  void Exit(InfantryAttribute unit);

}

public abstract class BaseUnitMachine //: MonoBehaviour
{
    

    public  IInfantryState currentState;
    protected Dictionary<Type, IInfantryState> states = new Dictionary<Type, IInfantryState>();

    public virtual void Awake()
    {
        //InitializationState();
    }
    public void AddState<T>(T state) where T:IInfantryState
    {
        states[typeof(T)]=state;
    }
    /*public void ChangeState<T>()where T : IUnitState
    {
        if(states.TryGetValue(typeof (T),out IUnitState newState))
        {
            currentState?.Exit(unit);
            currentState = newState;
            currentState.Enter(unit);
        }
    }*/
    
    
    public virtual void UpdateMachine()
    {
        
    }
}
public class InfantryStateMachine : BaseUnitMachine
{
    //protected IUnitState currentState;
    public InfantryAttribute unit;
    protected bool isNormalState => currentState is InfantryNormalState;
    protected bool isFearState => currentState is InfantryFearState;
    protected bool isSuppressionState => currentState is InfantrySuppressionState;

    public override void Awake()
    {
        InitializationState();
        //Debug.Log(unit);
    }
    public override void UpdateMachine( )
    {
        ChangeState(UpdateFearState());
        //Debug.Log(currentState);
        currentState?.Update(unit);
    }
    public IInfantryState  UpdateFearState()
    {
        if (unit == null)
        {
            Debug.Log("单位是空的");
            return null;
        }
        else
        {
            if (unit._fear > 0)
                ReduceFear();

            if (unit._fear >= 100f)
                return states[typeof(InfantrySuppressionState)];

            if (unit._fear >= 20f)
                return states[typeof(InfantryFearState)];

            return states[typeof(InfantryNormalState)];
        }
    }
    protected void InitializationState()   
    {
        AddState<InfantryNormalState>(new InfantryNormalState());
        AddState<InfantryFearState>(new InfantryFearState());//恐惧
        AddState<InfantrySuppressionState>(new InfantrySuppressionState());//压制
        //states[typeof(InfantryNormalState)] = new InfantryNormalState();//正常
        //states[typeof(InfantryFearState)] = new InfantryFearState();//恐惧
        //states[typeof(InfantrySuppressionState)] = new InfantrySuppressionState();//压制
        currentState = states[typeof(InfantryNormalState)];
    }//初始化状态
    protected void ChangeState(IInfantryState newState)
        //改变/切换 状态
    {
        if(newState== null)
        {
            //Debug.Log("返回状态为空");
            return;
        }

        if (currentState != newState)
        {
            currentState?.Exit(unit);
            currentState = newState;
            currentState.Enter(unit);
        }
    }
    protected void ReduceFear()
    {
        unit._fear -= unit._decayFear;
    }
}

public class VehicleStateMachine : BaseUnitMachine
{

}


