using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetRangeObj 
{


    public static UnitAttribute GetMinDistanceTarget(float range,Faction targetFaction,Vector3 pos)
    {
        List<UnitAttribute> unitList = GetRangeUnit(range,targetFaction, pos);

        if (unitList == null) { Debug.Log("范围内没有可用目标");return null; }
        float minDistance = Mathf.Infinity;
        UnitAttribute target=null;
        foreach(UnitAttribute unit in unitList)
        {
            float distnace=Vector3.Distance(pos,unit.transform.position);
            if(distnace < minDistance) 
            {
                target=unit;
                minDistance=distnace;
            }//选择最近距离单位
        }
        return target;
    }
    public static UnitAttribute GetMinHealthUnit(float range, Faction targetFaction, Vector3 pos)
    {
        List<UnitAttribute> unitList = GetRangeUnit(range, targetFaction,pos);
        if (unitList == null) { Debug.Log("范围内没有可用目标"); return null; }

        float minHealth = Mathf.Infinity;
        UnitAttribute target=null;
        foreach(UnitAttribute unit in unitList)
        {
            float health=unit.health;
            if(health!=unit.maxHealth&&health < minHealth)
            {
                target=unit; minHealth=health;
            }//选择最小健康值单位
        }
        return target;
    }
    public static List<UnitAttribute> GetRangeUnit(float range,Faction targetFaction, Vector3 pos)
    {
        Collider2D[] colList = Physics2D.OverlapCircleAll
                                (pos, range, LayerMask.GetMask("unit"));
        List<UnitAttribute> unitList = new List<UnitAttribute>();
        foreach (Collider2D col in colList)
        {
            UnitAttribute unit = col.GetComponent<UnitAttribute>();
            if (unit != null && unit.faction == targetFaction)
            {
                unitList.Add(unit);
            }
        }
        return unitList;
    } 
}
