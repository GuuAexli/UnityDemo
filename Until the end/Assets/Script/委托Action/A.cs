using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A : MonoBehaviour
{
    public B _B;
    public int a=0;
    private void Start()
    {
        _B.a += Add;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            _B.a.Invoke();
        }
    }
    private void OnEnable()
        // 实例化时 执行
    {
        _B.a?.Invoke();
    }
    private void OnDisable()
        //未/取消 实例化时 执行
    {
        _B.a -= Add;
    }
    public void Add()
    {
        _B.b = a;
        Debug.Log("2\t"+ ++a);
    }
}


