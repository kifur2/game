using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused;
    [SerializeField] private GameObject pauseMenu;

    // Start is called before the first frame update
    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void TriggerPause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        Application.Quit();
    }
}