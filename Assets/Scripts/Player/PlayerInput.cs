using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


[RequireComponent (typeof (PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerController player;

    Vector2 moveInput;

    bool canDash = true;

    bool canDownJump = true;

    bool isJump = false;
    public bool isDownJump = false;

    Vector2 directionalInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController> ();
    }

    // Update is called once per frame
    void Update()
    {
        directionalInput = new Vector2(moveInput.x, moveInput.y);
        player.SetDirectionalInput (directionalInput);

        //if (isJump)
        //{
        //    player.OnJumpInputDown(isJump,isDownJump);
        //}
        //if (!isJump)
        //{
        //    player.OnJumpInputUp(isJump,isDownJump);
        //}

    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (canDownJump&&moveInput.y == -1)
            {
                isDownJump = true;
                canDownJump = false;
                isJump = false;
                player.OnJumpInputDown(isJump, isDownJump);

                StartCoroutine(DownJumpCoolTime());


            }
            else if(moveInput.y != -1)
            {
                isJump = true;
                isDownJump = false;
                player.OnJumpInputDown(isJump, isDownJump);

            }

        }
        else
        {
            isJump = false;
            isDownJump = false;
            player.OnJumpInputUp(isJump, isDownJump);

        }
    }

    private void OnDash(InputValue value)
    {
        if (value.isPressed&&canDash && directionalInput.x !=0)
        {
            player.OnDashInputDown();
            StartCoroutine("DashCoolTime");
        }
        else
        {

        }
    }

    IEnumerator DownJumpCoolTime()
    {

        yield return new WaitForSeconds(0.1f);
        isDownJump = false ;
        player.OnJumpInputUp(isJump, isDownJump);

        yield return new WaitForSeconds(0.1f);
        canDownJump = true;

    }

    IEnumerator DashCoolTime()
    {
        canDash = false;

        yield return new WaitForSeconds(0.5f);

        canDash = true;
    }



}
