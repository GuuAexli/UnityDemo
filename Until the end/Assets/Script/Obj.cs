using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour
{
    public bool Y=false;
    // Start is called before the first frame update
    public void select(bool isSelect)
    {
        Y = isSelect;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
