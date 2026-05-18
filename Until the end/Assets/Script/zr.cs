using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zr : MonoBehaviour
{
    public List<GameObject> list = new List<GameObject>();
    public LayerMask targetLayer;

    private void Start()
    {
        targetLayer = 1 << gameObject.layer;
        //掩码结构    转换        int型
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            jr();
        if (Input.GetMouseButtonDown(1))
            lk();
    }
    public void jr()
    {
        Collider2D[] target = Physics2D.OverlapCircleAll(transform.position,
                                                             2f,
                                                             targetLayer);
        if (target != null)
            foreach (Collider2D _target in target)
                if(_target.gameObject != this.gameObject) 
                { 
                    list.Add(_target.gameObject);
                    _target.gameObject.SetActive(false);
                }
    }
    public void lk()
    {
        foreach(GameObject _target in list)
        {
            _target.SetActive(true);
            _target.transform.position =new Vector2(transform.position.x, transform.position.y - 0.5f);
        }
        list.Clear();
    }
}
