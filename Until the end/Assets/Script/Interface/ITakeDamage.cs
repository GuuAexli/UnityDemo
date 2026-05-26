using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeDamage
{
    bool TakeDamage(DamageInfo info);
    void ApplyDamage(float damage, UnitAttribute atkUnit);
}
public struct DamageInfo
{
    public float damage;
    public float penetration;      // ¥©…Ó÷µ
    public UnitAttribute unit;
    public UnitAttribute atkUnit;
}


