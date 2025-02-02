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


    /*
    Rigidbody2D body;
    //Animator anim;
    SpriteRenderer spriteRenderer;

    [Header("Move")]
    [SerializeField]
    private float speed;
    private float inputValue;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce;
    
    [SerializeField]
    bool isGround;
    [SerializeField]
    bool islongJump;
    [SerializeField]
    bool isJumping;
    [SerializeField]
    int maxJumpCount = 2;
    int currentJumpCount = 0;


    [SerializeField]
    float currentJumpForce = 0f;

    private void Awake()
    {

        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ////점프 중단 체크
        //if (!isGround && isJumping)
        //{
        //    StopJump();
        //}
    }

    private void FixedUpdate()
    {
        // 이동
        if (inputValue != 0f)
        {
            body.linearVelocityX = inputValue * speed * 100 * Time.fixedDeltaTime;
        }
        else
        {
            body.linearVelocityX = 0f;
        }
        // 이동 처리 (AddForce로 벽에서의 이동 개선)
        //body.AddForce(new Vector2(inputValue * speed * Time.fixedDeltaTime * 100, 0), ForceMode2D.Force);

        // 중력 체크
        // 점프 높낮이 체크 용
        if (islongJump && body.linearVelocityY > 0)
        {
            body.gravityScale = 5f;
        }
        else
        {
            body.gravityScale = 9.8f;
        }

    }

    private void LateUpdate()
    {
        if(inputValue != 0)
        {
            spriteRenderer.flipX = inputValue < 0;
        }
    }

    private void OnMove(InputValue value)
    {
        inputValue = value.Get<Vector2>().x;


    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (currentJumpCount > 0)
            {
                if (!isJumping && isGround)
                {
                    // 점프 시작
                    body.linearVelocityY = jumpForce;
                    //body.AddForceY(jumpForce, ForceMode2D.Impulse);
                    isJumping = true;
                    islongJump = true;
                    currentJumpCount--;
                }
                else
                {
                    // 점프 시작
                    body.linearVelocityY = jumpForce;
                    //body.AddForceY(jumpForce, ForceMode2D.Impulse);
                    islongJump = true;
                    currentJumpCount--;

                }
            }
            
        }
        else
        {
            Debug.Log("t");

            islongJump = false;
            isJumping = false;
        }
    }





    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            
            isGround = true;
            currentJumpCount = maxJumpCount;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGround = false;
            
        }
    }
    */
}
