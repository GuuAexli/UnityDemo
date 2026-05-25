using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssigner : MonoBehaviour
{
    public Weapon weapon;

    public void OnSelector(GameObject target)
    {
        UnitCombat combat = target.GetComponent<UnitCombat>();
        if (combat == null||combat.attr.faction!=Faction.Blue) { Debug.Log("没有合适单位"); return; }

        combat.SetWeapon(gameObject);
    }
    void Start()
    {
        weapon = gameObject.GetComponent<Weapon>();
        //if (weapon.owner != null) enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon.owner != null) { enabled = false; return; }
        //如果武器拥有所有者   关闭现在这个物体上的这个负责移动选择单位的脚本
        selsetOwener();


    }
    void selsetOwener()
    {
        
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
        if(Input.GetMouseButtonDown(0)) {
            //如果按下左键
            Collider2D hit = Physics2D.OverlapPoint(pos,LayerMask.GetMask("unit"));
            //发射射线 进行碰撞 来 获取被碰撞体信息
            if (hit!= null)
                //有碰撞到物体
            {
                UnitCombat combatWeapon = hit.GetComponent<UnitCombat>();
                //获取碰撞物上需要的脚本 
                if (combatWeapon != null)
                    //确认是要求的碰撞体
                {
                    combatWeapon.SetWeapon(gameObject);
                    //更改单位武器
                }
            }
            else Debug.Log("没有目标");

            Destroy(gameObject);
            //点击事件结束最后删除模型
        }

    }
}
