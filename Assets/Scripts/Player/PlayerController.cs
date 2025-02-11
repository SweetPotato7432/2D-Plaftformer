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

    bool isDownJump = false;

    Vector2 directionalInput;


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

        // 가속 및 감속 부분
        CalculateVelocity();
        controller.Move(velocity * Time.fixedDeltaTime, directionalInput, isDownJump);

        // 중력 초기화
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y*-gravity*Time.fixedDeltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown(bool isJump, bool isDownJump)
    {
        this.isDownJump = isDownJump;

        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                
                if(directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                {// 오를 수 없는 경사로에서 점프 불가능
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;

            }
        }
    }

    public void OnJumpInputUp(bool isJump, bool isDownJump)
    {
        this.isDownJump = isDownJump;

        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }




    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);

        velocity.y += gravity * Time.fixedDeltaTime;
    }
    
}
