using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]

public class TilemapRaycastController : MonoBehaviour
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
    public TilemapCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {


        collider = GetComponent<TilemapCollider2D>();


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
