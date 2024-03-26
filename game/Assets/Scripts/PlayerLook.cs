using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera playerCamera;
    private float xRotation = 0.0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public void processLook(Vector2 input)
    {
        float mouseX = (input.x * Time.deltaTime) * xSensitivity;
        float mouseY = (input.y * Time.deltaTime) * ySensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
