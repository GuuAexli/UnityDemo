using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarStrikes_80mm : SpecificEvent
{
    [SerializeField] private GameObject Shell;//炮弹
    [SerializeField] private int number;//数量
    [SerializeField] private float delay;//延迟 开始前准备时间
    [SerializeField] private float interval;//间隔 每次间隔

    private void Start()
    {
        StartCoroutine(SpawnStrikes()) ;
    }
    IEnumerator SpawnStrikes()
    {
        yield return new WaitForSeconds(delay);

        for(int currentNumber = 0;currentNumber < number; currentNumber++)
        {
            yield return new WaitForSeconds(Random.Range(interval-0.2f,interval+0.2f));
            Instantiate(Shell, SpawnPos(), Quaternion.identity);
        }
        Destroy(gameObject);
    }
    private Vector2 SpawnPos()
    {
        return new Vector2(Random.Range(EventManager.Instance._bounds.min.x,EventManager.Instance._bounds.max.x),
                            Random.Range(EventManager.Instance._bounds.min.y,EventManager.Instance._bounds.max.y));
    }

}
