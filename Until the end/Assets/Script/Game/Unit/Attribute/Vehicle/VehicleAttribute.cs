using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleAttribute : UnitAttribute
{
    public VehicleStateMachine UnitStateMachine { get; protected set; }
    //状态机

    [SerializeField] private bool canLoad;//可以装载
    public bool _canLoad => canLoad;

    private void Start()
    {
        isVehicle = unitData.isVehicle;

        ApplyLevelData();
        //weaponIcon = gameObject.GetComponent<UnitCombat>().weaponIcon;
        maxLevel = unitData.level.levels.Length - 1;
        health = maxHealth;
    }
    private void Update()
    {
        //unitMove.Move(this,moveSpeed);
    }
    public override void ApplyLevelData()
    {
        UnitLevelData.LevelStats levelData = unitData.level.levels[unitLevel];//应用现在等级属性

        maxHealth = levelData.health;//设置生命
        //unitNavMove.ApplyData(levelData.moveSpeed, levelData.rotationSpeed);
        moveSpeed = levelData.moveSpeed;//速度
        rotateSpeed = levelData.rotationSpeed;//旋转速度
        unitVolume = levelData.volum;//体积
        actualUnitVolume = unitVolume;//实际体积
        unitArmor = levelData.armor;//装甲
        nextLevelExp = levelData.nextLevelExp;
        //unitMove.ApplySpeedStats(this);
        nextLevelExp = levelData.nextLevelExp;//下一次升级需要的经验

        UnitCombat accurracy = gameObject.GetComponent<UnitCombat>();
        accurracy._unitAccurracy = levelData.accurracy;//设置单位精准度

    }    //应用等级状态
}

