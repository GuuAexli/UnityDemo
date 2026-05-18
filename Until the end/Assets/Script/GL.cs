using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("0");
            Vector2 mosPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mosPos, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("1");
                Obj objHit = hit.collider.GetComponent<Obj>();
                if (objHit != null)
                {
                    Debug.Log("2");
                    objHit.select(true);
                }
            }
        }
    }
}
