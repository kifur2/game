using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFootActions;
    private PlayerMotor motor;
    private PlayerLook look;
    
    void Awake()
    {
        playerInput = new PlayerInput();
        onFootActions = playerInput.OnFoot;
        
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        
        onFootActions.Jump.performed += ctx => motor.Jump();
        onFootActions.Crouch.performed += ctx => motor.Crouch();
        onFootActions.Sprint.performed += ctx => motor.Sprint();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Tell the PlayerMotor to move the player based on the input from the player
        motor.MovePlayer(onFootActions.Movement.ReadValue<Vector2>());
    }
    
    private void LateUpdate()
    {
        //Tell the PlayerLook to process the look based on the input from the player
        look.processLook(onFootActions.Look.ReadValue<Vector2>());
    }
    
    private void OnEnable()
    {
        onFootActions.Enable();
    }
    
    private void OnDisable()
    {
        onFootActions.Disable();
    }
}
