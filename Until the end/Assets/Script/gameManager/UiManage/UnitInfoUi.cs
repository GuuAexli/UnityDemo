using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
//          单位信息UI
{
    public static UnitInfoUI Instance;
    public UnitAttribute target;

    public GameObject unitInfoPanel;//单位数据面板
    public GameObject weaponInfoPanel;//武器数据面板
    //单例模式
    [Header("单位")]
    public Image unitImage;//单位图片   
    public Text unitName;//单位名字
    public Text unitVolume;//单位体积
    public Text unitArmor;//单位装甲
    public Text unitAccurracy;//单位命中率
    public Slider unitHealthSlider;//单位生命条
    public Text unitHealth;//单位生命
    public Slider unitExpSlider;//经验条
    public Text unitLevel;//单位等级
    
    [Header("武器")]
    public Button weaponInfoButton;
    public Button weaponHideInfoButton;
    
    //属性
    public Text weaponName;//武器名
    public Text weaponBullet;//子弹类型
    public Text weaponRange;//攻击范围
    public Text weaponSuppression;//压制力
    public Text weaponDuffusion;//散布
    //命中率
    public Text closeAccurracy;//近距离命中率
    public Text mediumAccurracy;//中距离命中率
    public Text LongAccurracy;//远距离命中率
    //时间
    public Text weaponDuration;//持续时间
    public Text weaponAimTime;//瞄准时间
    public Text weaponInterval;//间隔    
    public Text weaponCooling;//冷却时间   
    //倍率
    public Text damageMultiplier;//伤害倍率
    public Text penrtrationMultiplier;//穿甲倍率
    
  
    
   

    private void Awake() 
    { 
        UnitInfoUI.Instance = this;
        HideUnitInfo();
        weaponInfoButton.onClick?.AddListener(() => ShowWeaponInfo());
        weaponHideInfoButton.onClick?.AddListener(() => HideWeaponInfo());

    }
    //绑定 单位信息UI 单例


    private void OnEnable()
    {
        UIEvent.OnUnitInfo += TargetUnit;
        UIEvent.UpdateUnitInfo+=ShowUnitInfo;
        UIEvent.HideUnitInfo += HideUnitInfo;
        UIEvent.UnitOnDestroy += UnitOnDestroy;
    }
    private void OnDisable()
    {
        UIEvent.OnUnitInfo -= TargetUnit;
        UIEvent.UpdateUnitInfo -= ShowUnitInfo;
        UIEvent.HideUnitInfo -= HideUnitInfo;
        UIEvent.UnitOnDestroy -= UnitOnDestroy;
    }
    public void TargetUnit(UnitAttribute unit)
    //由 游戏管理 来赋值   GameController.HandleSelection()
    {    
        if (unit != null)
        {
            target = unit;
            GetComponent<UnitButtonUI>().ButtonState(unit);
            UIEvent.UpdateUnitInfo?.Invoke();
            unitInfoPanel.SetActive(true);
        }
    }
    public void ShowUnitInfo()
    //显示/更新 单位数据      需要 target
    {
        if (target == null) return;//需要有显示的单位  单位受击会调用 但需要是选择的单位

        UIEvent.UpdateButtonState?.Invoke(target);//更新单位按钮状态
        unitImage.sprite = target.unitIcon;//获取单位图片
        unitName.text = target.unitName;//获取单位名字
        unitHealth.text ="健康值："+$"{ target.health}/{ target.maxHealth}";
        //获取单位健康值    单位 现在生命  /  单位最大生命
        unitLevel.text ="等级："+ $"{target._unitLevel}/{target._maxLevel}";
        unitVolume.text = "体积：" + target._unitVolume;
        unitArmor.text = "装甲：" + target._unitArmor;
        unitAccurracy.text = "单位命中率：" + target._unitCombat._unitAccurracy;

        if(unitHealthSlider != null ) 
        {
            //单位生命条存在
            unitHealthSlider.maxValue = target.maxHealth;
            //获取 滑条最大值
            unitHealthSlider.value = target.health;
            //获取 显示的现在生命的值
        }//生命条
        if (unitExpSlider != null)
        {
            unitExpSlider.maxValue = target._nextLevelExp;
            unitExpSlider.value = target._unitExp;
        }//经验条
        //infoPanel.SetActive(true);
        //信息面板 设置活动状态 

    }
    public void ShowWeaponInfo()
    {
        if (target == null || weaponInfoButton == null) return;
        if (weaponInfoPanel == null) { Debug.Log("武器面板错误"); return; }
        if (weaponInfoPanel.activeInHierarchy) { HideWeaponInfo();return; }
        weaponInfoPanel.SetActive(true);
        ApplyWeaponInfo();
    }
    public void HideWeaponInfo()
    {
        weaponInfoPanel.SetActive(false);
    }
    public void ApplyWeaponInfo()
    {
        if (target._unitCombat == null) { Debug.Log("目标缺少战斗模块！");return; }
        Weapon data=target._unitCombat.weapon;
        if(data==null) { Debug.Log("目标没有武器！");return; }

        Bullet bullet=data.bullet.GetComponent<Bullet>();

        weaponName.text=data.weaponName;
        weaponBullet.text ="子弹类型："+ bullet._bulletData.prefabName;
        weaponRange.text ="范围：" + data.attackRange;
        weaponSuppression.text = "压制力：" + data.suppressionValue;
        weaponDuffusion.text = "散布：" + data.duffusion;

        closeAccurracy.text = "近距离：" + data.closeRangeAccurracy;
        mediumAccurracy.text = "中距离：" + data.mediumRangeAccurracy;
        LongAccurracy.text = "远距离：" + data.longRangeAccurracy;

        weaponDuration.text = "攻击持续时间：" + data.attackDuration;
        weaponAimTime.text="瞄准时间："+data.aimTime;
        weaponInterval.text = "攻击间隔：" + data.attackInterval;
        weaponCooling.text = "攻击冷却时间：" + data.attackCooling;

        damageMultiplier.text = "伤害倍率：" + data.damageMultiplier;
        penrtrationMultiplier.text = "穿甲倍率：" + data.penetrationMultiplier;
    }
    public void UnitOnDestroy(UnitAttribute unit)
    {
        if (unit == target)
            HideUnitInfo();
    }
    public void HideUnitInfo()
    {
        target = null;
        UIEvent.RemoveAllButtonListener?.Invoke();//移除激活的按钮
        unitInfoPanel.SetActive(false);
        HideWeaponInfo();
        //      信息面板 设置活动状态
        // owner = null; 
    }//关闭信息

}
