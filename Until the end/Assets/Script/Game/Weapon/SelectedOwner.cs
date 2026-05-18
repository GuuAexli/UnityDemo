using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedOwner : MonoBehaviour
{
    public GameObject weaponPrefad;
    public Weapon weapon;

    void Start()
    {
        weaponPrefad = this.gameObject;
        weapon = weaponPrefad.GetComponent<Weapon>();
        if (weapon.owner != null) enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon.owner != null) enabled = false;
        //如果武器拥有所有者   关闭现在这个物体上的这个负责移动选择单位的脚本
        selsetOwener();
    }
    void selsetOwener()
    {
        
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
        if(Input.GetMouseButtonDown(0)) {
            //如果按下左键
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            //发射射线 进行碰撞 来 获取被碰撞体信息
            if (hit.collider != null)
                //有碰撞到物体
            {
                UnitCombat combatWeapon = hit.collider.GetComponent<UnitCombat>();
                //获取碰撞物上需要的脚本 
                if (combatWeapon != null)
                    //确认是要求的碰撞体
                {
                    combatWeapon.SetWeapon(weaponPrefad);
                    //更改单位武器
                }
            }
            else Debug.Log("没有目标");

            Destroy(gameObject);
            //点击事件结束最后删除模型
        }

    }
}
