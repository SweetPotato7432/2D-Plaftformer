using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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

}
