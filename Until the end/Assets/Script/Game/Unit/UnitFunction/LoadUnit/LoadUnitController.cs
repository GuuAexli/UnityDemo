using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadUnitController : MonoBehaviour
{
    public TransportVehicle loadUnit;
    public UnitAttribute enterUnit;

    public bool isSelectedButton;//选择这个按钮
    [SerializeField] bool isSelectUnit; //正在选择单位
        public bool selectedEnterUnit;
        public bool selectedLoadUnit;
    public int targetLayer;
    void OnEnable()
    {
        UIEvent.OnActiveButton += EventListener;
    }
    private void OnDestroy()
    {
        UIEvent.OnActiveButton -= EventListener;
    }
    public void EventListener(UnitButtonUI.ActiveButtonType type,UnitAttribute unit)
    {
        if (type == UnitButtonUI.ActiveButtonType.Enter)
        {
            isSelectedButton = true;
            targetLayer=unit.gameObject.layer;
            EnterUnit(unit);
        }//进入按钮
        if(type== UnitButtonUI.ActiveButtonType.Exit)
        {
            ExitUnit(unit);
        }//离开按钮
    }//事件监听
    public void Update()
    {
        if(isSelectedButton)
            if (isSelectUnit)
            {
                SelectedUnit();
            }
    }
    public void EnterUnit(UnitAttribute Unit)   
    {
        
        if(Unit is TransportVehicle vehicle)
        {
            loadUnit = vehicle;
            selectedEnterUnit = true;
            Debug.Log("选择进入的单位");
        }//以 装载单位 选择 进入单位
        else
        {
            enterUnit = Unit;
            selectedLoadUnit=true;
            Debug.Log("选择装载的单位");
        }//以 进入单位 选择 装载单位

        isSelectUnit = true;
            //进入 选择装载的单位 判断
        GameController.Instance.isSelectedLoadUnit = true;
        //改变管理器的选择状态
    }//进入

    public void MoveToLoadUnit(VehicleAttribute unit)
    {

        enterUnit.isAction=true;
        loadUnit.isAction = true;
        //通知装载双方

        loadUnit.LoadUnit(enterUnit.gameObject);
        //enterUnit.unitMove.MoveToLoadUnit(unit,unit.transform.position);
        enterUnit._canAttack = false;

    }//进入方 移动到 装载方 进入方停止攻击（isLoad） 装载方开始检测范围内的进入目标（isLoad）
    public void MoveToEnterUnit(UnitAttribute unit)
    {
        loadUnit.isAction = true;
        loadUnit.LoadUnit(enterUnit.gameObject);
        //loadUnit.unitMove.MoveToLoadUnit(unit,unit.transform.position);
    }//装载方 移动到 进入方 只需要装载方移动并检测进入目标（isLoad）

    public void ExitUnit(UnitAttribute unit)
        //离开
    {
        unit.GetComponent<LoadList>().ExitUnit();
    }
    public void  SelectedUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mosPos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mosPos,Vector2.zero);

            if(hit.collider != null)
            {
                UnitAttribute unit=hit.collider.GetComponent<UnitAttribute>();
                if (unit != null&&unit.gameObject.layer==targetLayer)
                {
                    if(selectedEnterUnit&& unit._canEnterObject)   
                    {
                        Debug.Log("移动到需要装载的单位");
                        MoveToEnterUnit(enterUnit=unit);
                    }//以装载方 选择 进入单位
                    else if(selectedLoadUnit&&unit is TransportVehicle transport)   
                    {
                        Debug.Log("移动到需要进入的载具");
                        MoveToLoadUnit(loadUnit=transport);
                    }//以进入方 选择 装载单位
                    ResetState();
                    Debug.Log("选择完成");
                    return;
                }
                //对象错误
                ResetState();
                Debug.Log("目标对象无效");
                return;
            }
            //没有对象
            ResetState();
            Debug.Log("没有对象");
            return;
        }
        if (Input.GetMouseButtonDown(1))
            //取消选择
        {
            Debug.Log("取消装载");
            ResetState();
        }

    }
    void ResetState()
    {
        loadUnit = null;
        enterUnit = null;

        isSelectedButton =false;
        isSelectUnit = false;
            selectedEnterUnit = false;
            selectedLoadUnit = false;
        GameController.Instance.isSelectedLoadUnit=false;
        UIEvent.RemoveAllButtonActive?.Invoke();
        UIEvent.ResetButtonState?.Invoke();

    }
}
