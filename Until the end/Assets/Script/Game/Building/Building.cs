using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] private BuildingData data;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float effectValue_F;
    [SerializeField] protected int effectValue_I;
    public float health;

    public bool complete;
    private void Awake()
    {
        ApplyState();
    }
    public void Complete(Vector3 pos)
    {
        complete = true;
        transform.position = pos;
    }
    public void ApplyState()
    {
        maxHealth = data.maxHealth;
        health = maxHealth;
        effectValue_F = data.effectValue_F;
        effectValue_I= data.effectValue_I;
    }
    public void HitDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
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



