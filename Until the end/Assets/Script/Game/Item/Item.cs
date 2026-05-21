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
    public float _useRange => useRange;
    [SerializeField] protected float delay;//延迟
    public float _delay=>delay;
    [SerializeField] protected int activeNumber;//重复激活次数
    [SerializeField] protected GameObject effectPrefab;//效果预制体 如果有

    public UnitAttribute target;
    public Vector3 targetPos;

    private void Awake()
    {
        ApplyData();
        
    }
    public void ActiveItem(UnitBehavior owner)
    {       
        owner.StartCoroutine(owner.WaitForUseItem(useRange,targetFaction,this));
    }
    public void Use()
    {
        Debug.Log(ownerItemList.owner.attr.unitName + "使用" + itemName);
        //Instantiate(effectPrefab);
    }
    protected virtual void ApplyData() 
    {
        itemName = itemData.prefabName;
        repeatable = itemData.repeatable;
        requireTarget = itemData.requireTarget;

        effectValue= itemData.effectValue;
        useRange= itemData.useRange;
        delay = itemData.delay;
        activeNumber = itemData.useNumber;
        effectPrefab = itemData.effectPrefab;
        targetFaction = itemData.targetFaction;
    }//应用数据
}//功能道具
