using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneSetup : MonoBehaviour
{
    public AudioManager audioManagerPrefab;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(audioManagerPrefab);
        }
    }
}