using UnityEngine;

public class Slime : Enemy
{
    EnemyController controller;
    Controller2D controller2D;

    Vector2 directionalInput;
    public int nextMove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        id = 201;

    }

    override public void Start()
    {
        controller = GetComponent<EnemyController>();
        controller2D = GetComponent<Controller2D>();

        nextMove = 1;
        Invoke("Think", 5);



        base.Start();
    }

    private void FixedUpdate()
    {
        controller.SetDirectionalInput(directionalInput);

        bool frontCliff = controller2D.CliffCheck(directionalInput);
        bool frontWall = controller2D.WallCheck(directionalInput);
        if (frontCliff || frontWall)
        {
            nextMove *= -1;
            directionalInput = new Vector2(nextMove, 0);
            CancelInvoke();
            Invoke("Think", 5);

        }

        //controller.SetDirectionalInput(directionalInput);


    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    void Think()
    {
        // 이동 지정
        nextMove = Random.Range(-1, 2);
        Debug.Log(nextMove);
        directionalInput = new Vector2(nextMove, 0);

        Invoke("Think", 5);
    }
}
