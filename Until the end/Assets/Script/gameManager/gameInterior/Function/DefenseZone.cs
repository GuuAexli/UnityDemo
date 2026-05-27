using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseZone : MonoBehaviour
{
    public GridManager gm;
    public Collider2D col;
    public Bounds bounds;
    public List<Vector2Int> walkableCell=new List<Vector2Int>();

    public List<UnitData> unitList = new List<UnitData>();
    private void Awake()
    {
        gm = GridManager.Instance;
        col= GetComponent<Collider2D>();
        bounds = col.bounds;
    }
    public void Start()
    {
        CollectWalkableCell();
        StartCoroutine(SpawnDefenseUnit());
    }
    public void CollectWalkableCell()
    {
        Vector2Int min = gm.WorldToCell(bounds.min);
        Vector2Int max=gm.WorldToCell(bounds.max);

        for(int x=min.x;x<max.x;x++)
        {
            for(int y = min.y; y < max.y; y++)
            {
                Vector2Int cellPos= new Vector2Int(x,y);
                if(gm.IsWalkable(cellPos,null))
                {
                    walkableCell.Add(cellPos); 
                }
            }
        }
    }
    public IEnumerator SpawnDefenseUnit()
    {
        yield return new WaitForSeconds(1);
        for(int i=0;i<4;i++)
        {
            if (GameController.Instance.cost <= 0) yield break; 
            List<UnitData> list = new List<UnitData>();
            foreach (UnitData unit in unitList)
            {
                if (unit.costValue <= GameController.Instance.cost)
                {
                    list.Add(unit);
                }
            }
            if(list.Count == 0) yield break;
            int a = Random.Range(0, list.Count);
            list[a].Spawn();
            GameController.Instance.setCost(-list[a].costValue);
            yield return new WaitForSeconds(1f);
        }
    }
    public Vector3 GetRandomDefensePos()
    {
        int a=Random.Range(0,walkableCell.Count);
        Vector3 pos = gm.CellToWorld(walkableCell[a]);
        return pos;


    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        UnitAttribute unit = col.GetComponent<UnitAttribute>();
        if (unit != null && unit.faction==Faction.Red)
        {

            ManagerEvent.DefenseValueLoss(unit.unitData.destroyValue);
            Destroy(unit.gameObject);
            Debug.Log("一个敌人突破了防线");
        }
    }
}
