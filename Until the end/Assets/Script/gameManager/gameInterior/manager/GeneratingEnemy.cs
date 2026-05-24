using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneratingEnemy : MonoBehaviour
{
    

    public List<UnitData> enemyData=new List<UnitData>();//所有单位组
    public List<UnitData> generatingList=new List<UnitData>();//可以生成组
    //敌人单位数据
    public Bounds bounds;//生成范围通过触发器
    public Collider2D col;//
    public List<Vector2Int> generatingPos;

    public int currentCost=0;     //现在点数
    public int Cost=1;      //阶段点数
    public float time = 60; //生成间隔

    [SerializeField] private int generatingEnemyNumder=1;
    [SerializeField] private int currentNumder;

    [SerializeField] private Text textInning;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        bounds = col.bounds;
        
    }
    private void Start()
    {
        GetGeneratingPos();
        StartCoroutine(Available());        
    }
    private void CanGeneratingUnit()
    {
        generatingList.Clear();

        generatingList=enemyData.Where(Unit=>Unit.costValue<=currentCost).ToList();

        if (generatingList.Count == 0) 
        { Debug.Log("现在花费" + currentCost + "没有可以生成的单位"); }

    }
    IEnumerator Available()
    {
        while (true)
        {
            GameController.Instance.Inning++;
            UIEvent.UpdateInningInfo?.Invoke();
            GameController.Instance.setSupply(Random.Range(0,2));
            currentCost += Cost;

            for (currentNumder = 0; currentNumder < generatingEnemyNumder; currentNumder++)
            {
                CanGeneratingUnit();
                if (generatingList.Count == 0) break;//没有可以生成单位 

                int value = Random.Range(0, generatingList.Count);

                currentCost -= enemyData[value].costValue;
                int a=Random.Range(0,generatingPos.Count);

                Vector3 pos = GridManager.Instance.CellToWorld(generatingPos[a]);
                Instantiate(enemyData[value].prefab, pos,Quaternion.identity);

                Debug.Log(enemyData[value].prefabName+"生成位置"+pos);

                yield return new WaitForSeconds(5);
            }//生成数量
            Cost++;
            currentNumder = 0;

            if (GameController.Instance.Inning % 4 == 0)
                generatingEnemyNumder++;//每4回合 生成数量+1

            if (GameController.Instance.Inning % 10 == 0 && time > 30)
                time -= 5;//每10回合 等待时间-5秒 最少30秒

            yield return new WaitForSeconds(time);
        }
    }
    private void GetGeneratingPos() 
    {
        GridManager gm = GridManager.Instance;
        Vector2Int min = gm.WorldToCell(bounds.min);
        Vector2Int max=gm.WorldToCell(bounds.max);

        for(int x=min.x;x<max.x;x++)
        {
            for(int y = min.y; y < max.y; y++)
            {
                Vector2Int cellPos= new Vector2Int(x, y);
                if (gm.IsWalkable(cellPos))
                {
                    generatingPos.Add(cellPos);
                }
            }
        }
    }
}
