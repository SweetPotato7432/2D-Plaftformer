using System;
using UnityEngine;


public class Controller2D : RaycastController
{
    float maxClimbAngle = 80; // ���� �� �ִ� �ִ� ����
    float maxDesendAngle = 80;

    public CollisionInfo collisions;

    public Vector2 playerInput;

    bool isDownJump = false;


    public override void Start()
    {
        base.Start();

    }
    //�̵�
    public void Move(Vector3 velocity, bool standingOnPlatform)
    {
        Move(velocity,Vector2.zero,false, standingOnPlatform);
    }

    public void Move(Vector3 velocity,Vector2 input,bool isDownJump, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();// �浹 Ȯ�� ����

        playerInput = input;
        this.isDownJump = isDownJump;

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

        if (standingOnPlatform)
        {
            collisions.below = true;
        }

        //Debug.Log(collisions.fallingThroughPlatform);
    }

    // ���� Collsionüũ
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);

        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;


        for (int i = 0; i < horizontalRayCount; i++)
        {
            // ĳ���Ͱ� ���� or ���� ���϶� raycast�� ���۵� �κ� ����
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            //���� �浹
            if (hit)
            {
                // Through�±׸� ���� ������Ʈ�� �浹�� �̵� ���� ����
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    
                }

                // �÷��̾ ������ ������ ���ǰ��� �Ÿ��� 0 �϶�(������ ������ ��������)
                // ������ ������� ���ϰ� ������������ �̵��ϴ°��� ���� �ϱ����� üũ�� �Ѿ
                if (hit.distance == 0)
                {
                    continue;
                }

                // �ٴڰ� �浹������, ���ζ�� ������ ������ Ȯ���ؾ���
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle) 
                {
                    // ���θ� ������ �Ͱ� �������°��� ���ÿ� �۵���, �ӵ��� ��������°��� ����
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    // ���θ� Ż�� ������ �������� ���� ���� �� �ٸ� ������ ��ȯ �ɶ� �ڿ������� ��
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

                // ���θ� ������ ������ �۵�
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        // ���� �̵� �߿�  �������� ��ֹ��� ����
                        // �̵��Ÿ��� ���� �̵��ӵ��� ������� ���ϹǷ� ��簢�� �̿��� Ȯ��
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    

    // ���� Collsionüũ
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;


        for (int i = 0; i < verticalRayCount; i++)
        {
            // ĳ���Ͱ� ���� or ���� ���϶� raycast�� ���۵� �κ� ����
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.up*directionY,rayLength,collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up*directionY*rayLength, Color.red);

            // ���� �浹
            if (hit)
            {
                //Through�±׸� ���� ������Ʈ�� �浹�� �̵� ���� ����
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
                    // �Ʒ�Ű ������ ���� ����
                    if (isDownJump&&playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;

                        Invoke("ResetFallingThroughPlatform", 0.5f);
                        continue;
                    }
                }

                // �ӵ� ����, �繰 �� ����
                velocity.y = (hit.distance-skinWidth)*directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    // ���� �̵� �߿�  �������� ��ֹ��� ����
                    // x�̵��ӵ� = y�̵� �ӵ� / tan(��簢��)
                    velocity.x = velocity.y/Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

            
        }
        // ���� �̵� �� ������ ������ ����Ǿ�����, ��� ���ߴ� ���� �ذ�
        // 1�����ӿ� ���θ� �̵�������, �̵��� ��ġ�� ������ ������ ����Ǿ����� Ȯ��
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
        // ������ ������ ���� x,y�� �̵� �ӵ��� ��ȯ ���� �־����
        // y�̵��Ÿ� = ���� �̵��Ÿ� * sin(��簢��)
        // x�̵��Ÿ� = ���� �̵��Ÿ� * cos(��簢��)

        //Mathf.Deg2Rad : ������ �������� ��ȯ
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            // ������ ���� �ϴ� �浹 ������ ����
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

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }




    // �浹 ��ġ ����
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle,slopeAngleOld;
        public Vector3 velocityOld;
        public bool fallingThroughPlatform;

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
