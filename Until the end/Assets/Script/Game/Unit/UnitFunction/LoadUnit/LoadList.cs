using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadList : MonoBehaviour
{
    public bool isLoad;
    public Collider2D LoadRange;//装载范围
    public List<GameObject> unitList=new List<GameObject>();//装载组
    public List<GameObject> targetUnit=new List<GameObject>();//目标组
    public int maxLoad;
    void Awake()
    {
        if(GetComponent<VehicleAttribute>().unitData is VehicleData LoadNum)
        {
            maxLoad = LoadNum.maxLoad;
        }
        else
        {
            Debug.Log("装载上限错误");
            enabled = false;
        }

        if (LoadRange != null)
        {
            LoadRange.isTrigger = true;
        }//确认是触发器
        else Debug.Log("没有装载范围触发器");
    }
    public void OnTriggerStay2D(Collider2D col)
    {

        if (isLoad && targetUnit != null)
        {
            foreach (GameObject unit in targetUnit.ToList())
            {
                if (unit == col.gameObject)
                {
                    UnitAttribute loadUnit =col.gameObject.GetComponent<UnitAttribute>();
                    if (loadUnit != null && loadUnit._canEnterObject)
                    {
                        targetUnit.Remove(unit);//移除需要装载的目标组
                        EnterUnit(loadUnit);
                    }
                }
            }
        }
        else isLoad = false;

    }//通过触发器 判断是否进入装载范围
    public void EnterUnit(UnitAttribute unit)
    {
        UnitEvent.resetUnitAllBehavior.Invoke(unit);
        UnitEvent.resetUnitAllBehavior.Invoke(GetComponent<UnitAttribute>());

        if (unitList.Count < maxLoad)
        {
            unitList.Add(unit.gameObject);
            unit.transform.SetParent(this.gameObject.transform, true);
            unit.gameObject.SetActive(false);
            Debug.Log("装载完成");
        }
        else Debug.Log("超过装载数");

    }
    public void ExitUnit()
    {
        if (unitList.Count != 0)
            foreach (GameObject unit in unitList.ToList())
            {
                unit.gameObject.SetActive(true);//激活显示单位
                unit.transform.SetParent(null);//取消父级

                unit.GetComponent<UnitAttribute>().ApplyLevelData();
                //假如单位在掩体时进入载具会导致体积减少 所以需要重新 获取等级状态 来纠正 单位体积

                unit.transform.position = new Vector2(this.transform.position.x + Random.Range(-1f, 1f), transform.position.y - 2);
                UnitEvent.resetUnitAllBehavior.Invoke(unit.GetComponent<UnitAttribute>());//重置单位行为
                unitList.Remove(unit);
                Debug.Log("卸载完成");
            }
        else Debug.Log("没有装载单位");
    }
}
