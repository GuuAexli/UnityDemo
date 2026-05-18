using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FunctionItemList : MonoBehaviour
{
    [SerializeField]private List<FunctionItem> itemList = new List<FunctionItem>();
    public List<FunctionItem> _itemList => itemList;
    [SerializeField]private UnitAttribute owner;
    public UnitAttribute _owner => owner;
    public FunctionItem useItem=null;
    private void Start()
    {
        owner = GetComponent<UnitAttribute>();
        foreach(var item in itemList)
        {
            if (item != null)
            {
                item.ownerItemList = this;
            }
        }
    }
    public void UseItem(int value)
    {
        itemList[value].Active(owner);
    }//激活道具

    public void RemoveItem(FunctionItem removItem)
    {
        itemList.Remove(removItem);
    }
    public void AddItem(FunctionItem addItem)
    {
        for(int i = 0; i < 3; i++) 
        {
            if (itemList[i] == null)
            {
                itemList[i] = addItem;
                Debug.Log("正在装备"+addItem);
                return;
            }
        }
        Debug.Log("装备的道具达到上限");
        return;
    }//添加道具
    public void IsUseItem(FunctionItem item)
    {
        if (useItem != null)
        {
            if (useItem.myCoroutine != null)
            {
                useItem.isUse = false;
                owner._canAttack = true;
                useItem.StopMyCoroutine();
                Debug.Log("停止使用");
            }//之前使用的道具 有 执行协程 将它取消
        }//之前正在使用道具

        useItem = item;
        //设置现在使用的道具
    } 
    public int RemoveAllItem()
    {
        int cost=0;
        foreach(FunctionItem item in itemList)
        {
            cost+=item._itemData.costValue;
        }
        cost = Mathf.RoundToInt(cost*0.5f);//四舍五入
        itemList.Clear();
        Debug.Log("清空所有装备的道具");
        Debug.Log("返还费用一半 " + cost);
        return cost;
    }//移除所有道具
}//道具组
