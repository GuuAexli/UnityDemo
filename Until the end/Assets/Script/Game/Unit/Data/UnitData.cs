using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName ="UnitData",menuName ="UnitData/UnitData")]


//order在Assets里面的排列顺序（Int）越小越前
//removeFileExtension 是否移除文件拓展名
public class UnitData : Data
    //               允许在主文件夹里面创建自定义的数据单位
    //共有属性
{
    public bool isVehicle;

    //public string unitName;                                 //对应 单位名字
    //public GameObject prefab;                               //对应 单位预制
    public Sprite buttonIcon;                               //按钮图片
    public Sprite unitIcon;                                 //单位图片
    public UnitLevelData level;                             //对应 单位的等级数据类
    //public int costValue;                                   //对应 花费
    public int destroyValue;                                //破坏值
    public WeaponData[] weapon;                             //初始 武器  武器组里随机使用一个武器

    //public SpecialWeaponData specialWeapon;                 //特殊武器

    public bool canEnterObject;                             //可以进入对象（模型/掩体）



}
[CreateAssetMenu(fileName ="InfantryData",menuName ="UnitData/InfantryData")]
public class InfantryData : UnitData 
{
    //默认就是步兵单位
}
[CreateAssetMenu(fileName ="VehicleData",menuName ="UnitData/VehicleData")]
public class VehicleData : UnitData 
{
    
    public List<GameObject> loadList = new List<GameObject>();//装入成员组
    public int maxLoad;                                   //最大装入量
}//载具

