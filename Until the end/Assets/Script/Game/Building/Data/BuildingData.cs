using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : Data
{
    public float effectValue_F;
    public int effectValue_I;
    public float maxHealth;
    public float volume;
    public int armor;
    public override void Spawn()
    {
        Instantiate(prefab);
    }
}
