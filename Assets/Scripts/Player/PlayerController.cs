using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
    Controller2D controller;

    Ghost ghost;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    float moveSpeed = 12;

    public float dashDistance = 4f;
    public float timeToDashApex = .1f;

    float dashVelocity;

    int maxJumpCount = 2;
    public int currentJumpCount =0;

    float gravity;
    float defaultGravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    public bool isDownJump = false;
    bool isDashing = false;

    Vector2 directionalInput;

    [Header ("MeleeAttack")]
    public Vector2 meleeBoxSize;
    Vector2 meleeBoxPosition;


    private void Start()
    {
        controller = GetComponent<Controller2D>();
        ghost = GetComponent<Ghost>();

        defaultGravity = -(2*maxJumpHeight)/Mathf.Pow(timeToJumpApex, 2);
        gravity = defaultGravity;
        maxJumpVelocity = Mathf.Abs(gravity)*timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity)*minJumpHeight);

        dashVelocity = dashDistance/timeToDashApex;

        Debug.Log($"Gravity :{gravity}, JumpVelocity : {maxJumpVelocity}");

    }
    private void FixedUpdate()
    {
        // 가속 및 감속 부분
        CalculateVelocity(moveSpeed);
        // Controller2D로 이동 및 변수 전달
        controller.Move(velocity * Time.fixedDeltaTime, directionalInput, isDownJump);

        //CalculateVelocity(dashSpeed);
        //controller.Dash(velocity * Time.fixedDeltaTime, directionalInput);
        ////onDash = false;



        if (controller.collisions.below)
        {
            currentJumpCount = 0;
        }

        // 중력 초기화
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.fixedDeltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        // 사각형의 중심 위치
        meleeBoxPosition = new Vector2(transform.position.x + 1f * directionalInput.x, transform.position.y);


    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown(bool isJump, bool isDownJump)
    {
        this.isDownJump = isDownJump;

        if (!isDownJump&&currentJumpCount < maxJumpCount)
        {
            if (controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {

                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    {// 오를 수 없는 경사로에서 점프 불가능
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else
                {
                    currentJumpCount++;
                    velocity.y = maxJumpVelocity;

                }
            }
            else
            {
                // 더블 점프
                if (controller.collisions.slidingDownMaxSlope)
                {

                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    {// 오를 수 없는 경사로에서 점프 불가능
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else
                {
                    currentJumpCount++;
                    velocity.x = directionalInput.x * moveSpeed;
                    velocity.y = maxJumpVelocity;

                }
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

    public void OnDashInputDown()
    {
        ghost.makeGhost = true;
        StartCoroutine("Dash");
    }

    IEnumerator Dash()
    {
        
        if (isDashing) { yield break; }
        isDashing = true;

        if (!controller.collisions.below)
        {
            gravity = 0;
            velocity.y = 0;
        }
        velocity.x = directionalInput.x * dashVelocity;

        yield return new WaitForSeconds(timeToDashApex);

        gravity = defaultGravity;
        //velocity.y = 0;

        ghost.makeGhost = false;
        isDashing = false;
    }
    
    void CalculateVelocity(float moveSpeed)
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);

        velocity.y += gravity * Time.fixedDeltaTime;
    }

    public void MeleeAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(meleeBoxPosition, meleeBoxSize, 0f);
        foreach(Collider2D collider in colliders)
        {
            
            
                
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .3f);
        Gizmos.DrawCube(meleeBoxPosition, meleeBoxSize);
    }
}
