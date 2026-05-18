using UnityEngine;
using UnityEngine.UI;

public class SumBtnSpawnController : MonoBehaviour
{
    [Header("生成设置")]
    //public Transform spawnPoint;//生成位置[(-29)-(-9),2-5]
    public Data[] availableUnits;//所有可用单位数据

    [Header("需要生成的按钮预制体")]
    public Button subButtonPrefab;//按钮预制体
    [Header("按钮生成位置")]
    public Transform subButtonPos;//生成位置

    private void Start()
    {
        foreach(var unitData in availableUnits)
        {
            Button newBtn = Instantiate(subButtonPrefab, subButtonPos);
            //生成子按钮                 子按钮的预制体  子按钮挂载点（容器）
            newBtn.GetComponentInChildren<Text>().text = unitData.prefabName;
            //获取更改生成的按钮内文本文字
            //newBtn.image.sprite = unitData.buttonIcon;
            //获取生成相应的图片

            //绑定点击事件
            newBtn.onClick.AddListener(() => SpawnPrefab(unitData));
            //为每一个按钮绑定
        }//历遍每个数据 为他们生成按钮和监听事件
    }
    public void SpawnPrefab(Data data)
                //点击时每一个按钮上都有不同的数据但有共同的形参
    {
        if (data == null) return;

        if (GameController.Instance != null)
            if (GameController.Instance.Supply >= data.costValue)
            {
                GameController.Instance.setSupply(-data.costValue);
                //统一生成
                if (data is UnitData)
                {
                    SpawnUnit(data);
                    return;
                }//如果是单位
                if(data is WeaponData)
                {
                    Instantiate(data.prefab);
                }
                if(data is SupportData)
                {
                    SpawnSupport(data);
                    return;
                }
            }
            else Debug.Log("补给不足");
        else Debug.Log("管理器错误");
    }
    void SpawnUnit(Data unit)
    {
        Debug.Log("生成" + unit.prefabName);
        Vector3 spawnPoint = new Vector3(Random.Range(-30f, -8f), -10, 0);
        GameObject newUnit = Instantiate(unit.prefab, spawnPoint, Quaternion.identity);
        //生成  数据实例    生成位置  默认角度
        newUnit.GetComponent<UnitAttribute>().SetUnitMovePos(new Vector2(newUnit.transform.position.x, newUnit.transform.position.y + 5));
        //生成后向前移动
    }
    void SpawnSupport(Data support)
    {
        Instantiate(support.prefab);
        Debug.Log(support.name);
    }
}
