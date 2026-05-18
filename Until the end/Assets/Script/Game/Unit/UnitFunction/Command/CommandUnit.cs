using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : MonoBehaviour
{
    void Start()
    {
        GameController.Instance.canCommandValue+=1;
    }
    private void OnDestroy()
    {
        GameController.Instance.canCommandValue -= 1;
    }

}
