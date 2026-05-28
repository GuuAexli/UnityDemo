using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionData data;//爆炸效果数据

    [SerializeField] private float range;//范围
    [SerializeField] private float damage;//伤害
    [SerializeField] private float penetration;//穿深
    [SerializeField] private float delay;//延迟
    [SerializeField] private float fear;//恐惧值

    public float _range => range;
    private void Awake()
    {
        ApplyData();
    }
    private void Start()
    {
        Invoke("EnterEffect", delay);
        LineEvent.ShowExplosEvent(new ShowExplosionEvent 
                                { explosion = this, pos = transform.position,show=true });
    }
    private void OnDestroy()
    {
        
    }
    protected void EnterEffect()
    {
        if(data.clip!=null) GetComponent<AudioSource>().PlayOneShot(data.clip);
        HashSet<GameObject> processedUnit=new HashSet<GameObject>();
        //单位可能有碰撞体和触发器 导致判断两次
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position,
                                            range, LayerMask.GetMask("unit"));
        foreach(Collider2D col2 in col)
        {
            GameObject obj = col2.gameObject;
            if (processedUnit.Contains(obj)) continue;
                processedUnit.Add(obj);
            //将已经处理过的单位加入组，防止因为 碰撞体 和 触发器 导致 多次判断

            ITakeDamage damageable = col2.GetComponent<ITakeDamage>();
            DamageInfo info = new DamageInfo 
            {
                damage=this.damage,
                unit=obj.GetComponent<UnitAttribute>(),
                penetration=this.penetration
            };
                
            damageable?.TakeDamage(info);
            if (info.unit is InfantryAttribute inf)
            {
                inf.AddFear(fear);
            }//增加恐惧
        }        
        ExitEffect();
        LineEvent.ShowExplosEvent(new ShowExplosionEvent
            { explosion = this, pos = transform.position, show = false });
        Destroy(gameObject, delay);
    }
    protected void ExitEffect()
    {

    }
    private void Destance(Collider2D col)
    {
        float distance = Vector2.Distance(col.transform.position,transform.position);

    }//距离
    public void NoHit()
    {
        damage *= 0.4f;
    }
    private void ApplyData()
    {
        range = data.range;
        damage = data.damage;
        penetration = data.penetration;
        delay = data.delay;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,range);
    }
}
