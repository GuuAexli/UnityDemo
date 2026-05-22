using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    [SerializeField]protected BulletData bulletData;
    //子弹数据
    public BulletData _bulletData => bulletData;
    
    
    [Header("目标相关")]
    public UnitAttribute shooter;//发射者 

    public UnitAttribute target = null;//打击的目标
    public Vector3 attackPos;//攻击位置
    public bool hitPos;//是攻击位置
    public bool hitTarget = false;//打中目标

    [Header("子弹属性")]
    [SerializeField] protected float bulletSpeed = 2f;//速度
    public float bulletDamage;//伤害
    public  float bulletPenetration;//穿深
    [Header("效果")]
    [SerializeField] protected GameObject explosion;//爆炸效果
    [SerializeField] protected GameObject traces;//痕迹

    public float bulletDuffusion;//扩散 武器决定
    [Header("距离相关")]
    public Vector2 fierPos;//射击时的位置
    public float bulletFlightDistance;//飞行距离
    
    [Header("存在时间")]
    [SerializeField] private float bulletTime;
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        ApplyData();
    }
    private void Start()
    {
        bulletFlightDistance += Random.Range(-1f, 1f);
        //未命中 子弹距离目标落点距离
    }
    // Update is called once per frame
    void Update()
    {
        if (hitTarget)
        {
            Attack();
        }//命中目标
    }//命中
    private void FixedUpdate()
    {
        if (!hitTarget)
        {
            rb.velocity = transform.up * bulletSpeed;
            if (bulletFlightDistance < Vector3.Distance(fierPos, transform.position))
                  Destroy(gameObject);
        }//没有命中目标
    }//未命中
    private void OnDestroy()
    {
        if (explosion != null)
        {     
            Instantiate(explosion, transform.position,transform.rotation);
        }
        if (traces != null)
        {
            Instantiate(traces, transform.position, transform.rotation);
        }
    }//销毁效果
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject != null)
        {
            UnitAttribute hitTarget = col.gameObject.GetComponent<UnitAttribute>();
            //获取目标的脚本
            if (hitTarget != null&&hitTarget==target)  
            {
                Damage(hitTarget);
            }//不是空的
        }//碰撞的是打击目标  否则穿过
    }//碰撞
    public void Attack()
    {
        if (hitPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                                        attackPos, bulletSpeed * Time.deltaTime);
            if (transform.position == attackPos)
            {
                Destroy(gameObject);
            }
        }//有攻击位置  优先打击攻击位置
        else if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                                       target.transform.position, bulletSpeed * Time.deltaTime);
        }//目标不为空 移动到目标
        else
        {
            hitTarget = false;
        }//假如目标被销毁 未命中
    }
    public void AttackPos(Vector3 _attackPos)
    {
        hitTarget = true;
        hitPos = true;
        attackPos = _attackPos;
    }//攻击指定位置
    public void AttackUnit(UnitAttribute _target)
    {
        hitTarget = true;
        target= _target;
    }
    private void Damage(UnitAttribute hitTarget,UnitAttribute unit=null)
    {
        ITakeDamage damageable=hitTarget.GetComponent<ITakeDamage>();
        if (damageable == null)
        {
            Destroy(gameObject);
            return;
        }//有伤害接口

        DamageInfo info = new DamageInfo
        {
            damage = bulletDamage,//伤害
            unit = hitTarget,//命中目标
            atkUnit=unit,//攻击发起者
            penetration=bulletPenetration//穿透率
        };//伤害信息

        bool isProbability=damageable.TakeDamage(info);
        //
        if (isProbability)
        {
            Destroy(gameObject);
        }
        else
        {
            Ricochet();
        }
    }//伤害计算
    public void SetBullet(bool hit, UnitAttribute _target,
                        UnitAttribute _shooter, float penetration,
                        float damage, float distance, float duffusion, Vector2 _fierPos)
    {
        shooter = _shooter;//给予发射者信息
        fierPos = _fierPos;//设置发射位置
        hitTarget = hit;//命中
        target = _target;//设置目标单位
        fierPos = _fierPos;//射击位置（记录飞行距离）
        bulletDamage *= 1 + damage;//武器伤害倍率*子弹基础伤害
        bulletPenetration *= 1 + penetration;//武器穿甲倍率*子弹基础穿甲
        bulletDuffusion = duffusion;//武器散布(未命中)
        bulletFlightDistance += distance;//飞行距离(未命中)

        if (!hitTarget)
        {
            //未命中
            Quaternion dispersion = Quaternion.Euler(0, 0, Random.Range(-bulletDuffusion, bulletDuffusion));
            //生成散布偏转
            transform.rotation = transform.rotation * dispersion;
            target = null;//设置打击目标
            Destroy(gameObject, bulletTime);
        }
    }//武器攻击使用
    private void Ricochet()
    {
        //Debug.Log("跳弹");
        hitTarget = false;
        Quaternion ricochet = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        transform.rotation = transform.rotation * ricochet;
        fierPos = transform.position;
        Destroy(gameObject,2);
    }//跳弹
    private void ApplyData()
    {
        bulletDamage = bulletData.bulletDamage;//伤害
        bulletSpeed=bulletData.bulletSpeed;//速度
        bulletPenetration = bulletData.bulletPenetration;// 穿深
        bulletTime=bulletData.bulletTime;//存在时间
    }
}
