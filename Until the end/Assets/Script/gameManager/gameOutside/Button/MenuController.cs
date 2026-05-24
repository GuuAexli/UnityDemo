using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [System.Serializable]
    //使下面组可在控制器里面调整序列
    //
    public class MenuPair
    {
        //按下主按钮 显示/隐藏 子按钮

        public Button mainButton;
        //主要按钮
        public GameObject subMenuPos;
        //子按钮
    }
    public MenuPair[] menuPairs;
    //菜单对
    //记录拥有子菜单的按钮
    private GameObject currentSubMenu;
    //选择的按钮
    //[Header("单位生成引用")]
    //public UnitSpawnController unitSpawner;

    private void Start()
    {
        foreach(var pair in menuPairs){
            //历遍每一个菜单对
            pair.mainButton.onClick.AddListener(() => ToggleSubMenu(pair.subMenuPos));
            //为每一个主按钮添加事件监听器
            //当主按钮被点击就执行（）=>后面的函数或代码    按下后执行函数并给予自己附属的子菜单
            pair.subMenuPos.SetActive(false);
            //确保开始时子按钮没有被打开
        }
    }
    private void OnDestroy()
    {
        foreach(var pair in menuPairs)
        {
            pair.mainButton.onClick.RemoveAllListeners();
        }
    }
    void ToggleSubMenu(GameObject targetMenu)
        //事件监听器触发 会给予被按按的子按钮
    {
        if (currentSubMenu == targetMenu) 
        {
            targetMenu.SetActive(false);
            //关闭当前打开的子菜单
            currentSubMenu = null;
            //使选择为空
        }//如果之前选择的子菜单是现在的目标菜单
        else 
        {
            if (currentSubMenu != null)
                //如果选择的菜单不为空
                currentSubMenu.SetActive(false);
                //之前选择的关闭
            targetMenu.SetActive(true);
            //打开选择的菜单的子菜单
            currentSubMenu = targetMenu;
            //替换之前选择的菜单
            //RectTransform btnRect=targetMenu.transform.parent.GetComponent<RectTransfomr>()
        }//与选择打开的不同
    }//切换子按钮
}
