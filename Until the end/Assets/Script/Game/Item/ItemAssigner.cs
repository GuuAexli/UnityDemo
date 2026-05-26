using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssigner : MonoBehaviour
{
    Item item;

    private void Start()
    {
        item = GetComponent<Item>();
        if(item==null)
            Destroy(gameObject);
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
                if (col == null) {Debug.Log("目标没有道具组件"); Destroy(gameObject); return;}
                    ItemList list=col.GetComponent<ItemList>();
                
                if (list == null) { Debug.Log("目标没有道具组件");Destroy(gameObject);return ; }

                list.AddItem(item);
            } 
        }
        else
        {
            this.enabled = false;
        }
    }

}
