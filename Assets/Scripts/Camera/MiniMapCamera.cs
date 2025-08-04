using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    CameraFollow mainCamera;

    float roomWidth;
    float roomHeight;
    Vector2 centerPos;

    Camera miniMap_Camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        miniMap_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        SetMiniMapCamera(roomWidth, roomHeight, centerPos);
    }

    public void SetCameraArea(float roomWidth, float roomHeight, Vector2 centerPos)
    {
        this.roomWidth = roomWidth;
        this.roomHeight = roomHeight;
        this.centerPos = centerPos;
    }

    void SetMiniMapCamera(float roomWidth, float roomHeight, Vector2 centerPos)
    {
        float padding = 1f; // 여유 공간
        float size = Mathf.Max(roomWidth, roomHeight) / 2f + padding;
        miniMap_Camera.orthographicSize = size;

        transform.position = new Vector3(centerPos.x,centerPos.y,transform.position.z);
    }
}
