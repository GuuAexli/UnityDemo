using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSoldier : MonoBehaviour
{
    [SerializeField]private  GameObject soldier;
    [SerializeField] private float x, y;
    // Start is called before the first frame update
    public void generate()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.Supply >= 1)
            {
                GameController.Instance.setSupply(-1);
                x = Random.Range(-29f, -8f);
                y = Random.Range(2f, 4f);
                Vector3 tr = new Vector3(x, y, 0);
                GameObject newSoldier = Instantiate(soldier, tr, Quaternion.identity);
            }
            else Debug.Log("补给不足");
        }
        else Debug.Log("管理器不存在");
    }
}
