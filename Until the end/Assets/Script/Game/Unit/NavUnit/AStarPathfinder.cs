using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class AStarPathfinder
{
    public static List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld,UnitNavMove unit=null)
    {
        GridManager gm =  GridManager.Instance;

        Vector2Int startCell = gm.WorldToCell(startWorld);
        //开始格子 
        Vector2Int targetCell = gm.WorldToCell(targetWorld);
        //目标格子
        Node startNode = gm.GetNode(startCell);
        //获取开始节点信息
        Node targetNode = gm.GetNode(targetCell);
        //获取目标节点信息
        if (startNode == null || targetNode == null)
            return null;
            //开始格子不为空 目标格子不为空 
        if(!gm.IsWalkable(targetCell,unit))
        {
            Vector3 target=gm.GetClosesWalkableCellAround(targetCell,unit);
            targetCell = gm.WorldToCell(target);
            targetNode = gm.GetNode(targetCell);
            Debug.Log("移动到目标最近位置"+targetCell);
        }//目标格子不可行走 改为寻找目标最近的格子 

        List<Node> openSet = new List<Node>();
        //开启列表 可能移动的格子
        HashSet<Node> closedSet = new HashSet<Node>();
        //关闭列表 检查过的格子 
        openSet.Add(startNode);
        //添加开始位置

        startNode.gCost = 0;
        startNode.hCost = GetHeuristic(startCell, targetCell);
        //
        while (openSet.Count > 0)
        //开启列表会在寻找邻居的时候添加
        {
            Node currentNode = GetLowestFCostNode(openSet);
            //开始时 开启列表只有开始点
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            //移除开启列表加入关闭列表 开始检查 现在节点

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }//找到目标节点

            foreach(Node neighbour in gm.GetNeighbours(currentNode,unit))
            {
                if (closedSet.Contains(neighbour))
                    continue;//在关闭列表中 （其他节点的邻居）

                int moveCost = (neighbour.cellPos.x != currentNode.cellPos.x &&
                                neighbour.cellPos.y != currentNode.cellPos.y) ? 14 : 10;
                //对角线移动价值14 水平垂直移动价值10
                Vector3Int tilePos = (Vector3Int)neighbour.cellPos;
                int newGCost = currentNode.gCost + moveCost+GetTileCost(tilePos);
                //新的移动价值（不同点移动到当前节点之间的价值可能不一样）
                //最开始的节点的gCost初始化值为0
                if (newGCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost= newGCost;
                    //设置邻居移动价值（选择节点到邻居节点）
                    neighbour.hCost = GetHeuristic(neighbour.cellPos, targetCell);
                    //设置邻居的启发价值（邻居节点直接到目标节点）
                    neighbour.parent = currentNode;
                    //设置邻居的父节点（回溯路径）

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }//不在开启列表中
                }
            }
        }
        Debug.Log("没有路径");
        return null;
    }
    public static int GetTileCost(Vector3Int cell)
    {
        TileBase tileBase = GridManager.Instance.walkableTilemap.GetTile(cell);
        if(tileBase is CustomTile tileCost)
        {
            //Debug.Log(tileCost.moveCost);
            return tileCost.moveCost;
        }
        else
        {
            Debug.Log("没有瓦片数据,默认1");
            return 1;
        }
    }//获取瓦片的移动价值
    private static int CalculateHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) * 10 + Mathf.Abs(a.y - b.y) * 10;
    }// 曼哈顿距离
    private static int GetHeuristic(Vector2Int a,Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 14 * Mathf.Min(dx, dy) + 10*Mathf.Abs(dx-dy);
    }//对角线距离
    private static Node GetLowestFCostNode(List<Node> nodes)
    {
        Node best = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].fCost < best.fCost || (nodes[i].fCost ==
                best.fCost) && nodes[i].hCost < best.hCost)

                best = nodes[i];
        }
        return best;
    }//计算最小hCost值
    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        //路径
        Node currentNode = endNode;
        //现在节点 = 目标节点
        while (currentNode != startNode)
        {
            path.Add(GridManager.Instance.CellToWorld(currentNode.cellPos));
            //记录节点所在世界位置
            currentNode = currentNode.parent;
            //现在节点=现在节点的父集 邻居检查时设置
        }//现在节点不是开始节点
        path.Reverse();
        //反转路径
        return path;
    }//反转列表 获取移动路径
}
