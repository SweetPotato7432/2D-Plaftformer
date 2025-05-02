using System;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Item : MonoBehaviour
{

    public Controller2D controller;

    float maxJumpHeight = 4;
    float minJumpHeight = 1;
    float timeToJumpApex =0.4f;

    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    float moveSpeed = 12;

    float gravity;
    float defaultGravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    public int id;
    public string itemName;
    public int rarity;
    public DropItemInfo.EffectType effectType;
    public int effectStatus;
    public string effect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
    {
        controller = GetComponent<Controller2D>();

        defaultGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        gravity = defaultGravity;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        Debug.Log($"Item Gravity :{gravity}, JumpVelocity : {maxJumpVelocity}");

        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���� �� ���� �κ�
        CalculateVelocity(moveSpeed);
        // Controller2D�� �̵� �� ���� ����
        controller.Move(velocity * Time.fixedDeltaTime, Vector2.zero, false);

        //controller.Move(velocity * Time.fixedDeltaTime, false);

        // �߷� �ʱ�ȭ
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

    public void Initialize(int id, string itemName, int rarity, DropItemInfo.EffectType effectType, int effectStatus, string effect)
    {
        this.id = id;
        this.itemName = itemName;
        this.rarity = rarity;
        this.effectType = effectType;
        this.effectStatus = effectStatus;
        this.effect = effect;
    }

    void CalculateVelocity(float moveSpeed)
    {
        float targetVelocityX = 0f * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);

        velocity.y += gravity * Time.fixedDeltaTime;
    }

    //ȹ�� �˾� â
}
