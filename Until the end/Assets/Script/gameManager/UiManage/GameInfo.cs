using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    public Text supplyText;//补给
    public Text InningText;//回合
    public Text descriptionText;//描述
    GameController gc;
    private void Start()
    {
        gc = GameController.Instance;
        if (gc == null)
            Debug.LogError("没有游戏管理器");

        UIEvent.UpdateSupplyInfo += UpdateSupplyInfo;
        UIEvent.UpdateInningInfo += UpdateInningInfo;
    }
    private void OnDestroy()
    {
        UIEvent.UpdateSupplyInfo -= UpdateSupplyInfo;
        UIEvent.UpdateInningInfo -= UpdateInningInfo;
    }

    void UpdateSupplyInfo()
    {
        supplyText.text = $"{gc.Supply}";
    }
    void UpdateInningInfo()
    {
        InningText.text = $"{gc.Inning}";
    }
}
