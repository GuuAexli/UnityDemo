using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBuildingPos : MonoBehaviour
{
    public Building building;

    // Update is called once per frame
    private void Start()
    {
        building = GetComponent<Building>();
        if (building == null )
            Destroy(gameObject);
    }
    void Update()
    {
        if (building.complete)
            { this.enabled = false;return; }
        SelectorPos();
    }
    public void SelectorPos()
    {
        GridManager gm= GridManager.Instance;

        Vector2 mousePos=Camera.main.ScreenToWorldPoint( Input.mousePosition );
        Vector2Int cellPos = gm.WorldToCell(mousePos);
        Vector3 pos = gm.CellToWorld(cellPos);
        gameObject.transform.position = pos;

        if(Input.GetMouseButtonDown(0))
        {   
            if(cellPos == null &&!gm.IsWalkable(cellPos)) 
                { Debug.Log("选择位置无法建造");Destroy(gameObject); }
            

            building.Complete(pos);
        }
        if(Input.GetMouseButton(1))
        {
            Debug.Log("取消建造");
            Destroy(gameObject);
            return;
        }
    }
}
