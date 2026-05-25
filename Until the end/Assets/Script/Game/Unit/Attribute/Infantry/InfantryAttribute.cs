using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryAttribute : UnitAttribute,IFear
{
    [SerializeField] private bool canMoveAttack;//¿ÉÒÔÒÆ¶¯¹¥»÷
   
    private void Update()
    {        
        ReduceFear();
    }
    public void AddFear(float value)
    {
        fear += value;
        if (fear >= 120)
            fear = 120;
        UIEvent.UpdateUnitInfo?.Invoke();
    }
    public void ReduceFear()
    {
        fear -= decayFear*Time.deltaTime;
        if(fear<=0)
            fear = 0;
        UIEvent.UpdateUnitInfo?.Invoke();
    }
    public float _fear
    {
        get { return fear; }
        set { fear = Mathf.Clamp(value, 0, 200); }
    }
}
