using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterDeathManager : MonoBehaviour
{
    private static bool _isDead;
    private const string DeathAudioClipName = "Death";
    [SerializeField] private GameObject deathMenu;

    private void Start()
    {
        deathMenu.SetActive(false);
    }

    public void TriggerDeath()
    {
        if (_isDead) return;

        deathMenu.SetActive(true);
        Time.timeScale = 0f;
        _isDead = true;


        if (AudioManager.Instance)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySfxAtPosition(DeathAudioClipName, gameObject.transform.position, 1f);
        }
    }

    public void RestartGame()
    {
        deathMenu.SetActive(false);
        Time.timeScale = 1f;
        _isDead = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        _isDead = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        _isDead = false;
        Application.Quit();
    }
}