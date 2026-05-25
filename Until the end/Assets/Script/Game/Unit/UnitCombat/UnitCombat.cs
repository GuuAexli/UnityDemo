using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public  class UnitCombat : MonoBehaviour//执行战斗的脚本
{


    [SerializeField] protected bool isVehicle;
    [SerializeField] protected bool isSelected =>attr!=null && attr.isSelected;
    [Header("数据")]
    public UnitAttribute attr;//单位属性数据
    [Header("武器")]
    [SerializeField] protected Transform weaponParent;//武器挂点
    [SerializeField] protected GameObject weaponInstantiate;//武器实例化
    [SerializeField] protected GameObject currentWeapon;//现在的武器
    [SerializeField] public Weapon weapon { get; private set; }//武器组件
    [SerializeField] protected Sprite weaponIcon;//武器图片
    [Header("属性")]
    [SerializeField] protected float attackRange;//攻击范围
    public float unitAccurracy=>attr._unitAccurracy*attr.combatEfficiency;//单位精准度
    [SerializeField] protected float targetDistance;//目标距离
    public float rotateSpeed=>attr.rotateSpeed * attr.moveEfficiency;//单位旋转速度
    public bool isTowardsTarget { get; protected set; }//朝向目标
    [SerializeField] protected List<UnitAttribute> targetList = new List<UnitAttribute>();//目标组
    [SerializeField] protected UnitAttribute target;//选中的目标
    [SerializeField] protected UnitAttribute priorityTarget;//优先目标
    public UnitAttribute _priorityTarget{ get {return priorityTarget;} set { priorityTarget = value; } }

    private void Start()
    {
        attr = GetComponent<UnitAttribute>();
        
        isVehicle = attr.isVehicle;
        
        weaponInstantiate = attr.unitData.weapon
            [Random.Range(0, attr.unitData.weapon.Length)].prefab;
        ApplyWeapon();

    }

    private void Update()
    {
        if(attr._canAttack)
            Attack();
    }
    private void GetTargetList()
    {
        targetList.Clear();
        Collider2D[] hitTarget = Physics2D.OverlapCircleAll(transform.position, 
                                                                attackRange, 
                                                                LayerMask.GetMask("unit"));
        foreach(Collider2D colliderTarget in hitTarget){
            UnitAttribute target = colliderTarget.GetComponent<UnitAttribute>();
            if (target != null&&target.faction!=attr.faction)
            {      
                targetList.Add(target);
            }
        }
    }
    
    protected virtual void Attack()
    {
            if (ClosestTarget() != null)
            {
                if (!attr.isAction)
                {
                    AttackTarget(target);
                }//不在使用道具 就使用武器攻击
            }//有目标    
            else attr.isAttack = false;
    }//攻击行为
    protected virtual void AttackTarget(UnitAttribute target)
    {
        if (target == null) return;

        Transform targetTransform = target.transform;
        targetDistance = Vector3.Distance(target.transform.position,
            gameObject.transform.position);
        if(attr is InfantryAttribute inf&&inf.isAttack)
            transform.rotation = RotateHelper.RotateToUnit(transform, targetTransform,
                                                rotateSpeed);
        target.underAttack = true;//被攻击
        WeaponAttack();
    }//攻击逻辑
    protected virtual void WeaponAttack()
    {
        if (!attr.isAction)
        {
            attr.isAttack = true;
        }//不在行动状态
        if (weapon._isAttackCooling)
        {
            attr.isAttack = false;
        }//武器冷却时 停止攻击
        TowardsTarget(target);//朝向目标
        weapon.Attack(target, attr, targetDistance);//目标 攻击者 距离
    }
    protected virtual void TowardsTarget(UnitAttribute target)
    {
        if (target == null)
            return;

        float targetAngle=Mathf.Atan2(target.transform.position.y-transform.position.y,
                                        target.transform.position.x-transform.position.x)
                                        *Mathf.Rad2Deg;
        Quaternion Angle = Quaternion.Euler(0,0,targetAngle);
        if (Quaternion.Angle(transform.rotation, Angle) < 10)
        {
            isTowardsTarget = true;
        }
        else isTowardsTarget=false;
    }//朝向目标
    public UnitAttribute ClosestTarget()
    {
        GetTargetList();
        if(priorityTarget != null)
        {
            attr.SetUnitMovePos(priorityTarget.transform.position);//移动到优先目标
            foreach (UnitAttribute Target in targetList)
            {
                if (priorityTarget == Target)
                {
                    target = priorityTarget;
                    priorityTarget = null;
                    attr.SetMove();
                    targetDistance = Vector3.Distance(target.transform.position, gameObject.transform.position);
                    return target;
                }//优先目标在范围内 停止移动 取消优先目标
            }
        }//如果有优先目标
        if (target != null)
        {
            foreach (UnitAttribute Target in targetList)
            {
                if (target==Target)
                {
                    targetDistance = Vector3.Distance(target.transform.position, gameObject.transform.position);
                    return target;
                }
            }//目标还在攻击范围
            target = null;
        }//拥有目标 判断是否还在攻击范围
        if (target == null)
        {
            float minDistance = Mathf.Infinity;
            //最小距离              最大
            foreach (UnitAttribute target in targetList)
            {
                targetDistance = Vector3.Distance(target.transform.position, transform.position);
                if (targetDistance < minDistance)
                {

                    minDistance = targetDistance;
                    this.target = target;
                }
            }//寻找攻击范围内的最近目标
            return target;
        }//假如 没有/丢失 目标 寻找新目标
        return null;//没有目标
    }//选择目标
    public virtual void SetWeapon(GameObject newInstantiate)
    {
        Weapon newWeapon = newInstantiate.GetComponent<Weapon>();
        if(newWeapon == null)
        {
            Debug.Log("类型错误");
            return;
        }//没有武器组件
        else if (newWeapon.weaponName==weapon.weaponName)
        {
            Debug.Log("武器相同"); 
            return;
        }//相同武器
        Destroy(currentWeapon);
        weaponInstantiate = newInstantiate;
        ApplyWeapon();

    }//设置武器
    protected virtual void ApplyWeapon()
    {
        currentWeapon = Instantiate(weaponInstantiate, weaponParent);
        currentWeapon.transform.position = weaponParent.position;
        weapon=currentWeapon.GetComponent<Weapon>();
        weapon.SetOwner(this);
        attackRange = weapon.attackRange;
        attr.weaponIcon = weaponIcon;
    }//应用武器 
    public float _attackRange 
    {
        get { 
            return attackRange; 
        }

        set { 
            attackRange = value; 
        }
    }

}
