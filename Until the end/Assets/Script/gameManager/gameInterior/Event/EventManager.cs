using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    [SerializeField] private List<EventCategory> eventCategories= new List<EventCategory>();
    public EventCategory currentEvent;
    //事件类别组
    [SerializeField] private float eventInterval;//事件间隔
    [SerializeField] private Collider2D col;//
    [SerializeField] private Bounds bounds;//范围
    public Bounds _bounds=>bounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
        
    }
    private void Start()
    {
        StartCoroutine(SpewnEvent());
    }
    IEnumerator SpewnEvent()
    {
        while(EventManager.Instance != null)
        {
            yield return new WaitForSeconds(eventInterval);
            SpecificEvent();
        }
        Destroy(gameObject);
    }
    public void SpecificEvent()
    {
        EventCategories();//事件类型

        if (currentEvent.events.Count == 0)
        {
            Debug.Log(currentEvent.type+"没有具体事件");
            return;
        }
        int sumWeight = 0;
        foreach(SpecificGameEvent SGE in currentEvent.events)
        {
            sumWeight += SGE.weight;
        }
        //Debug.Log("最大权重"+sumWeight);
        int value = Random.Range(1, sumWeight+1); //[1,sumWeight)int区间 [1f,sumWeight]float区间
        //Debug.Log("权重数"+value);
        int currentWeight = 0;
        foreach(SpecificGameEvent SGE in currentEvent.events)
        {
            currentWeight+= SGE.weight;
            if (value <=currentWeight)
            {
                //Debug.Log("事件名"+SGE.eventName+ "现在权重值"+currentWeight);
                Instantiate(SGE.eventPrefab);
                return;
            }
        }
        Debug.Log("无事发生");
    }//具体事件

    public EventCategory EventCategories()
    {
        int sumWeight = 0;
        foreach (EventCategory category in eventCategories)
        {
            sumWeight += category.weight;
        }//权重和 历遍所以事件种类
        int currentWeight = 0;//现在的权重
        int value = Random.Range(1, sumWeight);//实际权重值
        foreach (EventCategory category in eventCategories)
        {
            currentWeight += category.weight;
            if (value <= currentWeight)
                return currentEvent = category;
        }//根据权重选择事件类型 
        return currentEvent = eventCategories[1];//中立事件
    }//事件类型
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(bounds.center,bounds.size);
    }
    
}
