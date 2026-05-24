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
                
                data.Spawn();//统一生成
            }
            else Debug.Log("补给不足");
        else Debug.Log("管理器错误");
    }


}
