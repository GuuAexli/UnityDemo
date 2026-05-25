using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shelter : DefenseBldg
{

    public override void EnterEffect(UnitAttribute unit)
    {
        if(unit is InfantryAttribute inf)
        {
            inf.SetVolumeFactor(VolumeFactorType.Cover, effectValue_F);
        }
    }
    public override void ExitEffect(UnitAttribute unit) 
    {
        if(unit is InfantryAttribute inf)
        {
            inf.RemoveVolumeFactor(VolumeFactorType.Cover);
        }
        
    }
    
}//ΡΪΜε
