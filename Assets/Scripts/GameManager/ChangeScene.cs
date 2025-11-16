using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(1);
    }


    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}