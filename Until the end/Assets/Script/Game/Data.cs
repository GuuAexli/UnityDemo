using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Data : ScriptableObject
{
    public string prefabName;
    public GameObject prefab;
    public int costValue;
    public string description;

    public abstract void Spawn();
}
