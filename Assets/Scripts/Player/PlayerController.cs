using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
    Controller2D controller;


    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    float moveSpeed = 12;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Vector2 moveInput;

    bool isJump = false;
    

    private void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2*jumpHeight)/Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity)*timeToJumpApex;
        Debug.Log($"Gravity :{gravity}, JumpVelocity : {jumpVelocity}");

    }

    private void FixedUpdate()
    {
        // 중력 초기화
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(moveInput.x, moveInput.y);

        if (isJump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        // 가속 및 감속 부분
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelarationTimeGrounded: accelarationTimeAirborne);

        //velocity.x = input.x*moveSpeed;

        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);


    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            isJump = true;
        }
        else
        {
            isJump = false;
        }
    }


    
}
