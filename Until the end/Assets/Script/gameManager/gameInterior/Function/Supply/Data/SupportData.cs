using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="SupportData",menuName="SupportData")]
public class SupportData : Data
{
    public bool canDraw;//可以绘制区域
    public bool canRotate;//可以旋转
    public bool canExtend;//可以延伸 

    public GameObject shell;//炮弹

    public int eachTimeNumber;//每次数量
    public int number;//射击数量
    public float delay;//开始延迟
    public float interval;//每次间隔
    public float radiusRange;//支援宽度
}
