using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private bool lerpCrouch;
    private float crouchTimer;
    public float walkSpeed = 5.0f;
    private float speed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float jumpHeight = 5.0f;
    private float gravity = -9.81f;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer;
            p*=p;
            if (isCrouching)
            {
                controller.height = Mathf.Lerp(controller.height, 1, p);
            }
            else
            {
                controller.height = Mathf.Lerp(controller.height, 2, p);
            }
            if(p>1)
            {
                lerpCrouch = false;
                crouchTimer = 0;
            }
        }
    }
    
    public void MovePlayer(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        controller.Move(velocity * Time.deltaTime);
    }
    
    public void Jump()
    {
        if(isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -gravity);
        }
    }

    public void Crouch()
    {
        isCrouching = !isCrouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }
    
    public void Sprint()
    {
        isSprinting = !isSprinting;
        if (isSprinting)
            speed = sprintSpeed;
        else
            speed = walkSpeed;
    }
}
