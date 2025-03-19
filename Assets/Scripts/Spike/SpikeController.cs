using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikeController : RaycastController
{
    public LayerMask passengerMask;
    ////단순 이동
    //public Vector3 move;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    // 플랫폼 움직임 순환 여부
    public bool cyclic;
    // 플랫폼 정지 후 대기
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints; // 0~1
    float nextMoveTime;

    TilemapCollider2D tilemapCollider2D;
    Bounds bounds;

    public Vector2 boxSize;
    public Vector2 center;


    public override void Start()
    {


        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        tilemapCollider2D = GetComponent<TilemapCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateRaycastOrigins();

        //Vector3 velocity = move * Time.fixedDeltaTime;

        if (localWaypoints.Length > 0)
        {
            Vector3 velocity = CalculatePlatformMovement();

            transform.Translate(velocity);

        }

        CheckForPlayer();

    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.fixedDeltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWayPoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWayPoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    // 배열을 모두 돌았으면 배열을 뒤집어서 다시 순환
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;

        }


        return newPos - transform.position;
    }

    void CheckForPlayer()
    {
        if (tilemapCollider2D == null) return;

        bounds = tilemapCollider2D.bounds;
        boxSize = bounds.size;
        center = bounds.center;

        Collider2D hit = Physics2D.OverlapBox(
        center,
        boxSize,
        0,
        LayerMask.GetMask("Player") // "Player" 레이어 감지
    );

        if (hit != null)
        {
            hit.GetComponent<Player>().TakeDamage(1);
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(center, boxSize);
    }
}
