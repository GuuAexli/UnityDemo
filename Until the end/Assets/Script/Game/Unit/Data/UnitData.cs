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

    public Sprite buttonIcon;                               //按钮图片
    public Sprite unitIcon;                                 //单位图片
    public Faction unitFaction;
    public UnitLevelData level;                             //对应 单位的等级数据类
    public int destroyValue;                                //破坏值(进入防御点扣除的健康值)
    public WeaponData[] weapon;                             //初始 武器  武器组里随机使用一个武器

    public bool canEnterObject;                             //可以进入对象（模型/掩体）


    public override void Spawn()
    {
        Vector2Int pos = GameController.Instance.defenseZone.walkableCell[
                                Random.Range(0, GameController.Instance.defenseZone.walkableCell.Count)];
        Vector3 spawnPos=GridManager.Instance.CellToWorld(pos);
        GameObject unit= Instantiate(prefab, spawnPos,Quaternion.identity);
        unit.GetComponent<UnitAttribute>().SetUnitMovePos(spawnPos+new Vector3(0,5));
        
    }
}

