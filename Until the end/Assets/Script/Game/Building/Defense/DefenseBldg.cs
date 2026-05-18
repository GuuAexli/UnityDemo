using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefenseBldg : Building
{
    public void OnTriggerEnter2D(Collider2D col)
    {
        UnitAttribute unit = col.GetComponent<UnitAttribute>();
        if (unit != null)
        {
            EnterEffect(unit);
        }
    }
    public void OnTriggerExit2D(Collider2D col)
    {
        UnitAttribute unit = col.GetComponent<UnitAttribute>();
        if (unit != null)
        {
            ExitEffect(unit);
        }
    }
    public  abstract void EnterEffect(UnitAttribute unit);

    public abstract void ExitEffect(UnitAttribute unit);
    
}
