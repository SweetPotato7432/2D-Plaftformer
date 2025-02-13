using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class RaycastController : MonoBehaviour
{
    public const float skinWidth = .015f;

    const float dstBetweenRays = .25f;

    public LayerMask collisionMask;
    public int horizontalRayCount;
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {


        collider = GetComponent<BoxCollider2D>();

        //// 플랫폼 크기에 비례해 레이 개수가 적다면 캐릭터가 밑으로 빠지거나 비정상적으로 행동하는 경우가 발생한다.
        //// 따라서 플랫폼 크기에 비례해 레이개수를 동적으로 조절하여 이상현상을 최소화 시킨다.(최소 4개, 최대 15개)
        //// 플랫폼 크기에 따라 레이 개수 동적으로 조절

        //// *중요!) 15개도 모자라는 경우가 생길수도 있으므로, 발판의 크기에 따라 최대치를 없애는 방향도 생각해볼것
        //verticalRayCount = Mathf.Clamp((int)(collider.bounds.size.x * 5), 4, 15);
        //horizontalRayCount = Mathf.Clamp((int)(collider.bounds.size.y * 5), 4, 15);


    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    //Raycast추가를 위한 기준점 설정(상하좌우 꼭짓점)
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);



        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // RayCast 지정 개수 만큼 추가
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);
        //horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        //verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    // Raycast기준 꼭짓점
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
