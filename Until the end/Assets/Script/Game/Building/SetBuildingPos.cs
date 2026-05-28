using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBuildingPos : MonoBehaviour
{
    public Building building;
    public List<Vector2Int> occupied=new List<Vector2Int>();
    public SpriteRenderer sr;
    Collider2D col;
    public bool can=true;
    // Update is called once per frame
    private void Start()
    {
        building = GetComponent<Building>();
        if (building == null){ Destroy(gameObject);return; }
        col = GetComponent<Collider2D>();
        sr= GetComponent<SpriteRenderer>();
        if (!building.complete)
        { UIEvent.OnMessageText?.Invoke("选择放置位置"); }
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

        if (col != null)
        {
            occupied = GridManager.Instance.GetOccupiedGrid(col);
            foreach(Vector2Int cell in occupied)
            {
                if (!GridManager.Instance.IsWalkable(cell))
                {
                    sr.color = Color.red;
                    can = false;
                    break;
                }
                else
                {
                    can = true;
                    sr.color = Color.white;
                }
            }
        }
        if(Input.GetMouseButtonDown(0))
        {   

            if(cellPos == null ||!can) 
            { 
                UIEvent.OnMessageText?.Invoke("选择位置无法建造");Destroy(gameObject);
                GameController.Instance.setCost(GetComponent<Building>().data.costValue);
                return; 
            }
            
            building.Complete(pos);
            UIEvent.OnMessageText?.Invoke("完成放置");
        }
        if(Input.GetMouseButton(1))
        {
            UIEvent.OnMessageText?.Invoke("取消建造");
            Destroy(gameObject);
            GameController.Instance.setCost(GetComponent<Building>().data.costValue);
            return;
        }
    }
}
