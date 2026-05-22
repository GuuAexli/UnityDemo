using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BulletData",menuName ="BulletData/BulletData")]
public class BulletData : Data
{
    public float bulletSpeed;//子弹速度
    public float bulletDamage;//子弹伤害
    public float bulletPenetration;//穿深
    public float bulletTime;//存在时间
    public GameObject expiosion;//爆炸效果
    public GameObject traces;//痕迹
}
