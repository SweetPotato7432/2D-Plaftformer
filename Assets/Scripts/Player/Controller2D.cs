using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    const float skinWidth = .015f;

    public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float maxClimbAngle = 80; // 오를 수 있는 최대 경사로
    float maxDesendAngle = 80;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    public CollisionInfo collisions;


    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    //이동
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();// 충돌 확인 리셋

        collisions.velocityOld = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if(velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    // 수평 Collsion체크
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;


        for (int i = 0; i < horizontalRayCount; i++)
        {
            // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            //무언가 충돌
            if (hit)
            {
                // 바닥과 충돌했을때, 경사로라면 경사로의 각도를 확인해야함
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle) 
                {
                    // 경사로를 오르는 것과 내려가는것을 동시에 작동해, 속도가 어색해지는것을 방지
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    // 경사로를 탈때 간격이 벌어지는 문제 수정 및 다른 각도로 변환 될때 자연스럽게 함
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) 
                    {
                        distanceToSlopeStart = hit.distance-skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity,slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;

                    //Debug.Log(slopeAngle);
                }

                // 경사로를 오르지 않을때 작동
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        // 경사로 이동 중에  수평으로 장애물을 만남
                        // 이동거리로 수직 이동속도를 계산하지 못하므로 경사각을 이용해 확인
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    

    // 수직 Collsion체크
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;


        for (int i = 0; i < verticalRayCount; i++)
        {
            // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.up*directionY,rayLength,collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up*directionY*rayLength, Color.red);

            // 무언가 충돌
            if (hit)
            {
                velocity.y = (hit.distance-skinWidth)*directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    // 경사로 이동 중에  수직으로 장애물을 만남
                    // x이동속도 = y이동 속도 / tan(경사각도)
                    velocity.x = velocity.y/Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

            
        }
        // 경사로 이동 중 경사로의 각도가 변경되었을때, 잠시 멈추는 문제 해결
        // 1프레임에 경사로를 이동했을때, 이동할 위치에 경사로의 각도가 변경되었는지 확인
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right*directionX,rayLength,collisionMask);

            //Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.yellow);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }
    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        // 경사로의 각도에 따라 x,y의 이동 속도를 변환 시켜 주어야함
        // y이동거리 = 경사로 이동거리 * sin(경사각도)
        // x이동거리 = 경사로 이동거리 * cos(경사각도)

        //Mathf.Deg2Rad : 각도를 라디안으로 변환
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            // 점프를 위해 하단 충돌 판정을 켜줌
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
        //else
        //{
        //    Debug.Log("jumping on slope");
        //}
        

        
    }

    private void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDesendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float desendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= desendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }


    //Raycast추가를 위한 기준점 설정(상하좌우 꼭짓점)
    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // RayCast 지정 개수 만큼 추가
    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    // Raycast기준 꼭짓점
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    // 충돌 위치 정보
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle,slopeAngleOld;
        public Vector3 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
