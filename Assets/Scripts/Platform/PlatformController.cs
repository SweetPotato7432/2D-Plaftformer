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

        // �÷��� ũ�⿡ ����� ���� ������ ���ٸ� ĳ���Ͱ� ������ �����ų� ������������ �ൿ�ϴ� ��찡 �߻��Ѵ�.
        // ���� �÷��� ũ�⿡ ����� ���̰����� �������� �����Ͽ� �̻������� �ּ�ȭ ��Ų��.(�ּ� 4��, �ִ� 15��)
        // �÷��� ũ�⿡ ���� ���� ���� �������� ����
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

        // �����̵� �÷���
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;


            for (int i = 0; i < verticalRayCount; i++)
            {
                // ĳ���Ͱ� ���� or ���� ���϶� raycast�� ���۵� �κ� ����
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

        // ���� �̵� �÷���
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;


            for (int i = 0; i < horizontalRayCount; i++)
            {
                // ĳ���Ͱ� ���� or ���� ���϶� raycast�� ���۵� �κ� ����
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

        // �������� �̵��ϴ� �÷����� �Ʒ��� �������� �÷����� �÷��̾ �����Ҷ�
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
