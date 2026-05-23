using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Tilemap引用")]
    public Tilemap walkableTilemap;   // 可行走区域的Tilemap（或主Tilemap）
    public Tilemap obstacleTilemap;   // 障碍物Tilemap（可选，用于标记不可行走）

    private int width, height;//地图范围
    public Node[,] grid;//格子组
    public bool[,] occupiedCell;//被占用的格子组
    public Vector3Int boundsMin;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    /// <summary>
    /// 构建网格
    /// </summary>
    public void BuildGrid()
    {
        BoundsInt bounds = walkableTilemap.cellBounds;
        boundsMin = bounds.min;
        //可行走的边界
        width = bounds.size.x;
        //边界最大x值
        height = bounds.size.y;
        //边界最大y值


        grid = new Node[width, height];
        occupiedCell = new bool[width, height];
        //设置格子范围

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(boundsMin.x+x,boundsMin.y+y, 0);
                bool walkable = walkableTilemap.HasTile(cellPos);
                // 有瓦片即视为可行走
                if (obstacleTilemap != null && obstacleTilemap.HasTile(cellPos))
                    walkable = false; 
                // 障碍物覆盖
                grid[x ,y] = new Node(new Vector2Int(cellPos.x,cellPos.y), walkable);
            }
        }//设置格子组内初始化
    }//生成网格  

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3Int cell = walkableTilemap.WorldToCell(worldPos);
        //Tilemap 世界坐标 转 格子坐标
        return new Vector2Int(cell.x, cell.y);
        //返回格子位置
    }// 世界坐标 → 格子坐标
 
    public Vector3 CellToWorld(Vector2Int gridPos)
    {
        return walkableTilemap.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        //格子位置 转 世界位置 获取格子中心在世界的位置
    }// 格子坐标 → 世界坐标（格子中心）

    public Node GetNode(Vector2Int gridPos)
    {
        int x = gridPos.x - boundsMin.x;
        int y = gridPos.y - boundsMin.y;
        if (x >= 0 && x < width && y >= 0 && y < height)
            return grid[x, y];
            //在范围内 返回指定的格子信息
        return null;
    }//获取节点

    public List<Node> GetNeighbours(Node node,UnitNavMove unit=null)
    {
        List<Node> neighbours = new List<Node>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                //自己的位置 跳过
                Vector2Int neighbourPos = new Vector2Int(node.cellPos.x + dx,
                                                    node.cellPos.y + dy);
                if (IsWalkable(neighbourPos,unit))
                {
                    Node neighbour = GetNode(neighbourPos);
                    if (neighbour != null)
                        neighbours.Add(neighbour);
                }
            }
        }
        return neighbours;
    }// 获取邻居节点（八方向）

    public void SetOccupied(Vector2Int cellPos,bool occupied)
    {
        int x = cellPos.x - boundsMin.x;
        int y = cellPos.y - boundsMin.y;
        if(x>=0&&x<width&&y>=0&&y<height)
            occupiedCell[x,y]=occupied;
        //位置在框架内 设置占用状态
    }//设置格子占用情况

    public void UpdateOccupied(List<Vector2Int> oldList,List<Vector2Int> newList)
    {
        foreach(var pos in oldList)
        {
            SetOccupied(pos, false);
            //Debug.Log(pos + "移除占用");
        }//移除旧占用
        foreach(var pos in newList)
        {
            SetOccupied(pos, true);
            //Debug.Log(pos + "占用");
        }//设置新占用
    }//更新占用状态
    public bool IsWalkable(Vector2Int cellPos,UnitNavMove unit=null)
    {
        Node node = GetNode(cellPos);
        if (node == null) return false;//没有节点
        if(!node.walkable) return false;//不可行走
        
        int x = cellPos.x - boundsMin.x;
        int y = cellPos.y - boundsMin.y;
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        if (occupiedCell[x, y])
        {
            if (unit != null && unit.IsOccupiedGrid(cellPos))
                //移动位置在自己的占用网格内
                return true;//可通行
            else
                return false;//不可行走
        }//网格被占用 如果在自己占用内 可通行
        return true;//没有被占用

    }//检查格子可行走情况
    public List<Vector2Int> GetOccupiedGrid(Collider2D col)
    {
        List<Vector2Int> grid = new List<Vector2Int>();
        
        Bounds colBounds= col.bounds;
        //设置碰撞体边界
        Vector2Int gridMin = WorldToCell(colBounds.min);
        Vector2Int gridMax = WorldToCell(colBounds.max);
        //获取框架范围
        for(int x = gridMin.x; x <= gridMax.x; x++)
        {
            for(int y = gridMin.y; y <= gridMax.y; y++)
            {
                Vector2Int cellPos=new Vector2Int(x, y);
                Vector3 cellCenter=CellToWorld(cellPos);
                if (col.OverlapPoint(cellCenter))
                {
                    grid.Add(cellPos);
                }//格子中心在碰撞体范围内
            }
        }
        return grid;
    }//获取占用网格 通过collide 
    public Vector3 GetClosesWalkableCellAround(Vector2Int centerCell, UnitNavMove unit,int maxRadius= 20)
    {
        for(int radius=0;radius<maxRadius; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for(int dy=-radius; dy <= radius; dy++)
                {
                    if(Mathf.Abs(dx)+Mathf.Abs(dy)!=radius) continue;//只检查周边的格子

                    Vector2Int cellPos = centerCell+new Vector2Int(dx, dy);
                    if (IsWalkable(cellPos,unit))
                    {
                        return CellToWorld(cellPos) ;
                    }//周围可行走格子
                }
            }
        }
        return Vector3.zero;//周围没有可行走格子
    }
    public bool IsOccupied(Vector2Int cellPos)
    {
        int x=cellPos.x-walkableTilemap.cellBounds.xMin;
        int y = cellPos.y - walkableTilemap.cellBounds.yMin;
        if (x >= 0 && x < width && y >= 0 && y < height)
            return occupiedCell[x, y];
        return false;
    }//检查格子占用情况

}

