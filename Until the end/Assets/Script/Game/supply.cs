using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class supply : MonoBehaviour
{
    public Collider2D _cd;
    public Rigidbody2D _rd;
    [Tooltip("需要检测的标签")]
    public string targetTag = "blue_tag";
    
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag)) {
            
            returnController();
            Debug.Log("supply+1");
        }
        Destroy(gameObject);
    }
    private void returnController()
    {
        if ((GameController.Instance!=null))
        {
            GameController.Instance.setSupply(1);
        }
    }
}
