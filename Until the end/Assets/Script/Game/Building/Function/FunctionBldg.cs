using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class FunctionBldg : Building
{
    public Collider2D col;
    public List<Vector2Int> occupiedList=new List<Vector2Int>();
    public override void Complete(Vector3 pos)
    {
        base.Complete(pos);
        StartCoroutine(AddCost());
        col= GetComponent<Collider2D>();

        if (col == null) {Debug.Log(data.prefabName +"建造没有碰撞触发器"); return; }
        occupiedList = GridManager.Instance.GetOccupiedGrid(col);
        GridManager.Instance.UpdateOccupied(new List<Vector2Int>(), occupiedList);
    }
    private void OnDestroy()
    {
        if (occupiedList == null) return;
        GridManager.Instance.UpdateOccupied(occupiedList,new List<Vector2Int>());
    }
    private IEnumerator AddCost()
    {
        while (true)
        {
            yield return new WaitForSeconds(effectValue_F);
            GameController.Instance.setCost(effectValue_I);
        }
    }

}//功能建筑
