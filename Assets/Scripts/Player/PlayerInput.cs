using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent (typeof (PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerController player;

    Vector2 moveInput;


    bool isJump = false;
    public bool isDownJump = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController> ();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionalInput = new Vector2(moveInput.x, moveInput.y);
        player.SetDirectionalInput (directionalInput);

        if (isJump)
        {
            player.OnJumpInputDown(isJump,isDownJump);
        }
        if (!isJump)
        {
            player.OnJumpInputUp(isJump,isDownJump);
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
