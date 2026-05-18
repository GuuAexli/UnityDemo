using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEvent : MonoBehaviour
{
    [Range(0,300)]public float eventTime = 60f;
    [SerializeField] private int eventNumder;
    //默认60f 实际受Range影响（0f,300f）
    public bool gameOver = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Event());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Event()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(eventTime);

            eventNumder = Random.Range(1, 10);
            Debug.Log(eventNumder);
            switch (eventNumder) {
                case 1:
                case 2: hapEvent(); break;
                case 3:
                case 4: 
                case 5: 
                case 6: badEvent();break;
                case 7: 
                case 8: 
                case 9: 
                case 10:medEvent(); break;

            }
            
        }
    }
    public void hapEvent()
    {
        Debug.Log("Hap");
        eventNumder = Random.Range(0, 5);
        Debug.Log(eventNumder);
    }
    public void badEvent()
    {
        Debug.Log("bad");
        eventNumder = Random.Range(0, 9);
        Debug.Log(eventNumder);
    }
    public void medEvent()
    {
        Debug.Log("med");
        eventNumder = Random.Range(0, 9);
        Debug.Log(eventNumder);
    }
}
