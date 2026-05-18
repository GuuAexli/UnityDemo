using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class FunctionItem : MonoBehaviour
{
    [SerializeField] protected ItemData itemData;
    public ItemData _itemData=>itemData;
    public FunctionItemList ownerItemList;

    [SerializeField] protected string itemName;
    public string _itemName => itemName;
    [SerializeField] protected bool repeatable;//重复使用
    [SerializeField] protected bool requireTarget;//需要目标
    [SerializeField] protected string targetTag;//目标标签
    [SerializeField] protected float effectValue;//效果值
    [SerializeField] protected float useRange;//使用范围
    [SerializeField] protected float delay;//延迟
    [SerializeField] protected int useNumber;//重复使用次数
    [SerializeField] protected GameObject effectPrefab;//效果预制体 如果有

    public bool isUse;//正在使用
    public Coroutine myCoroutine;
    //public UnitAttribute target;
    private void Awake()
    {
         
    }
    private void Start()
    {
        ApplyData();
        //ownerItemList = target.GetComponent<FunctionItemList>();
    }
    public void SelectedOwner(UnitAttribute unit)
    {
        FunctionItemList unitItemList = unit.GetComponent<FunctionItemList>();
        if (unitItemList!= null)
        {
            Debug.Log(unit + "正在装备" + itemName);
            GameObject newItem=Instantiate(gameObject,unit.transform);
            newItem.transform.SetParent(unit.transform);
            unitItemList.AddItem(this);
            ownerItemList = unitItemList;
            UIEvent.UpdateUnitInfo?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("目标无法装备道具");
            Destroy(gameObject);
        }
    }//选择获取所有者的道具组
    public void Active(UnitAttribute useUnit)
    {
        Debug.Log(this);
        ownerItemList.IsUseItem(this);
        Debug.Log("正在使用" + itemName);
        isUse = true;
       
        myCoroutine =requireTarget ? StartCoroutine(UseItem_Target())
                            : StartCoroutine(UseItem_Pos());//根据判断选择执行获取位置（目标）的协程
    }//激活效果
    protected virtual void ActiveEffect(Vector2 spawnPos,GameObject unit)
    {
        GameObject newItem=Instantiate(effectPrefab,
                        ownerItemList._owner.transform.position,
                        ownerItemList._owner.transform.rotation);
        Bullet bullet=newItem.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (unit != null)
            {
               // bullet.AttackTarget(unit);
            }//直接打击目标（属于需要目标类型）
            else
            {
                bullet.AttackPos(spawnPos);
            }//直接打击位置
        }//如果属于子弹类型
        DisposableItem();//一次性道具
    }
    protected virtual void DisposableItem()
    {
        if (!repeatable)
        {
            Debug.Log("消耗" + itemName);
            ownerItemList.RemoveItem(this);
            Destroy(gameObject);
            UIEvent.UpdateUnitInfo?.Invoke();
        }//消耗品道具
    }
    protected virtual IEnumerator UseItem_Pos()
    {
        UnitAttribute useUnit = ownerItemList._owner;
        while (isUse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float distance = Vector2.Distance(mousePos, useUnit.transform.position);

                //确认使用位置

                if (distance > useRange)
                {
                    useUnit.SetUnitMovePos(mousePos);//移动到鼠标位置（可以使用道具的位置）
                    useUnit.isAction = true;//正在行动
                    Debug.Log("正在移动到可以使用道具的范围");
                }//不在使用距离
                while (distance > useRange)
                {
                    yield return 5;
                    distance = Vector2.Distance(mousePos, useUnit.transform.position);
                }//检测距离

                UnitEvent.resetUnitAllBehavior.Invoke(useUnit);//重置状态（结束 使用距离不够产生的移动）
                useUnit._isUseItem = true;
                Debug.Log("正在使用道具");

                //useUnit.unitMove.rotatPos=mousePos;//单位需要转向目标（位置）

                while (useUnit._isUseItem)
                {
                    for (int i = 0; i < useNumber; i++)
                    {
                        yield return new WaitForSeconds(delay);
                        ActiveEffect(mousePos,null);
                    }//单次道具使用次数
                    useUnit._isUseItem = false;
                }//单位正在使用道具

                UnitEvent.resetUnitAllBehavior(useUnit);
                EndUse();
                
                Debug.Log("道具使用完成");
                yield break;
            }
            if (Input.GetMouseButtonDown(1))
            {
                UnitEvent.resetUnitAllBehavior(useUnit);
                EndUse();
                Debug.Log("取消使用");
                yield break;
            }//取消使用
            yield return null;
        }
    }
    protected virtual IEnumerator UseItem_Target()
    {
        yield return null;

        UnitAttribute useUnit = ownerItemList._owner;
        GameObject target = null;
        float distance=Mathf.Infinity;
        Debug.Log("选择目标 ");
        while (isUse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider!=null&&hit.collider.CompareTag(targetTag))
                {
                    target = GetTarget(hit.collider); 
                }//目标 标签正确
                else
                {
                    EndUse();
                    Debug.Log("目标对象错误");
                    yield break;
                }//目标 标签错误

                if (target != null)
                {
                    distance = Vector2.Distance(target.transform.position, useUnit.transform.position);
                    if (distance>useRange) 
                    {
                            useUnit.SetUnitMovePos(target.transform.position);
                            useUnit.isAction = true;
                            Debug.Log("移动到使用范围");                       
                    }
                }//确认目标
                else
                {
                    Debug.Log("没有目标");
                    EndUse();
                    yield break;
                }//没有目标

                while (distance > useRange)
                {
                    yield return 5;
                    distance = Vector2.Distance(target.transform.position, useUnit.transform.position);
                }//如果需要移动

                UnitEvent.resetUnitAllBehavior(useUnit);
                useUnit._isUseItem = true;
                //useUnit.unitMove.rotatPos = target.transform.position;
                while (useUnit._isUseItem)
                {
                    for (int i = 0; i < useNumber; i++)
                    {
                        yield return new WaitForSeconds(delay);
                        ActiveEffect(Vector2.zero, target);
                    }
                    Debug.Log("使用完成");
                    EndUse();
                    UnitEvent.resetUnitAllBehavior(useUnit);
                    yield break;
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                UnitEvent.resetUnitAllBehavior(useUnit);
                EndUse();
                Debug.Log("取消使用");
                yield break;
            }//取消使用
            yield return null;
        }//正在使用
    } 
    protected virtual GameObject GetTarget(Collider2D hitTarget)
    {
        return hitTarget.gameObject;
    }
    public void EndUse()
    {
        isUse = false;
        ownerItemList.IsUseItem(null);
        myCoroutine = null;
    }
    public void StopMyCoroutine()
    {
            StopCoroutine(myCoroutine);
    }//停止执行
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
        targetTag= itemData.targetTag;
    }//应用数据
}//功能道具
