using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds;
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySfxAtPosition(string name, Vector3 position, float volume)
    {
        AudioSource audioSource = Instantiate(sfxSource, position, Quaternion.identity);
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found.");
        }
        else
        {
            audioSource.clip = s.clip;
            audioSource.volume = volume;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }
    }
    public void PlayMusic(string name, float volume)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found.");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = volume;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSfxVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
    }
}