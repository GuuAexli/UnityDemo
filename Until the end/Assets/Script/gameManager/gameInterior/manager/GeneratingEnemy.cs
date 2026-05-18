using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratingEnemy : MonoBehaviour
{
    

    public UnitData[] enemyData;
    //敌人单位数据

    public int nowCost;     //现在点数
    public int Cost=1;      //阶段点数
    public Vector3 tr;      //生成位置
    public float time = 60; //生成间隔

    [SerializeField] private int generatingEnemyNumder=1;
    [SerializeField] private int nowNumder;

    [SerializeField] private Text textInning;
    private void Start()
    {
        StartCoroutine(Available());
        
    }

    IEnumerator Available()
    {
        while (true)
        {
            GameController.Instance.Inning++;
            UIEvent.UpdateInningInfo?.Invoke();
            GameController.Instance.setSupply(Random.Range(0,2));
            nowCost = Cost;

            for (nowNumder = 0; nowNumder < generatingEnemyNumder; nowNumder++)
            {
                int count = Random.Range(0, enemyData.Length);
                if (enemyData[count].costValue <= nowCost)
                {

                    nowCost -= enemyData[count].costValue;
                    Instantiate(enemyData[count].prefab, tr = new Vector3(Random.Range(-30, -8), Random.Range(30, 31), 0), Quaternion.identity);
                    Debug.Log(enemyData[count].prefabName);

                    yield return new WaitForSeconds(5);
                }
                else nowNumder--;
            }
            Cost++;
            nowNumder = 0;

            if (GameController.Instance.Inning % 4 == 0)
                generatingEnemyNumder++;

            if (GameController.Instance.Inning % 10 == 0 && time > 30)
                time -= 5;

            yield return new WaitForSeconds(time);
        }
    }

}
