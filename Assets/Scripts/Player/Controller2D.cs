using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    const float skinWidth = .015f;

    public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;



    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    //이동
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        if(velocity.x != 0)
        {
            HorizontalCollision(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollision(ref velocity);
        }

        transform.Translate(velocity);
    }

    // 수직 Collsion체크
    void VerticalCollision(ref Vector3 velocity)
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

            if (hit)
            {
                velocity.y = (hit.distance-skinWidth)*directionY;
                rayLength = hit.distance;

            }
        }
    }
    // 수평 Collsion체크
    void HorizontalCollision(ref Vector3 velocity)
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

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

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

        horizontalRayCount = Mathf.Clamp(horizontalRayCount,2,int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount,2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount -1);

    }

    // Raycast기준 꼭짓점
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
