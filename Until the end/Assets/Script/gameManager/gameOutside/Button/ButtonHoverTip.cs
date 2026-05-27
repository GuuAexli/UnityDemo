using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverTip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public string text;
    public void SetData(Data data)
    {
        text= data.prefabName+"\n"+data.description + "\n花费：" + data.costValue;
    }
    public void OnPointerEnter(PointerEventData data)
    {
        UIEvent.OnHoverTip?.Invoke(text);
    }//鼠标进入
    public void OnPointerExit(PointerEventData data) 
    {
        UIEvent.OnHoverTip?.Invoke(null);
    }//鼠标离开 
}
