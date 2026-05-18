

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAttribute : MonoBehaviour,ITakeDamage
{
    [Header("数据")]
    public UnitData unitData;//单位数据


    [Header("组件")]
    [SerializeField] protected Rigidbody2D _rd;
    [SerializeField] protected Collider2D _cd;
    //[SerializeField] public UnitMove unitMove;//移动脚本
    [SerializeField] protected UnitNavMove unitNavMove;
    [SerializeField] protected UnitCombat unitCombat;
    public UnitCombat _unitCombat => unitCombat;
    public UnitNavMove _unitNavMove => unitNavMove;


    [Header("单位状态")]
    [SerializeField] protected bool canMove = true;//允许移动
    [SerializeField] protected float moveEfficiency = 1;//效率（移动，旋转）
    [SerializeField] protected bool canAttack = true;//允许攻击
    [SerializeField] protected float attackEfficiency = 1;//效率（整体命中率）
    [SerializeField] protected bool isUseItem = false;//正在使用道具
    [SerializeField] private bool canEnterObject;//可以进入 模型/载具   


    [Header("控制")]
    public bool isSelected;//正在选中
    public bool isAttack;//正在攻击
    public bool underAttack;//正在被攻击
    public bool isMove;
    public bool isAction;//正在行动

    public bool _canEnterObject => canEnterObject;
    public bool _canAttack { get { return canAttack; } set { canAttack = value; } }
    public bool _isUseItem { get { return isUseItem; }set { isUseItem = value; } }


    [Header("等级属性")]
    [SerializeField]protected int unitLevel = 0;//单位等级
    [SerializeField] protected int nextLevelExp;//升级需要经验
    [SerializeField] protected int unitExp; //单位经验
    [SerializeField] protected int maxLevel;//最大等级
    public int _unitLevel => unitLevel;
    public int _unitExp=>unitExp;
    public int _nextLevelExp => nextLevelExp;
    public int _maxLevel => maxLevel;


    [Header("单位种类")]
    public bool isVehicle;//是载具


    [Header("单位信息")]
    public string unitName;//单位名字
    public Sprite unitIcon;//单位图片
    public Sprite weaponIcon;//武器图片


    [Header("单位属性")]
    public float maxHealth;//最大生命值
    public float health;//现在健康值
    public float moveSpeed;//移动速度
    public float rotateSpeed;//单位旋转速度


    [Header("战斗属性")]
    [SerializeField]protected float unitVolume;//单位体积
    public float _unitVolume => unitVolume;
    public float actualUnitVolume;

    protected int unitArmor;//单位装甲
    public int _unitArmor=>unitArmor;
    //public float targetAngle;//敌人角度
    protected virtual void Awake()
    {
        _rd = GetComponent<Rigidbody2D>();
        _cd = GetComponent<Collider2D>();

        unitCombat = GetComponent<UnitCombat>();
        unitNavMove = GetComponent<UnitNavMove>();

        AwakeUnitData();
    }

    public abstract void ApplyLevelData();

    public void UnitSelected(bool stats)
    //选择单位 接收 bool值
    {
        if (gameObject.tag != "red_tag")
            //这个模型的标签不是red_tag
            isSelected = stats;
    }
    public void SetUnitMovePos(Vector2 pos)
    {
        //unitMove.movePos = pos;
        unitNavMove.SetMovePos(pos);
        //移动到目标位置 = 接收的值
    }//移动到目标位置 接收 Vector2值
    public void AwakeUnitData()
    {
        unitName = unitData.prefabName;
        unitIcon = unitData.unitIcon;
        isVehicle = unitData.isVehicle;
        canEnterObject = unitData.canEnterObject;
    }//初始化单位数据

    public void OnDestroy() 
    {
        if (GameController.Instance != null && GameController.Instance.selectedUnit == this)
            GameController.Instance.ClearSelection();
        LineEvent.HideDestroyUnitEvent?.Invoke(this);
        UIEvent.UnitOnDestroy?.Invoke(this);
    }//销毁时 如果 游戏控制器 选择 这个单位 将它取消
    public void ApplyDamage(float damage,UnitAttribute atkUnit = null)
    {
        if ((health -= damage) <= 0)
        {
            Destroy(gameObject);
            if (atkUnit != null)
                Debug.Log(unitName + "死亡\t" + "击杀者：" + atkUnit.unitName);
        }
        if (UnitInfoUI.Instance.target == this)
        {
            UIEvent.UpdateUnitInfo?.Invoke();
        }//更新单位UI面板状态
    }//造成伤害
    public bool TakeDamage(DamageInfo info)
    {
        float probability=info.penetration/unitArmor;
        //击穿率
        bool isPenetration = (probability >= 1f || 
            (probability >= 0.3f) && Random.Range(0f, 1f) >= probability);

        if (isPenetration)
        {
            float damage =(probability>=1f)? info.damage: info.damage * probability;
            ApplyDamage(damage, info.atkUnit);
        }
        return isPenetration;
     
    }//伤害接口
    public void AddHealth(float value)
    {
        health += value;
        if (health > maxHealth) 
            health = maxHealth;

        if (UnitInfoUI.Instance.target == this)
        {
            UIEvent.UpdateUnitInfo?.Invoke();
        }
    }//回复健康值
    public void AddExp(int exp)
    {
        unitExp += exp;
        if (unitExp >= nextLevelExp && unitLevel < maxLevel) {

            unitLevel++;
            ApplyLevelData();
        }
    }//添加经验 

}


