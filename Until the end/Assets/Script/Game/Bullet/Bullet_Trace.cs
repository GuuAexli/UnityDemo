using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Trace : MonoBehaviour
{
    private void Start()
    {
        Invoke("destory", 10);
    }
    public void destory ()
    {
        Destroy(gameObject);
    }
}
