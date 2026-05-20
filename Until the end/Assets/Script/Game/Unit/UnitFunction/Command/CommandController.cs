using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{

    public bool isSelectedButton;
    [SerializeField] private bool isSelected;

    [SerializeField] private UnitAttribute atkUnit;
    private void OnEnable()
    {
        UIEvent.OnActiveButton += EventListener;
    }
    private void OnDestroy()
    {
        UIEvent.OnActiveButton -= EventListener;
    }
    private void Update()
    {
        if (isSelectedButton)
            if (isSelected)
            {
                SelectedTarget();
            }
    }

    public void EventListener(UnitButtonUI.ActiveButtonType type,UnitAttribute unit)
    {
        
            if (type == UnitButtonUI.ActiveButtonType.Attack)
            {
                isSelectedButton = true;
                AttackButton(unit);
            }
    }
    public void AttackButton(UnitAttribute unit)
    {
        GameController.Instance.isCommand = true;
        isSelected = true;
        atkUnit = unit;
        Debug.Log("选择攻击目标");
    }
    private void SelectedTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mosPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mosPos, Vector2.zero);
            
            if (hit.collider!= null)
            {
                UnitAttribute target = hit.collider.GetComponent<UnitAttribute>();
                if (target != null&&target.gameObject.tag=="red_tag")
                {    
                    Debug.Log("目标" + target.name);

                    ResetState();
                    return;
                }
                Debug.Log("无效目标");
                ResetState();
                return;
            }
            Debug.Log("取消选择");
            ResetState();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("取消选择");
            ResetState();
        }
    }
    private void ResetState()
    {
        isSelectedButton = false;
        isSelected = false;
        GameController.Instance.isCommand = false;
        UIEvent.RemoveAllButtonActive?.Invoke();
        UIEvent.ResetButtonState?.Invoke();
    }
}
