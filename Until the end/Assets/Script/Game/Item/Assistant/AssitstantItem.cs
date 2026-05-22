using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssitstantItem : Item
{
    public override void Use()
    {
        Debug.Log("这是辅助道具不能主动激活");
    }
}
