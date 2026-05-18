using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"[Weapon] Awake: {transform.position}");
    }

    private void OnEnable()
    {
        Debug.Log($"[Weapon] OnEnable: {transform.position}");
    }

    private void Start()
    {
        Debug.Log($"[Weapon] Start: {transform.position}");
    }

    private void Update()
    {
        // ÷ª¥Ú”°«∞10÷°£¨±‹√‚À¢∆¡
        if (Time.frameCount <= 10)
            Debug.Log($"[Weapon] Update Frame {Time.frameCount}: {transform.position}");
    }

    private void LateUpdate()
    {
        if (Time.frameCount <= 10)
            Debug.Log($"[Weapon] LateUpdate Frame {Time.frameCount}: {transform.position}");
    }
}
