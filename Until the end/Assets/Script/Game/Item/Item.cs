using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Item : MonoBehaviour
{
    [SerializeField] protected ItemData itemData;
    public ItemData _itemData=>itemData;
    public ItemList ownerItemList;
    [SerializeField] protected string itemName;
    public string _itemName => itemName;
    [SerializeField] protected bool repeatable;//重复使用
    [SerializeField] protected bool requireTarget;//需要目标
    [SerializeField] protected Faction targetFaction;//目标阵营
    [SerializeField] protected float effectValue;//效果值
    [SerializeField] protected float useRange;//使用范围
    
    [SerializeField] protected float delay;//延迟
    [SerializeField] protected int useNumber;//重复激活次数
    [SerializeField] protected GameObject effectPrefab;//效果预制体 如果有

    public UnitAttribute target;
    public bool use;//使用完成
    public bool move;//需要移动

    private void Awake()
    {
        ApplyData();
    }
    public void ActiveItem(UnitBehavior owner)
    {       
        owner.StartCoroutine(owner.WaitForUseItem(useRange,targetFaction,this));
    }
    private void Use()
    {
        Debug.Log(ownerItemList.owner.attr.unitName + "使用" + itemName);
    }
    public IEnumerator UseItem(Vector3? pos=null)
    {
        while (move) 
        {
            float distance = Vector2.Distance(ownerItemList.owner.transform.position,
                                        target.transform.position);
            if(distance<=useRange)
                move=false;
            //到达可以使用的范围
            yield return null;
        }
        Debug.Log(itemName+"使用中");
        while (use==false)
        {
            if ((delay -= Time.deltaTime) <= 0)
            {
                Use();
                use = true;
                delay = itemData.delay;
            }//使用延迟
            yield return null;
        }
        use = false;
    }
    protected virtual void ApplyData() 
    {
        itemName = itemData.prefabName;
        repeatable = itemData.repeatable;
        requireTarget = itemData.requireTarget;

        effectValue= itemData.effectValue;
        useRange= itemData.useRange;
        delay = itemData.delay;
        useNumber = itemData.useNumber;
        effectPrefab = itemData.effectPrefab;
        targetFaction = itemData.targetFaction;
    }//应用数据
}//功能道具
