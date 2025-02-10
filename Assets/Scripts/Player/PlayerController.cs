using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
    Controller2D controller;


    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    float moveSpeed = 12;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Vector2 moveInput;

    bool isJump = false;
    public bool isDownJump = false;


    private void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2*maxJumpHeight)/Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity)*timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity)*minJumpHeight);
        Debug.Log($"Gravity :{gravity}, JumpVelocity : {maxJumpVelocity}");

    }

    private void FixedUpdate()
    {


        Vector2 input = new Vector2(moveInput.x, moveInput.y);

        if (isJump && controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }
        if (!isJump)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        // 가속 및 감속 부분
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelarationTimeGrounded: accelarationTimeAirborne);

        //velocity.x = input.x*moveSpeed;

        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime,input,isDownJump);

        // 중력 초기화
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (moveInput.y == -1)
            {
                isDownJump = true;
            }
            else
            {
                isJump = true;
            }
        }
        else
        {
            isJump = false;
            isDownJump = false;
        }
    }

    
}
