using UnityEngine;
//存储的武器数据
[CreateAssetMenu(menuName = "WaeponData/weaponData")]
public class WeaponData : Data
{
    [Header("武器UI")]
    public Sprite menuIcon;                     //按钮武器图片 
    public Sprite weaponIcon;                   //武器图片

    [Header("武器时间属性")]
    public float aimTime;                       //瞄准时间
    public float attackInterval;                //间隔
    public float attackCooling;                 //冷却

    [Header("武器属性")]
    //命中率=单位*体积* 距离
        public float closeRangeAccurracy;           //近距离精准度 
        public float mediumRangeAccurracy;          //中距离精准度
        public float longRangeAccurracy;            //远距离精准度
    public float attackDuration;                //攻击持续时间
    public GameObject bullet;                   //子弹
    public float suppressionValue;              //压制力
    [Range(0, 100)] public float attackRange;   //攻击半径
    [Range(0, 100)] public float duffusion;     //散布

    [Header("倍率")]
    public int penetrationMultiplier;         //穿深倍率
    public float damageMultiplier;              //伤害倍率
    [Header("载具")]
    public float barbetteRotatSpeed;            //载具炮台旋转速度
    //public GameObject barbette;                 //载具炮台

    //public LayerMask targetLayer;             //目标图层

}
