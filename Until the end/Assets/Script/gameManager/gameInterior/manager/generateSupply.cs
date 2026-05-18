using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateSupply : MonoBehaviour
{
    public float time = 30f;
    //生成时间
    public GameObject supply;
    //生成物体
    [SerializeField] float X;
    //生成物体的x位置
    [SerializeField] float Y;
    //生成物体的Y位置
    public Vector3 tr;
    [Header("己方半场生成概率")]
    [Range(0, 1)] public float highProbability = 0.7f;
    //在控制面板生成滑动条控制  高概率  的值[0f,1f]
    //帮助测试
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Generate());
        //协程  ！不会停止
        //启动协程   执行的函数（重复）
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Generate()
    //启动协程的关键字
    {
        while (true)
        //一直为真
        {
            //Debug.Log("start");
            if (Random.value <= highProbability)
            //随机生成一个临时数 判断是否处于高概率事件
            {
                //高概率
                yield return new WaitForSeconds(time);
                //协程         执行等待    30f(受time影响）
                //Debug.Log("h");
                Y = Random.Range(8f, 1f);
                //随机
            }
            else
            //低概率
            {
                yield return new WaitForSeconds(time);
                //执行等待                     30f(受time影响）
                //Debug.Log("d");
                Y = Random.Range(24f, 1f);
                //随机v}
            }
            X = Random.Range(-29f, -8f);
            tr = new Vector3(X, Y, 0);
        //X,Y的float转换vector
         Instantiate(supply, tr, Quaternion.identity);
        //生成       物体   位置（V3)   方向
            
        }
    }

}
