using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportVehicle : VehicleAttribute,ILoadUnit
{
    public void LoadUnit(GameObject target)
    {
        LoadList loadList = GetComponent<LoadList>();
        if (loadList != null)
        {
            loadList.isLoad = true;
            loadList.targetUnit.Add(target);
        }
    }
}
