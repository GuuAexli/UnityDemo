using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Start_Quit : MonoBehaviour
{

    public Button start;
    public Button quit;
    private void Start()
    {
        LoadScene();
        Quit();
    }
    public void LoadScene()
    {
        start.onClick.AddListener(() => SceneManager.LoadScene("Game")); 
    }
    public void Quit()
    {
        quit.onClick.AddListener(() => Application.Quit());
    }
}
