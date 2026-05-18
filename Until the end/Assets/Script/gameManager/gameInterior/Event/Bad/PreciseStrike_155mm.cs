using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PreciseStrike_155mm : SpecificEvent
{
    [SerializeField] private GameObject shell;
    [SerializeField] private float delay;
    [SerializeField] private List<UnitAttribute> targetList = new List<UnitAttribute>();
    [SerializeField] private UnitAttribute targetUnit;
    [SerializeField] private Vector2 spawnPos;

    private void Start()
    {
        StartCoroutine(SpawnStrike());
    }
    private void Update()
    {
        if(targetUnit==null)
            SelectedTarget();
    }
    IEnumerator SpawnStrike()
    {
        //yield return new WaitForSeconds(delay);
        yield return new WaitForSeconds(0.2f);
        SelectedTarget();
        yield return new WaitForSeconds(delay);
        SpawnShell();
        Debug.Log("射击完成");
        Destroy(gameObject);
    }
    private void SpawnShell()
    {
        Instantiate(shell, spawnPos,Quaternion.identity);
    }
    private void SpawnPos(UnitAttribute unit)
    {
        Debug.Log(unit);
        if (unit == null)
        {
            Destroy(gameObject);
            return;
        }
        spawnPos = new Vector2(unit.transform.position.x+Random.Range(-1f,1f),
                               unit.transform.position.y+Random.Range(-1f,1f));
    }
    private void SelectedTarget()
    {
        Bounds bounds = EventManager.Instance._bounds;
        Vector2 pos1 = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 pos2 = new Vector2(bounds.max.x, bounds.max.y);

        Collider2D[] col = Physics2D.OverlapAreaAll(pos1,
                                pos2,
                                LayerMask.GetMask("unit"));
        //检测范围
        foreach (Collider2D col2 in col)
        {
            if (col2.gameObject.tag == "blue_tag")
            {
                UnitAttribute unit = col2.gameObject.GetComponent<UnitAttribute>();
                if (unit != null)
                    targetList.Add(unit);
            }
        }//获取可能目标
        Debug.Log(targetList.Count);
        int maxCost = 0;
        foreach(UnitAttribute target in targetList)
        {
            if (target.unitData.costValue >= maxCost)
            {
                maxCost = target.unitData.costValue;
                targetUnit = target;
            }
        }
        SpawnPos(targetUnit);
        //Debug.Log("目标：" + targetUnit);
    }
}
