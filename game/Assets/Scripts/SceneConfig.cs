using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConfig : MonoBehaviour
{
    public string backgroundMusicName;
    public float volume = 1f;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(backgroundMusicName, volume);
        }
    }
}