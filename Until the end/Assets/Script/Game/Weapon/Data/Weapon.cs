using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
//实际的武器
public  class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData data;
    
    public UnitCombat owner { get; protected set; }//武器拥有者
    [Header("武器UI")]
    public string weaponName;
    public Sprite weaponIcon;
    public SpriteRenderer weaponRenderer;
    /////////////////////////武器时间关联///////////////////////
    public float attackDuration { get; protected set; }//攻击持续时间
    [SerializeField] protected float currentDurationTime;//现在持续时间
    public float aimTime { get; protected set; }//瞄准时间
     public float attackInterval { get; protected set; }//攻击间隔
     public float attackCooling { get; protected set; }//攻击冷却
    [SerializeField] private float currentAttackTime;//现在攻击时间

    /////////////////////////武器属性///////////////////////////
    [SerializeField] protected Transform fierPos;//发射位置
    public  GameObject bullet { get; protected set; }//子弹
    public float suppressionValue { get; protected set; }//压制值
    public float attackRange { get; protected set; }//攻击半径
    public float duffusion { get; protected set; }//散布

    ///////////////////////////精准度///////////////////////////
    public float closeRangeAccurracy { get; protected set; }//近距离精准度
    public float mediumRangeAccurracy { get; protected set; }//中距离精准度
    public float longRangeAccurracy { get; protected set; }//远距离精准度

    ///////////////////////////倍率//////////////////////////////////
    public float penetrationMultiplier { get; protected set; }//深穿倍率
    public float damageMultiplier { get; protected set; }//伤害倍率

    /////////////////////////载具特有///////////////////////////
    public float VehicleRotatSpeed;//载具炮台旋转速度

    [Header("状态判断")]
    [SerializeField] private bool isAttackCooling = false;//正在攻击冷却
    [SerializeField] private bool isSelected;//现在是选择状态
    //public abstract void Attack(GameObject target,GameObject shooter);
    public  void Awake()
    {
        weaponRenderer = GetComponent<SpriteRenderer>();
        if (weaponRenderer == null)
            weaponRenderer.AddComponent<SpriteRenderer>();
        ApplyWeaponData();
    }
    public void Attack(UnitAttribute target, UnitAttribute shooter,float targetDistance)
        //                         目标           所有者                 距离
    {

        if (target != null)
        {
            if (!isAttackCooling){     
                if ((currentDurationTime+=Time.deltaTime)>=aimTime)
                    //攻击持续时间>现在持续时间>瞄准射击
                    if ((currentAttackTime += Time.deltaTime) >= attackInterval&&owner.isTowardsTarget)     
                    {
                            GameObject newBullet = Instantiate(bullet, fierPos.position,
                                                                    fierPos.rotation);
                            SetBullet(newBullet, target, shooter, targetDistance, 
                                                        fierPos.position);
                            target.GetComponent<IFear>()?.AddFear(suppressionValue);
                            //增加目标恐惧值 如果是步兵类型
                            currentAttackTime = 0;
                        if (currentDurationTime >= attackDuration)  
                            isAttackCooling=true;
                        //攻击持续时间
                    }//单次攻击间隔 &&所有者可以攻击目标（朝向目标）
            }//不在射击冷却 
            else
            {
                currentDurationTime = 0;
                if ((currentAttackTime += Time.deltaTime) >= attackCooling)
                {
                    currentAttackTime = 0;
                    isAttackCooling = false;
                }
            }
        }
    }
    public void SetOwner(UnitCombat newOwner)
    {
        owner = newOwner;
        isSelected=false;
    }
    public void SetBullet(GameObject bulletPrefab, UnitAttribute target, UnitAttribute shooter,float targetDistance,Vector2 fierPos)
    {
        Bullet bullet=bulletPrefab.GetComponent<Bullet>();
        bool hit = HitTarget(owner._unitAccurracy, target, targetDistance);

        bullet.SetBullet(hit,target,owner.attr,penetrationMultiplier,damageMultiplier,
                        targetDistance,duffusion,fierPos);

        owner.attr?.AddExp(2);
        //给予 发射者 经验值
    }
    
    public bool HitTarget(float unit,UnitAttribute targetVolum,float targetDistance)
    {
        /*
        //计算命中率 并判断 每一次攻击是否命中目标
        //单位命中率*目标体积*距离影响
        //uintAccurracy  * target.volum *distanceAccurracy= 命中率
        //0.3+*0.9*0.4=0.108
        */

        float distanceAccurracy = targetDistance switch
        //距离命中率变化
        {
            _ when targetDistance >= attackRange*0.66 => longRangeAccurracy,
            _ when targetDistance >= attackRange*0.33 => mediumRangeAccurracy,
            _ => closeRangeAccurracy
        };
        //Debug.Log("命中率=" + (unit * targetVolum.GetComponent<UnitAttribute>().actualUnitVolum) * distanceAccurracy);
        
        if (Random.Range(0f, 1f) <= (unit * targetVolum.actualUnitVolume)*distanceAccurracy)
        {
            //Debug.Log("命中");
            return true;
        }
        else 
        { 
            //Debug.Log("未命中");
            return false; 
        } 
    }
    public void ApplyWeaponData()
    {
        weaponName = data.prefabName;
        aimTime = data.aimTime;
        attackDuration = data.attackDuration;
        attackRange = data.attackRange;
        bullet = data.bullet;
            closeRangeAccurracy = data.closeRangeAccurracy;
            mediumRangeAccurracy=data.mediumRangeAccurracy;
            longRangeAccurracy = data.longRangeAccurracy;
        attackInterval = data.attackInterval;
        attackCooling = data.attackCooling;
        penetrationMultiplier = data.penetrationMultiplier;
        damageMultiplier = data.damageMultiplier;
        duffusion = data.duffusion;
        weaponIcon = data.weaponIcon;
        weaponRenderer.sprite = weaponIcon;
        VehicleRotatSpeed = data.barbetteRotatSpeed;
        suppressionValue = data.suppressionValue;
    }
    public bool _isAttackCooling => isAttackCooling;
    public float _rotateSpeed=>VehicleRotatSpeed;
    public Transform _fierPos
    {
        get { return fierPos; }
        //获取值的方式 外部函数获取fierPos的值通过FierPos
        //unitFierPos=FierPos
        set { fierPos = value; }
        //修改值的方式 外部修改fierPos通过FierPos
        //FierPos=unitFierPos
    }
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value; }
    }
    }
    
    

