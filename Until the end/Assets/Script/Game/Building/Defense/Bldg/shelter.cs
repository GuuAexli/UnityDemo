using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shelter : DefenseBldg
{

    public override void EnterEffect(UnitAttribute unit)
    {
        if(unit is InfantryAttribute inf)
        {
            inf.actualUnitVolume -= effectValue_F;
            if (inf.actualUnitVolume < 0.1)
                inf.actualUnitVolume = 0.1f;
            //Debug.Log(inf.actualUnitVolum);
        }
    }
    public override void ExitEffect(UnitAttribute unit) 
    {
        if(unit is InfantryAttribute inf)
        {
            inf.actualUnitVolume = inf._unitVolume;
            //Debug.Log(inf.actualUnitVolum);
        }
        
    }
    
}
