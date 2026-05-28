using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Building : MonoBehaviour,ITakeDamage
{
    public BuildingData data;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float effectValue_F;
    [SerializeField] protected int effectValue_I;
    public float health;
    public float volume { get; protected set; }
    public int armor { get; protected set; }

    public bool complete;
    private void Awake()
    {
        ApplyState();
    }
    public virtual void Complete(Vector3 pos)
    {
        Debug.Log(data.prefabName + "建造完成");
        complete = true;
        transform.position = pos;
    }
    public void ApplyState()
    {
        maxHealth = data.maxHealth;
        health = maxHealth;
        effectValue_F = data.effectValue_F;
        effectValue_I= data.effectValue_I;
        armor=data.armor;
        volume = data.volume;

    }
    public bool TakeDamage(DamageInfo info)
    {
        float probability = info.penetration / armor;
        bool isProbability=(probability >= 1 || (probability >= 0.2 && Random.Range(0, 1f) >= probability));
        if(isProbability)
        {
            float damage = info.damage;
            if (probability < 1) damage*= probability;
            ApplyDamage(damage, info.atkUnit);
            return isProbability;
        }
        else { return isProbability; }
    }
    public void ApplyDamage(float damage,UnitAttribute atkUnit)
    {
        health -= damage;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void ReplyHealth(float valum)
    {
        health += valum;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }
}



