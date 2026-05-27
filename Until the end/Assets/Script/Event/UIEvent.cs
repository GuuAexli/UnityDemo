using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitButtonUI;

public class TextEvent 
{
    public string killerName;//击杀者
    public string victimName;//被害者

}

public static class UIEvent 
{
    //Game
    public static System.Action UpdateSupplyInfo;//更新补给变化
    public static System.Action UpdateInningInfo;//更新回合变化
    public static System.Action<Data> UpdateDescriptionInfo;//更新描述变化

    //Unit
    public static System.Action<UnitAttribute> OnUnitInfo;//显示单位UI
    public static System.Action<UnitAttribute> UnitOnDestroy;//单位被销毁隐藏面板（如果显示的是选择的单位）
    public static System.Action HideUnitInfo;//关闭单位UI 管理器单例取消选择||没有选择目标激活
    public static System.Action UpdateUnitInfo;//更新单位UI
    public static System.Action ShowWeaponInfo;
    public static System.Action HideWeaponInfo;

    //Button
    public static System.Action<UnitAttribute> UpdateButtonState;//更新按钮状态
    public static System.Action RemoveAllButtonListener;//移除全部按钮的监听
    public static System.Action<ActiveButtonType, UnitAttribute> OnActiveButton;//激活按钮
    public static System.Action RemoveAllButtonActive;//移除所以按钮的选择状态
    public static System.Action ResetButtonState;//重置选择按钮的状态

    //Text
    public static Action<UnitAttribute, UnitAttribute> OnUnitDied;
}
