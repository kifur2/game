using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera playerCamera;
    private float _xRotation = 0.0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        var mouseX = (input.x * Time.deltaTime) * xSensitivity;
        var mouseY = (input.y * Time.deltaTime) * ySensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80.0f, 80.0f);

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
