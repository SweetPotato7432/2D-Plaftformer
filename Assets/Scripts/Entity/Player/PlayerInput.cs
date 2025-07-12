using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public delegate void ActivePickupItemEffect(Player player);
public delegate void InteractionSceneChange();

[RequireComponent (typeof (PlayerController))]
public class PlayerInput : MonoBehaviour
{
    public static event ActivePickupItemEffect OnActivePickupItemEffect;
    public static event InteractionSceneChange OnInteractionSceneChange;

    PlayerController playerController;
    Player player;

    Vector2 moveInput;

    bool canDash = true;

    bool canDownJump = true;

    bool isJump = false;
    bool isDownJump = false;

    Vector2 directionalInput;

    bool isAttack = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<PlayerController> ();
        player = GetComponent<Player> ();


    }

    // Update is called once per frame
    void Update()
    {
        directionalInput = new Vector2(moveInput.x, moveInput.y);
        playerController.SetDirectionalInput (directionalInput);

    }

    private void OnMove(InputValue value)
    {
        if (Time.timeScale >= 1.0f)
        {
            moveInput = value.Get<Vector2>();
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    private void OnJump(InputValue value)
    {
        if (Time.timeScale >= 1.0f)
        {
            if (value.isPressed)
            {
                if (canDownJump && moveInput.y == -1)
                {
                    isDownJump = true;
                    canDownJump = false;
                    isJump = false;
                    playerController.OnJumpInputDown(isJump, isDownJump);

                    StartCoroutine(DownJumpCoolTime());


                }
                else if (moveInput.y != -1)
                {
                    isJump = true;
                    isDownJump = false;
                    playerController.OnJumpInputDown(isJump, isDownJump);

                }

            }
            else
            {
                isJump = false;
                isDownJump = false;
                playerController.OnJumpInputUp(isJump, isDownJump);

            }
        }
    }

    private void OnDash(InputValue value)
    {
        if (Time.timeScale >= 1.0f)
        {
            if (value.isPressed && canDash && directionalInput.x != 0)
            {
                playerController.OnDashInputDown();
                StartCoroutine("DashCoolTime");
            }
            else
            {

            }
        }
    }

    IEnumerator DownJumpCoolTime()
    {

        yield return new WaitForSeconds(0.1f);
        isDownJump = false ;
        playerController.OnJumpInputUp(isJump, isDownJump);

        yield return new WaitForSeconds(0.1f);
        canDownJump = true;

    }

    IEnumerator DashCoolTime()
    {
        canDash = false;

        yield return new WaitForSeconds(0.5f);

        canDash = true;
    }


    private void OnAttack(InputValue value)
    {
        if (Time.timeScale >= 1.0f ) 
        {
            if (value.isPressed)
            {
                isAttack = true;
                playerController.MeleeAttack(isAttack);

            }
            else
            {
                isAttack = false;
                playerController.MeleeAttack(isAttack);

            }
        }
        

    }

    private void OnInteractive(InputValue value)
    {
        if (value.isPressed)
        {
            OnActivePickupItemEffect?.Invoke(player);
            OnInteractionSceneChange?.Invoke();
        }
    }


}
