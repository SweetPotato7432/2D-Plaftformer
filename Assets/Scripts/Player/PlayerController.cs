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
    private float jumpPower;
    [SerializeField]
    bool isGround;

    private void Awake()
    {

        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        body.linearVelocityX = inputValue * speed;
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
        if (isGround)
        {
            body.AddForceY(jumpPower, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGround = true;
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
