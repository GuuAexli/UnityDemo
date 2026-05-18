using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    public Collider2D col;
    public UnitAttribute unit;
    public int attackHit;

    private void Awake()
    {
        col= GetComponent<Collider2D>();    
    }
    private void Start()
    {
        Destroy(gameObject, 10);
    }

    public void  OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<UnitAttribute>() != null)
            AttackUnit(other.GetComponent<UnitAttribute>());
    }
    
    public void AttackUnit(UnitAttribute unit)
    {
        Debug.Log(unit.health);
        unit.health -= attackHit;
        Debug.Log(unit.health);
    }
}
