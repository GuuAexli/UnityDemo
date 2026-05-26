using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class DefenseBldg : Building
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
    public void EnterEffect(UnitAttribute unit)
    {
        if(unit is InfantryAttribute inf)
        {
            inf.SetVolumeFactor(VolumeFactorType.Cover, effectValue_F);
        }
    }
    public void ExitEffect(UnitAttribute unit)
    {
        if (unit is InfantryAttribute inf)
        {
            inf.RemoveVolumeFactor(VolumeFactorType.Cover);
        }
    }
    
}//·ÀÓùÑÚÌå
