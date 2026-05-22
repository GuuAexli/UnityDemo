using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UnitButtonUI : MonoBehaviour
{
    public GameObject unitAttackBtn;
    public GameObject unitEnterBtn;
    public GameObject unitExitBtn;
    public List<Button> ItemBtn=new List<Button>();
    private ActiveButtonType currentButtonType = ActiveButtonType.None;

    public enum ActiveButtonType 
    {
        None,
        Attack,
        Enter, 
        Exit
    }
    private void OnEnable()
    {
        UIEvent.UpdateButtonState += ButtonState;
        UIEvent.RemoveAllButtonActive += RemoveAllButtonActive;
        UIEvent.RemoveAllButtonListener += RemoveAllListener;
        UIEvent.ResetButtonState += ResetButtonType;
    }
    private void OnDestroy()
    {
        RemoveAllListener();
        UIEvent.RemoveAllButtonActive -= RemoveAllButtonActive;
        UIEvent.RemoveAllButtonListener -= RemoveAllListener;
        UIEvent.ResetButtonState -= ResetButtonType;
        UIEvent.UpdateButtonState -= ButtonState;
    }
    public void ButtonState(UnitAttribute unit)
    {
        RemoveButtonHide();
        RemoveAllListener();

       AttackButton(unit);
       EnterButton(unit);
       ExitButton(unit);
       ItemButton(unit);
    }
    
    public void AttackButton(UnitAttribute unit)
    {
        if (unit.faction == Faction.Blue && GameController.Instance.canCommandValue > 0)
        {
            unitAttackBtn.SetActive(true);
            unitAttackBtn.GetComponent<Button>()?.
                onClick.AddListener(() =>
                {
                    ActiveButton(ActiveButtonType.Attack, unit);
                });
        }//单位是友军&&存在指挥单位
        else return;
    }//显示攻击按钮并监听事件
    public void EnterButton(UnitAttribute unit)
    {
        if (unit.tag != "red_tag" && unit._canEnterObject||unit.TryGetComponent<ILoadUnit>(out var transport))//可以装载的单位 运输的载具
        {
            unitEnterBtn.SetActive(true);
            unitEnterBtn.GetComponent<Button>()?.
                onClick.AddListener(() => 
                {
                    ActiveButton(ActiveButtonType.Enter, unit);
                });

            return;
        }
    }//进入按钮
    public void ExitButton(UnitAttribute unit)
    {
        if (unit.tag != "red_tag" && unit.TryGetComponent<ILoadUnit>(out var transport))//运输的载具
        {
            unitExitBtn.SetActive(true);
            unitExitBtn.GetComponent<Button>()?.onClick.AddListener(() =>
            {
                ActiveButton(ActiveButtonType.Exit, unit);
            });
        }
    }//离开按钮
    public void ItemButton(UnitAttribute unit)
    {
        ItemList itemList=unit.GetComponent<ItemList>();
        if (itemList != null)
        {
            for (int a = 0; a < itemList._itemList.Count; a++)
            {
                if (a == 3) return;
                
                int index = a;
                //c#闭包捕获问题
                if (itemList._itemList[index] != null)
                {                    
                    ItemBtn[index].gameObject.SetActive(true);
                    ItemBtn[index].onClick?.AddListener(() => itemList.UseItem(index));
                    
                    Item item= itemList._itemList[index];
                    Text itemText = ItemBtn[index].GetComponentInChildren<Text>();
                    itemText.text = item._itemData.prefabName;
                    if (item.isCooling)
                    {
                        itemText.text = $"{ item.cooling.ToString("F2") }";
                    }
                    else
                        itemText.text=item._itemData.prefabName;
                }//显示并监听对应位置的按钮
            }//检查道具组
        }//获取单位的道具组

    }//功能道具按钮
    private void ActiveButton(ActiveButtonType buttonType,UnitAttribute unit)
    {
        if (currentButtonType == buttonType)
        {
            RemoveAllButtonActive();//关闭全部按钮选择状态
            currentButtonType = ActiveButtonType.None;//重置选择按钮Type
            Debug.Log("取消相同选择");
        }//是之前的选择 重置 选择按钮Type
        else
        {
            RemoveAllButtonActive();
            currentButtonType = buttonType;
            //Debug.Log("清除所有选择");

            switch (currentButtonType) 
            {

                case ActiveButtonType.Attack:
                    UIEvent.OnActiveButton?.Invoke(currentButtonType,unit);
                    //Debug.Log("攻击选择");
                    break;
                case ActiveButtonType.Enter:
                    UIEvent.OnActiveButton?.Invoke(currentButtonType, unit);
                    //Debug.Log("进入选择");
                    break;    
                case ActiveButtonType.Exit:
                    UIEvent.OnActiveButton?.Invoke(currentButtonType, unit);
                    //Debug.Log("离开选择");
                    break;
            
            }

        }
    }//判断激活的按钮
    public void RemoveAllButtonActive()
    {
        GetComponent<CommandController>().isSelectedButton = false;
        //GetComponent<LoadUnitController>().isSelectedButton = false;
    }//移除所有的按钮激活
    public void RemoveButtonHide()
    {
        unitAttackBtn.SetActive(false);
        unitEnterBtn.SetActive(false);
        unitExitBtn.SetActive(false);
        foreach(var Btn in ItemBtn)
        {
            Btn.gameObject.SetActive(false);
        }

    }//移除按钮显示
    public void RemoveAllListener()
   
    {
        unitAttackBtn?.GetComponent<Button>().onClick.RemoveAllListeners();
        unitEnterBtn?.GetComponent<Button>().onClick.RemoveAllListeners();
        unitExitBtn?.GetComponent<Button>().onClick.RemoveAllListeners();
        foreach(var Btn in ItemBtn)
        {
            Btn.onClick?.RemoveAllListeners();
        }
    } //移除 按钮 监听
    public void ResetButtonType()
    {
        currentButtonType = ActiveButtonType.None;
    }//完成后重置按钮选择状态
}
