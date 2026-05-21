using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryAttribute : UnitAttribute,IFear
{
    [SerializeField] private bool canMoveAttack;//可以移动攻击
   

    private void Start()
    {
        ApplyLevelData();//应用等级状态
        maxLevel = unitData.level.levels.Length - 1;
        health = maxHealth;

        //UnitStateMachine = new InfantryStateMachine();
        //UnitMoveMachine = new InfantryMoveMachine();
        //获取状态机
        //UnitStateMachine.unit = this;
        //UnitStateMachine?.Awake();
        //UnitMoveMachine.OnAwake();
    }
    private void Update()
    {        
        ReduceFear();
    }
    public override void ApplyLevelData()
    {
        UnitLevelData.LevelStats levelData = unitData.level.levels[unitLevel];//应用现在等级属性

        maxHealth = levelData.health;//设置生命
        moveSpeed = levelData.moveSpeed;//速度
        rotateSpeed = levelData.rotationSpeed;//旋转速度
        unitVolume = levelData.volum;//体积
        actualUnitVolume = unitVolume;//实际体积
        unitArmor = levelData.armor;//装甲
        decayFear = levelData.decayFear;//恐惧衰减
        nextLevelExp = levelData.nextLevelExp;

        

        //UnitStateMachine.currentState.Enter(this);
        //升级可能提升速度导致实际速度重置 恐惧和压制在进入时更改实际速度

        nextLevelExp = levelData.nextLevelExp;//下一次升级需要的经验

        UnitCombat accurracy = gameObject.GetComponent<UnitCombat>();
        accurracy._unitAccurracy = levelData.accurracy;//设置单位精准度

    }    //应用等级状态
    public void AddFear(float value)
    {
        fear += value;
        if (fear >= 120)
            fear = 120;
        UIEvent.UpdateUnitInfo?.Invoke();
    }
    public void ReduceFear()
    {
        fear -= decayFear*Time.deltaTime;
        if(fear<=0)
            fear = 0;
        UIEvent.UpdateUnitInfo?.Invoke();
    }
    public float _fear
    {
        get { return fear; }
        set { fear = Mathf.Clamp(value, 0, 200); }
    }
}
