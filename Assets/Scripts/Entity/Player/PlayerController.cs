using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    Controller2D controller;

    Animator playerAnim;

    SpriteRenderer spriteRenderer;

    Ghost ghost;

    Player player;

    public float maxJumpHeight;
    public float minJumpHeight;
    public float timeToJumpApex;
    // 미끌리는 시간
    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    public float moveSpeed;

    float dashDistance = 4f;
    float timeToDashApex = .1f;

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

    [Header("MeleeAttack")]
    public bool isAttack;
    public Vector2 meleeBoxSize;
    Vector2 meleeBoxPosition;
    float attackDir = 1;

    float attackVelocity;

    bool enableCombo = true;
    bool enableAttackBox = false;

    float atk;
    float atkSpeed;

    HashSet<Enemy> attackedEnemy = new HashSet<Enemy>();

    PlayerInfo stat;

    bool isDead = false;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        playerAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ghost = GetComponent<Ghost>();
        player = GetComponent<Player>();

        stat = GameManager.Instance.PlayerStatInitialize(player.id);

        atk = stat.atk;
        atkSpeed = stat.attackSpeed;
        moveSpeed = stat.moveSpeed;
        maxJumpHeight = stat.maxJumpHeight;
        minJumpHeight = stat.minJumpHeight;
        timeToJumpApex = stat.timeToJumpApex;

        defaultGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        gravity = defaultGravity;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        dashVelocity = dashDistance / timeToDashApex;

        attackVelocity = 3f;

        Debug.Log($"Gravity :{gravity}, JumpVelocity : {maxJumpVelocity}");
    }

    private void FixedUpdate()
    {
        if (player.state == Entity.States.ATTACK && !enableAttackBox)
        {
            gravity = defaultGravity;
            // 가속 및 감속 부분
            CalculateVelocity(moveSpeed);
            velocity.x = 0;
            // Controller2D로 이동 및 변수 전달
            controller.Move(velocity * Time.fixedDeltaTime, directionalInput, isDownJump);
        }
        else if(player.state == Entity.States.DEAD)
        {
            isDead = true;
        }
        else
        {
            // 가속 및 감속 부분
            CalculateVelocity(moveSpeed);
            // Controller2D로 이동 및 변수 전달
            controller.Move(velocity * Time.fixedDeltaTime, directionalInput, isDownJump);
        }

        // 사각형의 중심 위치
        meleeBoxPosition = new Vector2(transform.position.x /*+ (meleeBoxSize.x/2) * attackDir*/, controller.collider.transform.position.y + controller.collider.offset.y + (meleeBoxSize.y / 4));

        //공격 범위 활성화
        if (enableAttackBox)
        {
            velocity.x = 0;

            if (!controller.collisions.below) 
            {
                gravity = 0;
                velocity.y = 0;
            }


            // 공격 중이면 이동 입력 무시하고 공격 전진 속도 적용
            CalculateVelocity(moveSpeed);
            velocity.x = directionalInput.x * attackVelocity; // 공격 방향으로 전진
            controller.Move(velocity * Time.fixedDeltaTime, directionalInput, isDownJump);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(meleeBoxPosition, meleeBoxSize, 0f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    Enemy enemy = collider.GetComponent<Enemy>();
                    if (!attackedEnemy.Contains(enemy))
                    {
                        attackedEnemy.Add(enemy);
                        // 추후 공격 스탯 기반으로 수정
                        enemy.TakeDamage(player.atk);
                    }

                }
                //Debug.Log(collider.name);
            }
        }

        //CalculateVelocity(dashSpeed);
        //controller.Dash(velocity * Time.fixedDeltaTime, directionalInput);
        ////onDash = false;

        if (controller.collisions.below)
        {
            playerAnim.SetFloat("YVelocity", 0);
            playerAnim.SetBool("isGround",true);
            currentJumpCount = 0;
        }
        else
        {
            playerAnim.SetBool("isGround", false);
            playerAnim.SetFloat("YVelocity", velocity.y);
        }

        if(directionalInput.x != 0)
        {
            playerAnim.SetBool("isWalking", true);
        }
        else
        {
            playerAnim.SetBool("isWalking", false);
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


       
        

    }

    public void SetDirectionalInput(Vector2 input)
    {
        if(isDead) return;
        directionalInput = input;
        if (directionalInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(directionalInput.x < 0)
        {
            spriteRenderer.flipX= true;
        }

        if(attackDir != directionalInput.x && directionalInput.x != 0)
        {
            attackDir = Mathf.Sign(directionalInput.x);
        }
    }

    public void OnJumpInputDown(bool isJump, bool isDownJump)
    {
        if (isDead) return;
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
        if (isDead) return;
        this.isDownJump = isDownJump;

        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void OnDashInputDown()
    {
        if (isDead) return;
        ghost.makeGhost = true;
        StartCoroutine("Dash");
    }

    IEnumerator Dash()
    {
        
        if (isDashing) { yield break; }
        playerAnim.SetTrigger("isDash");
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

    IEnumerator AttackMove()
    {
        if (isAttack) { yield break; }
        isAttack = true;

        if (!controller.collisions.below)
        {
            gravity = 0;
            velocity.y = 0;
        }

        // 플레이어가 바라보는 방향으로 이동
        velocity.x = attackDir * attackVelocity;

        yield return new WaitForSeconds(0.1f);

        gravity = defaultGravity;

        // 이동 정지
        velocity.x = 0;

        isAttack = false;
    }

    void CalculateVelocity(float moveSpeed)
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);

        velocity.y += gravity * Time.fixedDeltaTime;
    }

    // 공격방식
    public void MeleeAttack(bool isAttack)
    {
        if (isDead) return;
        this.isAttack = isAttack;
        if (isAttack)
        {
            if (enableCombo)
            {
                playerAnim.SetTrigger("nextCombo");

            }
        }
        

    }
    
    void EndComboAttack()
    {
        player.state = Entity.States.IDLE;
    }

    void EnableComboAttack()
    {

        enableCombo = true;
    }
    void DisableComboAttack()
    {
        player.state = Entity.States.ATTACK;

        enableCombo = false;
    }
    void EnableAttackBox()
    {
        if (!enableAttackBox)
        {
            enableAttackBox = true;
        }
    }
    void DisableAttackBox()
    {
        if (enableAttackBox)
        {


            enableAttackBox = false;
            attackedEnemy.Clear();
        }

    }

    public void EnableInvinsible()
    {
        playerAnim.SetBool("TakeDMG", true);
    }

    public void DisableInvinsible()
    {
        playerAnim.SetBool("TakeDMG", false);
    }

    public void DeadStart()
    {
        playerAnim.SetTrigger("isDead");
    }


    private void OnDrawGizmosSelected()
    {
        if (enableAttackBox)
        {
            Gizmos.color = new Color(0, 1, 0, .3f);
            Gizmos.DrawCube(meleeBoxPosition, meleeBoxSize);
        }

    }
}
