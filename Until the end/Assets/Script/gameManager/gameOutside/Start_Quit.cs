using UnityEngine;
using UnityEngine.SceneManagement;
public class Start_Quit : MonoBehaviour
{
    //需要存在场景中
    //并将挂载这个脚本的物体拖入需要的按钮的OnClick上
    //选择对应的脚本
    //选择载入场景选项 输入对应的 场景名
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Quit()
    {
        Debug.Log("1");
        Application.Quit();
        Debug.Log("2");
    }
}
