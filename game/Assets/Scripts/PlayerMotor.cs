using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isSprinting;
    private bool _isCrouching;
    private bool _lerpCrouch;
    private float _crouchTimer;
    public float walkSpeed = 5.0f;
    private float _speed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float jumpHeight = 5.0f;
    private readonly float _gravity = -9.81f;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = _controller.isGrounded;
        if (!_lerpCrouch) return;
        _crouchTimer += Time.deltaTime;
        var p = _crouchTimer;
        p *= p;
        _controller.height = Mathf.Lerp(_controller.height, _isCrouching ? 1 : 2, p);
        if (!(p > 1)) return;
        _lerpCrouch = false;
        _crouchTimer = 0;
    }

    public void MovePlayer(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        _controller.Move(transform.TransformDirection(moveDirection) * _speed * Time.deltaTime);
        _velocity.y += _gravity * Time.deltaTime;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _controller.Move(_velocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (PauseManager.IsPaused) return;

        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -_gravity);
        }
    }

    public void Crouch()
    {
        if (PauseManager.IsPaused) return;

        _isCrouching = !_isCrouching;
        _crouchTimer = 0;
        _lerpCrouch = true;
    }

    public void Sprint()
    {
        if (PauseManager.IsPaused) return;

        _isSprinting = !_isSprinting;
        _speed = _isSprinting ? sprintSpeed : walkSpeed;
    }
}