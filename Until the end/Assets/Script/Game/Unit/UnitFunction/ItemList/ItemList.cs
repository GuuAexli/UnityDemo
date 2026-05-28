using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class ItemList : MonoBehaviour
{
    [SerializeField]private List<Item> itemList = new List<Item>();
    public List<Item> _itemList => itemList;
    public UnitBehavior owner{ get; private set; }
    public Item useItem=null;
    private void Start()
    {
        owner = GetComponent<UnitBehavior>();
        for (int i = 0; i < 3; i++)
            itemList.Add(null);//初始化防止越界
    }

    public void UseItem(int i)
    {
        itemList[i].ActiveItem(owner);
    }//激活道具

    public void RemoveItem(Item removItem)
    {
        itemList.Remove(removItem);
    }//移除道具
    public void AddItem(Item addItem)
    {

        
        for(int i = 0; i < 3; i++) 
        {
            if (itemList[i] == null)
            {
                itemList[i] = addItem;
                addItem.transform.SetParent(owner.attr.itemPos);
                addItem.transform.position = owner.attr.itemPos.position;
                addItem.transform.rotation = owner.attr.itemPos.rotation;
                addItem.ownerItemList = this;
                return;
            }//寻找空位
        }
        UIEvent.OnMessageText?.Invoke("装备的道具达到上限");
        Destroy(addItem);
        return;
    }//添加道具
    public int RemoveAllItem()
    {
        int cost=0;
        foreach(var prefab in itemList)
        {
            Item item=prefab.GetComponent<Item>();
            cost+=item._itemData.costValue;
        }
        cost = Mathf.RoundToInt(cost*0.5f);//四舍五入
        itemList.Clear();
        Debug.Log("清空所有装备的道具");
        Debug.Log("返还费用一半 " + cost);
        return cost;
    }//移除所有道具
}//道具组
