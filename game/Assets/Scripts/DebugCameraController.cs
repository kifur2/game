using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCameraController : MonoBehaviour
{
    public Camera mainCamera;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ToggleCamera()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        mainCamera.gameObject.SetActive(!gameObject.activeSelf);
    }
}