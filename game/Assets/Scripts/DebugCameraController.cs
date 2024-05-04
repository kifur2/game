using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float movementSpeed = 10.0f;
    public float fastMovementSpeed = 100.0f;
    public float sensitivity = 0.25f;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
    }

    public void ToggleCamera()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        mainCamera.gameObject.SetActive(!gameObject.activeSelf);
    }
}