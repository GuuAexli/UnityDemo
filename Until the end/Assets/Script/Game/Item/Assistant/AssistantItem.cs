using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AssistantItem : Item
{
    public bool activeItem=false;
    public Coroutine coroutine;

    public void ChangeActive()
    {
        if (activeItem == false)
        {
            activeItem = true;
            if(coroutine != null) 
            {
                StopCoroutine(coroutine);
                Debug.Log("取消之前激活的辅助道具" );
            }//如果之前有 取消 之前的协程
            coroutine=ownerItemList.owner.StartCoroutine(ownerItemList.owner.AssistantItemBehavior(this));
            Debug.Log("激活"+itemName);
        }
        else
        {
            activeItem = false;{ }
            { 
                ownerItemList.owner.StopCoroutine(coroutine);
                Debug.Log("取消自动行为");
                ownerItemList.owner.ClearAssistantItemBehavior();
            }
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
