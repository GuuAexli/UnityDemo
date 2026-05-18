using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Grid grid;

    public TileBase waterTile;
    public TileBase mudTile;
    public TileBase grassTile;
    
    public Bounds bounds;
    public Collider2D col;

    [Tooltip("噪音等级")]
    public float noiseScale=0.2f;
    [Header("地形阈值")]
    [Range(0, 1)]
    [Tooltip("低于此值生成水")]
    public float waterThreshold = 0.3f;
    [Range(0, 1)]
    [Tooltip("介于水阈值和此值之间生成泥地，高于此值生成草地")]
    public float mudThreshold = 0.6f;

    [Header("种子")]
    [Tooltip("地图种子，0=随机")]
    public int seed = 0;
    [Tooltip("是否在游戏启动时自动生成地图")]
    public bool autoGenerateOnStart = true;

    private void Awake()
    {
        col= GetComponent<Collider2D>();
        bounds=col.bounds;
        if (autoGenerateOnStart)
        {
            GenerateMap();
        }
    }
    private void Start()
    {
        
    }
    public void GenerateMap()
    {    

        if (!ValidateComponents()) return;//验证生成必要组件

        // 基于种子生成随机偏移量（保证相同种子生成相同偏移）
        System.Random rng= new System.Random(seed==0?System.Environment.TickCount:seed);
        if (seed == 0)
        {
            seed = rng.Next();// 随机种子生成后更新显示
            rng = new System.Random(seed);
        }
        float offsetX = (float)rng.NextDouble() * 1000f;
        float offsetY = (float)rng.NextDouble() * 1000f;

        tilemap.ClearAllTiles();

        Vector3Int min = tilemap.WorldToCell(bounds.min);
        Vector3Int max = tilemap.WorldToCell(bounds.max);

        int width = max.x - min.x + 1;
        int height = max.y - min.y + 1;

        for(int x=min.x;x<max.x; x++)
        {
            for(int y=min.y;y<max.y; y++)
            {
                float sampleX = (x + offsetX) * noiseScale;
                float sampleY = (y + offsetY) * noiseScale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

                TileBase tileToBase=GetTileByNoise(noiseValue);//根据噪音 选择瓦片
                Vector3Int tilePos = new Vector3Int(x, y, 0);//获取瓦片位置
                tilemap.SetTile(tilePos,tileToBase);//生成瓦片
            }
        }
        Debug.Log($"地图生成完成，种子：{seed}，使用偏移量：({offsetX:F2}, {offsetY:F2})");
        if (GridManager.Instance == null)
        {
            Debug.LogError("没有网格管理器");
            return;
        }
        GridManager.Instance.BuildGrid();
    }
    public TileBase GetTileByNoise(float noise)
    {
        if (noise <= waterThreshold)
            return waterTile;
        else if (noise <= mudThreshold)
            return mudTile;
        else return grassTile;
    }
    /// <summary>
    /// 验证必要的组件和资源是否完整
    /// </summary>
    private bool ValidateComponents()
    {
        // 自动查找组件（如果未手动拖拽）
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
        if (grid == null && tilemap != null)
            grid = tilemap.GetComponentInParent<Grid>();
        if (tilemap == null || grid == null)
        {
            Debug.LogError("未找到Tilemap或Grid组件！请将脚本挂载到Tilemap对象或手动指定组件。");
            return false;
        }

        // 检查瓦片资源
        if (waterTile == null || mudTile == null || grassTile == null)
        {
            Debug.LogError("请为水、泥地、草地瓦片资源赋值！");
            return false;
        }

        return true;
    }
    /// <summary>
    /// 随机生成新种子（可在编辑器右键菜单调用）
    /// </summary>
    [ContextMenu("Random Seed")]
    public void RandomizeSeed()
    {
        seed = Random.Range(1, int.MaxValue);
        Debug.Log($"新随机种子：{seed}");
    }
    /// <summary>
    /// 使用指定种子重新生成地图
    /// </summary>
    public void GenerateWithSeed(int newSeed)
    {
        seed = newSeed;
        GenerateMap();
    }
}


