using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="ItemData",menuName ="ItemData")]
public class ItemData : Data
{
    public bool repeatable;//可以重复
    public bool requireTarget;//需要目标
    
    public Faction targetFaction;//目标标签
    public float effectValue;//效果值(非爆炸类效果值)
    public float useRange;//使用范围
    public float delay;//延迟
    public float cooling;//冷却
    public int useNumber;//重复激活次数
    public GameObject effectPrefab;//效果 范围效果预制体


    public float searchRange;//寻找范围（辅助道具使用）

    public override void Spawn()
    {
        Instantiate(prefab);
    }
}

