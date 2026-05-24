using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantItem : Item
{
    public bool activeItem=false;

    public void ChangeActive()
    {
        if (activeItem == false)
        {
            activeItem = true;
            ownerItemList.owner.StartCoroutine(ownerItemList.owner.AssistantItemBehavior(this));
            Debug.Log("激活"+itemName);
        }
        else
        {
            activeItem = false;
            Debug.Log("取消激活"+itemName);
        }
    }
    public override void ActiveItem(UnitBehavior owner)
    {
        ChangeActive();
    }
    public override void Use()
    {
        target.AddHealth(effectValue);
    }
}//辅助道具(如果激活 自动寻找目标)
