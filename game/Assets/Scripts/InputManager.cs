using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInput.OnFootActions _onFootActions;
    private PlayerInput.MenuActions _menuActions;
    private PlayerInput.DebugActions _debugActions;
    public DebugCameraController debugCameraController;

    private PlayerMotor _motor;
    private WeaponSwitch _weaponSwitch;
    private PlayerLook _look;
    private Canvas _canvas;
    private PauseManager _pauseManager;

    public void Awake()
    {
        _playerInput = new PlayerInput();
        _onFootActions = _playerInput.OnFoot;
        _menuActions = _playerInput.Menu;
        _debugActions = _playerInput.Debug;

        _motor = GetComponentInChildren<PlayerMotor>();
        _look = GetComponentInChildren<PlayerLook>();
        _canvas = GetComponentInChildren<Canvas>();
        _weaponSwitch = GetComponentInChildren<WeaponSwitch>();
        _pauseManager = GetComponent<PauseManager>();

        _debugActions.ToggleDebugCamera.performed += ctx => ToggleDebugCamera();
        _menuActions.Pause.performed += ctx => _pauseManager.TriggerPause();
        _onFootActions.Jump.performed += ctx => _motor.Jump();
        _onFootActions.Crouch.performed += ctx => _motor.Crouch();
        _onFootActions.Sprint.performed += ctx => _motor.Sprint();
        _onFootActions.Reload.performed += ctx => GetActiveGun()?.Reload();
        _onFootActions.SwitchWeapon.performed += ctx => _weaponSwitch.SwitchWeapon(ctx.ReadValue<float>());
        _onFootActions.SwitchToGun.performed += ctx => _weaponSwitch.SelectWeapon(0);
        _onFootActions.SwitchToRifle.performed += ctx => _weaponSwitch.SelectWeapon(1);
        _onFootActions.SwitchToPistol.performed += ctx => _weaponSwitch.SelectWeapon(2);
    }

    private void ToggleDebugCamera()
    {
        if (PauseManager.IsPaused) return;

        if (debugCameraController != null)
        {
            debugCameraController.ToggleCamera();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Tell the PlayerMotor to move the player based on the input from the player
        _motor.MovePlayer(_onFootActions.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        //Tell the PlayerLook to process the look based on the input from the player
        _look.ProcessLook(_onFootActions.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        _onFootActions.Enable();
        _menuActions.Enable();
        _debugActions.Enable();
    }

    private void OnDisable()
    {
        _onFootActions.Disable();
        _menuActions.Disable();
        _debugActions.Disable();
    }

    private Gun GetActiveGun()
    {
        return GetComponentInChildren<Gun>();
    }
}