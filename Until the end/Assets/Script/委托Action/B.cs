using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B : MonoBehaviour
{
    public Action a;//相当于函数组 执行时 按顺序 依次 执行存入的函数 可重复存入同一个函数
    public int b = 0;
    private void Start()
    {
        a += A;
    }
    void A()
    {
        Debug.Log("1\t"+b);

    }
}
