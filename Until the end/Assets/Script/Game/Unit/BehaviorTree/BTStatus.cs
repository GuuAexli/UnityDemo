using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BTStatus
{
    Success,//成功
    Failure,//失败
    Running//运行中
}//行为状态

public class Blackboard 
{
    private Dictionary<string,object> data= new Dictionary<string,object>();
    //私有字典 data ，字符串=>对象.object (C#)任何类型（int，float，List<>）(需要强制转换) 
    public void Set<T>(string key, T value) => data[key] = value;
    //<T> 表示该方法有一个泛型类型参数,调用时可以指定为具体类型，如 int、string 
    public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    //如果字典包含该 key，则取出 data[key]（它是 object 类型），并强制转换为 T 类型(T)data[key]。
    //如果不包含,则返回 default(T)（对于引用类型为 null，对于值类型如 int 为 0，bool 为 false 等
    //Set("useItem"(key),item(Item))  Get<Item>("useItem"(key))=>item(Item)

    public bool HasKey(string key)=> data.ContainsKey(key);//判断键是否有值
    public void Remove(string key)=>data.Remove(key);//移除钥匙
}

public static class BlackboardKeys
{
    // 单位组件
    public const string Attribute = "attribute";
    public const string NavMove = "navMove";
    public const string Combat = "combat";
    public const string ItemList = "itemList";

    // 行为相关
    public const string PatrolPos = "patrolPos";
    public const string ForcedMove = "forcedMove";

    // 主动道具
    public const string ManualUseItem = "manualUseItem";
    public const string ManualMoveToItemRange = "manualMoveToItemRange";

    // 辅助道具
    public const string AssistantItem = "assistantItem";
    public const string MoveToItemRange = "moveToItemRange";
}