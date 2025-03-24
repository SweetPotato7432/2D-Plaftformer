using System;
using UnityEngine;


public class Controller2D : RaycastController
{
    public float maxSlopeAngle = 80; // 오를 수 있는 최대 경사로

    public CollisionInfo collisions;

    public Vector2 playerInput;

    bool isDownJump = false;


    public override void Start()
    {
        base.Start();

    }
    //이동
    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount,Vector2.zero,false, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount,Vector2 input,bool isDownJump, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();// 충돌 확인 리셋

        playerInput = input;
        this.isDownJump = isDownJump;

        collisions.moveAmountOld = moveAmount;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }
        if(moveAmount.x != 0)
        {
            HorizontalCollisions(ref moveAmount);
        }
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }
        

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }

        //Debug.Log(collisions.fallingThroughPlatform);
    }
    public void Dash(Vector2 moveAmount,Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();// 충돌 확인 리셋

        playerInput = input;

        collisions.moveAmountOld = moveAmount;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }
        if(moveAmount.x != 0)
        {
            HorizontalCollisions(ref moveAmount);
        }
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }
        
        

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }

        //Debug.Log(collisions.fallingThroughPlatform);
    }

    // 수평 Collsion체크
    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);

        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;


        for (int i = 0; i < horizontalRayCount; i++)
        {
            // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX /* * rayLength*/, Color.red);

            //무언가 충돌
            if (hit)
            {
                // Through태그를 가진 오브젝트와 충돌시 이동 제한 해제
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    
                }

                // 플레이어가 모종의 이유로 발판과의 거리가 0 일때(발판을 관통해 지나갈때)
                // 발판을 통과하지 못하고 비정상적으로 이동하는것을 방지 하기위해 체크를 넘어감
                if (hit.distance == 0)
                {
                    continue;
                }

                // 바닥과 충돌했을때, 경사로라면 경사로의 각도를 확인해야함
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle) 
                {
                    // 경사로를 오르는 것과 내려가는것을 동시에 작동해, 속도가 어색해지는것을 방지
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }
                    // 경사로를 탈때 간격이 벌어지는 문제 수정 및 다른 각도로 변환 될때 자연스럽게 함
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) 
                    {
                        distanceToSlopeStart = hit.distance-skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount,slopeAngle,hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;

                    //Debug.Log(slopeAngle);
                }

                // 경사로를 오르지 않을때 작동
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        // 경사로 이동 중에  수평으로 장애물을 만남
                        // 이동거리로 수직 이동속도를 계산하지 못하므로 경사각을 이용해 확인
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }   
    }

    

    // 수직 Collsion체크
    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;


        for (int i = 0; i < verticalRayCount; i++)
        {
            // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.up*directionY,rayLength,collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up*directionY *rayLength, Color.red);

            // 무언가 충돌
            if (hit)
            {
                //Through태그를 가진 오브젝트와 충돌시 이동 제한 해제
                if(hit.collider.tag == "Through")
                {
                    if(directionY == 1 || hit.distance ==0)
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    // 아래키 누르면 하향 점프
                    if (isDownJump&&playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;

                        Invoke("ResetFallingThroughPlatform", 0.1f);
                        continue;
                    }
                }

                // 속도 제한, 사물 못 뚫음
                moveAmount.y = (hit.distance-skinWidth)*directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    // 경사로 이동 중에  수직으로 장애물을 만남
                    // x이동속도 = y이동 속도 / tan(경사각도)
                    moveAmount.x = moveAmount.y/Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

            
        }
        // 경사로 이동 중 경사로의 각도가 변경되었을때, 잠시 멈추는 문제 해결
        // 1프레임에 경사로를 이동했을때, 이동할 위치에 경사로의 각도가 변경되었는지 확인
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right*directionX,rayLength,collisionMask);

            //Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.yellow);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;

                }
            }
        }
    }
    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle,Vector2 slopeNormal)
    {
        // 경사로의 각도에 따라 x,y의 이동 속도를 변환 시켜 주어야함
        // y이동거리 = 경사로 이동거리 * sin(경사각도)
        // x이동거리 = 경사로 이동거리 * cos(경사각도)

        //Mathf.Deg2Rad : 각도를 라디안으로 변환
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            // 점프를 위해 하단 충돌 판정을 켜줌
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;

            collisions.slopeNormal =slopeNormal;

        }
        //else
        //{
        //    Debug.Log("jumping on slope");
        //}



    }

    private void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopehitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopehitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

        if (maxSlopehitLeft ^ maxSlopehitRight)
        {
            SlideDownMaxSlope(maxSlopehitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopehitRight, ref moveAmount);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float desendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= desendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;

                            collisions.slopeNormal = hit.normal;

                        }
                    }
                }
            }
        }

        
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = hit.normal.x*(Mathf.Abs(moveAmount.y)-hit.distance)/Mathf.Tan(slopeAngle*Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    public bool CliffCheck(Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        rayOrigin += Vector2.right* moveAmount.x*0.2f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);

        if (hit.collider == null)
        {
            Debug.Log("경고 낭떠러지");
            return true;
        }
        return false;
    }



    // 충돌 위치 정보
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle,slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
