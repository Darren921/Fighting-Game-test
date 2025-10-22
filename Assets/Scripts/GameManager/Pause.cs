using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        PauseManager.Instance?.SetPaused(true);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        PauseManager.Instance?.SetPaused(false);
    }
}