using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    public Text supplyText;//补给
    public Text InningText;//回合
    public Text hoverTip;//描述
    public GameObject gameEnd;
    public Text text;
    public Button continueButton;
    public Button breakButton;
    GameController gc;
    private void Start()
    {
        gc = GameController.Instance;
        gameEnd.SetActive(false);
        if (gc == null)
            Debug.LogError("没有游戏管理器");

        UIEvent.UpdateSupplyInfo += UpdateSupplyInfo;
        UIEvent.UpdateInningInfo += UpdateInningInfo;
        UIEvent.OnHoverTip += OnHoverTip;
        UIEvent.GameEnd += GameEnd;
        breakButton.onClick.AddListener(()=>Back());
        continueButton.onClick.AddListener(()=> ContinueGame());
    }
    private void OnDestroy()
    {
        UIEvent.UpdateSupplyInfo -= UpdateSupplyInfo;
        UIEvent.UpdateInningInfo -= UpdateInningInfo;
        UIEvent.OnHoverTip -= OnHoverTip;
        UIEvent.GameEnd -= GameEnd;
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
    void GameEnd(int inning)
    {
        gameEnd.SetActive(true);
        text.text = "坚守回合数" + inning;
    }
    void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }
    void Back()
    {
        SceneManager.LoadScene("Main");
    }
}
