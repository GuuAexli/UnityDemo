using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UnitCorpse : MonoBehaviour
{
    public float time=30;
    
    void Start()
    {
        StartCoroutine(FadeTo());
    }
    private IEnumerator FadeTo()
    {
        SpriteRenderer sr=GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b,0);

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;
            sr.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}

