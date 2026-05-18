using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
        public Vector2Int cellPos;   // 格子坐标
        public bool walkable;        // 是否可行走
        public int gCost;            // 从起点到当前节点的代价
        //起点到当前位置 距离目标点的价值
        public int hCost;            // 从当前节点到目标节点的估算代价
        //当前位置到目标点价值
        public int fCost => gCost + hCost;
        public Node parent;          // 路径中的父节点

        public Node(Vector2Int gridPos, bool walkable)
        {
            this.cellPos = gridPos;
            this.walkable = walkable;
        }
}
