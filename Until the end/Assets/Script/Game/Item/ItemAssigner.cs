using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssigner : MonoBehaviour
{
    Item item;

    private void Start()
    {
        item = GetComponent<Item>();
        if (item == null) { Destroy(gameObject); return; }

        if(item.ownerItemList == null) 
        {
            UIEvent.OnMessageText?.Invoke("选择单位装备"+item._itemData.prefabName);
            return;
        }

    }
    private void Update()
    {
        SelectorUnit();
    }
    public void SelectorUnit()
    {
        if (item.ownerItemList == null)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pos;

            if (Input.GetMouseButtonDown(0)) 
            {
                Collider2D col = Physics2D.OverlapPoint(pos, LayerMask.GetMask("unit"));
                if (col == null) 
                { 
                    UIEvent.OnMessageText?.Invoke("没有目标"); 
                    Destroy(gameObject);
                    GameController.Instance.setCost(item._itemData.costValue);
                    return;
                }//没有目标
                    ItemList list=col.GetComponent<ItemList>();
                
                if (list == null) 
                { 
                    UIEvent.OnMessageText?.Invoke("目标没有道具组件");
                    Destroy(gameObject);
                    GameController.Instance.setCost(item._itemData.costValue);
                    return ; 
                }//缺少组件

                list.AddItem(item);
                UIEvent.OnMessageText?.Invoke(list.owner.attr.unitName+"装备"+item._itemData.prefabName);
            }
            if (Input.GetMouseButtonDown(1))
            { 
                UIEvent.OnMessageText?.Invoke("取消选择装备");
                GameController.Instance.setCost(item._itemData.costValue);
                Destroy(gameObject);
            }//取消
        }
        else
        {
            this.enabled = false;
        }
    }

}
