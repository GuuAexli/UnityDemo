using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="UnitLevelData/LevelData")]
public class UnitLevelData : ScriptableObject
{
    [System.Serializable]
    //创建
    public struct LevelStats {
        //每个等级阶段的属性设置
        public float health;//健康值
        public float moveSpeed;//移动速度
        public float rotationSpeed;//旋转速度
        public float accurracy;//单位精准度
        public float volum;//体积
        public int armor;//装甲
        public int nextLevelExp;//下一等级需要的经验

        public float decayFear;//恐惧值衰减
    }
    //LevelStats组的内部
public LevelStats[] levels;
    //LevelStats的形参
}
