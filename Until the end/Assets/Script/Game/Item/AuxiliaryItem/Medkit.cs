using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : FunctionItem
    //医疗包
{
    protected override void ActiveEffect(Vector2 spawnPos, GameObject unit)
    {
        
        if (unit != null)
        {
            unit.GetComponent<UnitAttribute>()?.AddHealth(effectValue);
            Debug.Log(unit+"获得回复");
        }
        else
        {
            if (effectPrefab!=null)
            {
                Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            } 
        }
        DisposableItem();
    }
    protected override GameObject GetTarget(Collider2D hitTarget)
    {
        if (hitTarget.GetComponent<InfantryAttribute>() != null)
        {
            return hitTarget.gameObject;
        }
        else { return null; }
    }
}
