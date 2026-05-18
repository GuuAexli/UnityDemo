using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDragController : MonoBehaviour
{
    [Header("配置")]
    public Transform weaponsMenu;//子按钮挂载点（容器）
    public Button weaponButtonPrefab;//武器子按钮预制体
    public WeaponData[] weaponDatas;//所以武器数据
    private void Start()
    {
        foreach(WeaponData data in weaponDatas)
        {
            Button newBtn = Instantiate(weaponButtonPrefab, weaponsMenu);
            newBtn.GetComponentInChildren<Text>().text = data.name;
            newBtn.onClick.AddListener(() => SpawnWeapon(data));
        }
    }
    void  SpawnWeapon(WeaponData data)
    {
        if (data == null) return ;

        GameObject selectedWeapon = Instantiate(data.prefab);
        selectedWeapon.GetComponent<Weapon>().IsSelected=true;
        
    }

}
