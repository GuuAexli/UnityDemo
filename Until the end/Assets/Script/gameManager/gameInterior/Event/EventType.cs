using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum eventType
{
      Good,       //好
      Neutral,    //无关紧要 = 中立
      Bad         //坏
}//事件类别
[System.Serializable]
public class EventCategory
{
    public eventType type;//类别
    public int weight;//概率
    public List<SpecificGameEvent> events;//所属的子类 事件组 形参events
}//事件类别 概率 事件组
[System.Serializable]
public class SpecificGameEvent
{
    public GameObject eventPrefab;//事件预制体
    public string eventName;
    public int weight;
}//具体事件 权重
public abstract class SpecificEvent : MonoBehaviour
{
    [SerializeField]private string eventName;//事件名
    [SerializeField]private string description; //描述
}//事件基类





