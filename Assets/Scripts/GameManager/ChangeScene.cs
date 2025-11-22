using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] Animator UIAnim;
    [SerializeField] Animator WorldUIanim;
    public void LoadGame()
    {
        SceneManager.LoadScene(0);
       if(Time.timeScale == 0) Time.timeScale = 1;
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(1);
    }
    public void StartGame()
    {
        StartCoroutine(StartChar());
    }
    public void Back()
    {
        StartCoroutine(back2Menu());
    }
    IEnumerator StartChar()
    {
        UIAnim.Play("ToChar");
        yield return new WaitForSeconds(0.5f);
        WorldUIanim.Play("CharSelect");
    }
    IEnumerator back2Menu()
    {
        WorldUIanim.Play("BackToMain");
        yield return new WaitForSeconds(0.5f);
        UIAnim.Play("MenuStart");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
}