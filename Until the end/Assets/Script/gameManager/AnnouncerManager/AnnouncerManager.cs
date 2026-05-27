using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncerManager : MonoBehaviour
{
    public AnnouncerManager Instance { get; private set; }

    [SerializeField] GameObject announcerText;//文本预制体
    [SerializeField] Transform textPos;//文本生成位置
    [SerializeField] float showTime = 2;//显示时间
    [SerializeField] float hideTime = 1f;//隐藏时间

    public string GetColorByFaction(Faction faction)
    {
        switch (faction) 
        {
            case Faction.Blue: return "#0000ff";
            case Faction.Red:return "#ff0000";
            default: return "white";
        }

    }
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnEnable()
    {
        UIEvent.OnUnitDied += OnUnitKilled;
        UIEvent.OnMessageText += ShowMessage;
    }
    public void OnDisable()
    {
        UIEvent.OnUnitDied -= OnUnitKilled;
        UIEvent.OnMessageText -= ShowMessage;
    }
    public void OnUnitKilled(UnitAttribute killer,UnitAttribute victim)
    {
        string message = $"{ColorName(killer)}击杀{ColorName(victim)}";
        ShowMessage(message);

    }//单位被击杀
    private string ColorName(UnitAttribute unit)
    {
        string colorName = GetColorByFaction(unit.faction);
        return $"<color={colorName}>{unit.unitName}</color>";
    }
    public void ShowMessage(string message)
    {
        GameObject newMsg = Instantiate(announcerText, textPos);
        Text text = newMsg.GetComponent<Text>();
        if (text == null) {Destroy(newMsg); return; }

        text.text = message;
        StartCoroutine(MessageDestroy(newMsg));
    }//显示信息
    IEnumerator MessageDestroy(GameObject msgObj)
    {
        if (msgObj == null) yield break;
        CanvasGroup cg = msgObj.GetComponent<CanvasGroup>();
        float time = 0;
        while (time < showTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        if (cg == null) {Destroy(msgObj);yield break; }
        float hide = 0;
        while ((hide += Time.deltaTime) < hideTime)
        {
            float alpha = 1 - (hide / hideTime);
            cg.alpha = alpha;
            yield return null;
        }
        Destroy(msgObj);
    }
}
