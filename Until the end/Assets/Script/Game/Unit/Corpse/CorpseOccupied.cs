using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseOccupied : MonoBehaviour
{
    public List<Vector2Int> occupied=new List<Vector2Int>();
    void Start()
    {
         Collider2D col=GetComponent<Collider2D>();
        if (col == null) return;
        occupied = GridManager.Instance.GetOccupiedGrid(col);
        GridManager.Instance.UpdateOccupied(new List<Vector2Int>(), occupied);
          
    }//设置载具尸体占用
    private void OnDestroy()
    {
        if ( occupied != null )
            GridManager.Instance.UpdateOccupied(occupied, new List<Vector2Int>());
    }

}
