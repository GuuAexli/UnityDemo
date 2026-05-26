using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleData", menuName = "UnitData/VehicleData")]
public class VehicleData : UnitData
{

    public List<GameObject> loadList = new List<GameObject>();//装入成员组
    public int maxLoad;                                   //最大装入量
}//载具
