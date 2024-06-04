using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterDeathManager : MonoBehaviour
{

    public static bool IsDead;
    [SerializeField] private GameObject deathMenu;

    private void Start()
    {
        deathMenu.SetActive(false);
    }

    public void TriggerDeath()
    {
        if (IsDead) return;
        
        deathMenu.SetActive(true);
        Time.timeScale = 0f;
        IsDead = true;
    }

    public void RestartGame()
    {
        deathMenu.SetActive(false);
        Time.timeScale = 1f;
        IsDead = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        IsDead = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        IsDead = false;
        Application.Quit();
    }
}
