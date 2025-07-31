using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Item : MonoBehaviour
{
    // 아이템 등장 확률
    Dictionary<int, int> baseWeights = new Dictionary<int, int>
    {
        { 0, 45 }, { 1, 25 }, { 2, 15 }, { 3, 10 }, { 4, 5 }
    };

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
        //Debug.Log($"Item Gravity :{gravity}, JumpVelocity : {maxJumpVelocity}");

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 가속 및 감속 부분
        CalculateVelocity(moveSpeed);
        // Controller2D로 이동 및 변수 전달
        controller.Move(velocity * Time.fixedDeltaTime, Vector2.zero, false);

        //controller.Move(velocity * Time.fixedDeltaTime, false);

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

    public void Initialize(int id, string itemName, int rarity, int effectStatus, string effect)
    {
        this.id = id;
        this.itemName = itemName;
        this.rarity = rarity;
        
        this.effectStatus = effectStatus;
        this.effect = effect;
    }

    void CalculateVelocity(float moveSpeed)
    {
        float targetVelocityX = 0f * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);

        velocity.y += gravity * Time.fixedDeltaTime;
    }

    public int CalculateRarityFromDropItem()
    {

        var rarityGroups = GameManager.Instance.dropItemRarityGroups;

        var filteredWeights = baseWeights
            .Where(kvp => rarityGroups.ContainsKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        int totalWeight = filteredWeights.Values.Sum();
        int rand = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;

        foreach (var kvp in filteredWeights)
        {
            cumulative += kvp.Value;
            if (rand < cumulative)
            {
                return kvp.Key;
            }
        }
        throw new Exception("Rarity selection failed.");
    }

    public int CalculateRarityFromPassiveItem()
    {

        var rarityGroups = GameManager.Instance.passiveItemRarityGroups;

        var filteredWeights = baseWeights
            .Where(kvp => rarityGroups.ContainsKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        int totalWeight = filteredWeights.Values.Sum();
        int rand = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;

        foreach (var kvp in filteredWeights)
        {
            cumulative += kvp.Value;
            if (rand < cumulative)
            {
                return kvp.Key;
            }
        }
        throw new Exception("Rarity selection failed.");
    }


    //획득 팝업 창
}
