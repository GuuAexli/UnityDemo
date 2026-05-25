

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAttribute : MonoBehaviour,ITakeDamage
{
    [Header("数据")]
    public UnitData unitData;//单位数据
    [Header("组件")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Collider2D col;
    public GameObject posture;//临时表现姿态（白色站立，黄色匍匐，红色被压制）
    [Header("单位信息")]
    public string unitName;//单位名字
    public Sprite unitIcon;//单位图片
    public Sprite weaponIcon;//武器图片
    public Faction faction;//派系/阵营
    [Header("单位属性")]
    [SerializeField]public float maxHealth;//最大生命值
    public float health;//现在健康值
    public float moveSpeed;//移动速度
    public float rotateSpeed;//单位旋转速度
    public float fear { get; protected set; }//恐惧值
    public float decayFear{ get; protected set; }//恐惧值衰减
    [Header("战斗属性")]
    [SerializeField] protected float unitVolume;//单位体积
    public float _unitVolume => unitVolume;
    public float actualUnitVolume;

    protected int unitArmor;//单位装甲
    public int _unitArmor => unitArmor;

    [Header("管理")]
    public bool isSelected;//正在选中
    [Header("移动组件")]   
    [SerializeField] protected UnitNavMove unitNavMove;
    [SerializeField] protected bool canMove = true;//允许移动
    [SerializeField] protected bool isMove;
    public float moveEfficiency { get; private set; }//效率（移动，旋转）
    public bool _isMove=>isMove;
    public UnitNavMove _unitNavMove => unitNavMove;

    [Header("战斗组件")]
    [SerializeField] protected UnitCombat unitCombat;
    [SerializeField] protected float unitAccurracy;

    
    [SerializeField] protected bool canAttack = true;//允许攻击
    
    public bool isAttack;//正在攻击
    public bool underAttack;//正在被攻击
    public Transform itemPos;
    public UnitCombat _unitCombat => unitCombat;
    public float _unitAccurracy => unitAccurracy;
    public float combatEfficiency { get; private set; }//效率（整体命中率）

    public bool isAction;//正在行动
    [SerializeField] protected bool isUseItem = false;//正在使用道具
    [SerializeField] private bool canEnterObject;//可以进入 模型/载具   

    

    public bool _canEnterObject => canEnterObject;
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


    //public float targetAngle;//敌人角度


    protected Dictionary<VolumeFactorType, float> volumeFactor=new Dictionary<VolumeFactorType, float>();
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        unitCombat = GetComponent<UnitCombat>();
        unitNavMove = GetComponent<UnitNavMove>();

        AwakeUnitData();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        if (faction == Faction.Blue)
        {
            if (ColorUtility.TryParseHtmlString("#5A99FF", out Color color))
                sr.color = color;
        }
        else 
        {
            if (ColorUtility.TryParseHtmlString("#FF4545", out Color color))
                sr.color= color;
        }
    }
    private void Start()
    {
        ApplyLevelData();
        health = maxHealth;
        moveEfficiency = 1;
        combatEfficiency = 1;
    }
    public virtual void ApplyLevelData() 
    {
        UnitLevelData.LevelStats levelData = unitData.level.levels[unitLevel];//应用现在等级属性

        maxHealth = levelData.health;//设置生命
        moveSpeed = levelData.moveSpeed;//速度
        rotateSpeed = levelData.rotationSpeed;//旋转速度
        unitVolume = levelData.volum;//体积
        RecalculateActualVolume();
        unitArmor = levelData.armor;//装甲
        decayFear = levelData.decayFear;//恐惧衰减

        nextLevelExp = levelData.nextLevelExp;//下一次升级需要的经验
        UnitCombat accurracy = gameObject.GetComponent<UnitCombat>();
        unitAccurracy = levelData.accurracy;//设置单位精准度

    }    //应用等级状态


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
        unitNavMove.SetMovePos(pos,true);
        //移动到目标位置 = 接收的值
    }//移动到目标位置 接收 Vector2值
    public void AwakeUnitData()
    {
        unitName = unitData.prefabName;
        unitIcon = unitData.unitIcon;
        isVehicle = unitData.isVehicle;
        canEnterObject = unitData.canEnterObject;
        faction = unitData.unitFaction;
        maxLevel =unitData.level.levels.Length-1;
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
    public void SetMove(bool value=false)
    {
        isMove = value;
    }
    public void SetVolumeFactor(VolumeFactorType type,float value)
    {
        volumeFactor[type]=value;
        RecalculateActualVolume();
    }//设置因子值
    public void RemoveVolumeFactor(VolumeFactorType type)
    {
        volumeFactor.Remove(type);
        RecalculateActualVolume();
    }
    public void RecalculateActualVolume()
    {
        actualUnitVolume = unitVolume;
        foreach(var factor in volumeFactor) 
        {
            actualUnitVolume *= factor.Value;
        }
        if (actualUnitVolume <= 0.1f) 
        {
            actualUnitVolume=0.1f;
        }
    }//重新计算体积
    public void SetMoveEfficiency(float value = 1)
    {
        moveEfficiency=value;
    }//设置移动效率
    public void SetCombatEfficiency(float value = 1)
    {
        combatEfficiency=value;
    }//设置战斗效率
    public bool _canMove { get => canMove; set => canMove = value; }
    public bool _canAttack { get => canAttack; set => canAttack = value; }


}


