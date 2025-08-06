using UnityEngine;
using System.Collections;

public delegate void ActivePickupItemEffect(Player player);
public delegate void InteractionSceneChange();

[RequireComponent (typeof (PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    public static event ActivePickupItemEffect OnActivePickupItemEffect;
    public static event InteractionSceneChange OnInteractionSceneChange;

    AudioSource audioSource;

    [SerializeField] AudioClip[] audioClips;
    
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
        audioSource = GetComponent<AudioSource>();

        playerController = GetComponent<PlayerController> ();
        player = GetComponent<Player> ();

        UserInputManager.OnMoveInput -= OnMove;
        UserInputManager.OnMoveInput += OnMove;

        UserInputManager.OnJumpInput -= OnJump;
        UserInputManager.OnJumpInput += OnJump;

        UserInputManager.OnDashInput -= OnDash;
        UserInputManager.OnDashInput += OnDash;

        UserInputManager.OnAttackInput -= OnAttack;
        UserInputManager.OnAttackInput += OnAttack;

        UserInputManager.OnInteractiveInput -= OnInteractive;
        UserInputManager.OnInteractiveInput += OnInteractive;
    }

    // Update is called once per frame
    void Update()
    {
        directionalInput = new Vector2(moveInput.x, moveInput.y);
        playerController.SetDirectionalInput (directionalInput);

    }

    private void OnMove(Vector2 input)
    {
        if (Time.timeScale >= 1.0f)
        {
            moveInput = input;
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    private void OnJump(bool isPressed)
    {
        if (Time.timeScale >= 1.0f)
        {
            if (isPressed)
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

    private void OnDash(bool isPressed)
    {
        if (Time.timeScale >= 1.0f)
        {
            if (isPressed && canDash /*&& directionalInput.x != 0*/)
            {
                playerController.OnDashInputDown();
                audioSource.GetComponent<AudioSource>().PlayOneShot(audioClips[0]);
                StartCoroutine("DashCoolTime");
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


    private void OnAttack(bool isPressed)
    {
        if (Time.timeScale >= 1.0f ) 
        {
            if (isPressed)
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

    private void OnInteractive(bool isPressed)
    {
        if (isPressed)
        {
            OnActivePickupItemEffect?.Invoke(player);
            OnInteractionSceneChange?.Invoke();
        }
    }

    public void AttackSoundPlay()
    {
        audioSource.GetComponent<AudioSource>().PlayOneShot(audioClips[1]);
    }

    private void OnDestroy()
    {

        UserInputManager.OnMoveInput -= OnMove;

        UserInputManager.OnJumpInput -= OnJump;

        UserInputManager.OnDashInput -= OnDash;

        UserInputManager.OnAttackInput -= OnAttack;

        UserInputManager.OnInteractiveInput -= OnInteractive;
    }
}
