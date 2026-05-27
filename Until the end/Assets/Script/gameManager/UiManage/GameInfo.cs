using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    public Text supplyText;//补给
    public Text InningText;//回合
    public Text hoverTip;//描述
    GameController gc;
    private void Start()
    {
        gc = GameController.Instance;
        if (gc == null)
            Debug.LogError("没有游戏管理器");

        UIEvent.UpdateSupplyInfo += UpdateSupplyInfo;
        UIEvent.UpdateInningInfo += UpdateInningInfo;
        UIEvent.OnHoverTip += OnHoverTip;
    }
    private void OnDestroy()
    {
        UIEvent.UpdateSupplyInfo -= UpdateSupplyInfo;
        UIEvent.UpdateInningInfo -= UpdateInningInfo;
        UIEvent.OnHoverTip -= OnHoverTip;
    }
    void OnHoverTip(string tip)
    {
        hoverTip.text = tip;
    }
    void UpdateSupplyInfo()
    {
        supplyText.text = $"{gc.cost}";
    }
    void UpdateInningInfo()
    {
        InningText.text = $"{gc.Inning}";
    }
}
