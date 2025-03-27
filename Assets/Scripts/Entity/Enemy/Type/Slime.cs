using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Slime : Enemy
{
    EnemyController enemyController;
    Controller2D controller;

    Vector2 directionalInput;
    public int nextMove;

    Vector2 detectedPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Awake()
    {
        id = 201;
        base.Awake();
    }

    override public void Start()
    {
        

        base.Start();

        enemyController = GetComponent<EnemyController>();
        controller = GetComponent<Controller2D>();

        ChangeToMoveState();

        nextMove = 1;
        Invoke("Think", 5);
 
    }

    private void FixedUpdate()
    {
        stateMachine.Update();


    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    public override void Think()
    {
        // 이동 지정
        nextMove = Random.Range(-1, 2);
        Debug.Log(nextMove);
        directionalInput = new Vector2(nextMove, 0);

        if (nextMove == 0)
        {
            CancelInvoke();
            enemyController.SetDirectionalInput(directionalInput);
            ChangeToIdleState();
        }
        else
        {
            Invoke("Think", 5);
        }


    }

    public override void Move()
    {
        detectedPos = transform.position - new Vector3(0, 1.3f);

        enemyController.SetDirectionalInput(directionalInput);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(detectedPos, attackRange);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                float direction = collider.transform.position.x - transform.position.x;
                if (direction > 0)
                {
                    directionalInput.x = 1;
                    Debug.Log("플레이어가 우측에 있음");
                }
                else if (direction < 0)
                {
                    directionalInput.x = -1;
                    Debug.Log("플레이어가 좌측에 있음");
                }
                enemyController.SetDirectionalInput(directionalInput);
                ChangeToAttackState();
            }
        }


        bool frontCliff = controller.CliffCheck(directionalInput);
        bool frontWall = controller.WallCheck(directionalInput);
        if (frontCliff || frontWall)
        {
            nextMove *= -1;
            directionalInput = new Vector2(nextMove, 0);
            CancelInvoke();
            Invoke("Think", 5);

        }
    }

    public override void AttackStart()
    {

    }

    public override void AttackEnd()
    {
        ChangeToIdleState();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, .3f);
        Gizmos.DrawWireSphere(detectedPos, attackRange);

    }

}
