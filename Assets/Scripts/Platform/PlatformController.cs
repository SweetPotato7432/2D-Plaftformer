using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    public Vector3 move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        // 플랫폼 크기에 비례해 레이 개수가 적다면 캐릭터가 밑으로 빠지거나 비정상적으로 행동하는 경우가 발생한다.
        // 따라서 플랫폼 크기에 비례해 레이개수를 동적으로 조절하여 이상현상을 최소화 시킨다.(최소 4개, 최대 15개)
        // 플랫폼 크기에 따라 레이 개수 동적으로 조절
        verticalRayCount = Mathf.Clamp((int)(collider.bounds.size.x * 5), 4, 15);
        horizontalRayCount = Mathf.Clamp((int)(collider.bounds.size.y * 5), 4, 15);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.fixedDeltaTime;

        MovePassengers(velocity);
        transform.Translate(velocity);
    }


    void MovePassengers(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // 수직이동 플랫폼
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;


            for (int i = 0; i < verticalRayCount; i++)
            {
                // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }

                }

            }
        }

        // 수평 이동 플랫폼
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;


            for (int i = 0; i < horizontalRayCount; i++)
            {
                // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = 0;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }

                }
            }
        }

        // 수평으로 이동하는 플랫폼과 아래로 내려가는 플랫폼에 플레이어가 존재할때
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

    }
}
