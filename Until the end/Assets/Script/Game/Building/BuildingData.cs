using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingData : Data
{
    //public GameObject prefab;
    //影响的效果值
     public float effectValue_F;
     public int effectValue_I;
     public float maxHealth;

    public override void Spawn()
    {
        Instantiate(prefab);
    }
}
[CreateAssetMenu(fileName ="DefenseBldgData",menuName ="BuildingData/DefenseBldgData")]
public class DefenseBldgData : BuildingData 
{
    
}
[CreateAssetMenu(fileName ="FunctionBldgData",menuName ="BuildingData/FunctionBldgData")]
public class FunctionBldgData : BuildingData
{
    
}
