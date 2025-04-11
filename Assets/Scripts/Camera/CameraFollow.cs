using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //미니맵 카메라
    [SerializeField]
    MiniMapCamera miniMapCamera;

    // player 연결
    public Controller2D target;
    // 캐릭터가 너무 중앙에 오지 않게 조정, 수치는 1정도
    public float verticalOffset;

    // 플레이어 전방 확인 거리
    public float lookAheadDstX;
    // 부드럽게 변경되는 시간
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    // 카메라의 중심이 될 범위
    public Vector2 focusAreaSize;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    // 방향
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    [SerializeField]
    float roomWidth;
    [SerializeField]
    float roomHeight;
    [SerializeField]
    Vector2 centerPos;

    [SerializeField]
    float height;
    [SerializeField]
    float width;

    public float clampX;
    public float clampY;

    public void Start()
    {


        focusArea = new FocusArea(target.collider.bounds,focusAreaSize);
        transform.position = focusArea.center + Vector2.up * verticalOffset;
    }

    private void FixedUpdate()
    {
        focusArea.Update(target.collider.bounds);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if(focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            // 플레이어의 입력에 따라서만 위치를 이동시킨다.
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX*lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX,targetLookAheadX,ref smoothLookVelocityX,lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY,verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;

        transform.position = (Vector3)focusPosition + Vector3.forward *-10;
        LimitCameraArea();
    }

    void LimitCameraArea()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;

        float lx = roomWidth/2 - width;
        clampX = Mathf.Clamp(transform.position.x, -lx + centerPos.x, lx + centerPos.x);

        float ly = roomHeight/2 - height;
        clampY = Mathf.Clamp(transform.position.y, -ly + centerPos.y, ly + centerPos.y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }

    public void SetCameraArea(float roomWidth, float roomHeight, Vector2 centerPos)
    {
        this.roomWidth = roomWidth;
        this.roomHeight = roomHeight;
        this.centerPos = centerPos;
        miniMapCamera.SetCameraArea(roomWidth, roomHeight, centerPos);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,.3f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        float left, right;
        float top, bottom;

        // 영역 이동
        public Vector2 velocity;


        // 카메라의 위치 이동을 확인하는 경계 지정
        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }


        public void Update(Bounds targetBounds)
        {
            // 좌 우측 확인
            float shiftX = 0;
            //플레이어의 위치에 따라 좌우측 이동 확인
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            //플레이어의 위치에 따라 상하 이동 확인
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);

        }
    }
}
