using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    GameObject door;
    [SerializeField]
    GameObject wall;

    [SerializeField]
    BoxCollider2D teleportPoint;

    Vector2Int roomPos;
    Vector2Int destination;

    RoomManager roomManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        roomManager = FindFirstObjectByType<RoomManager>();
        DoorActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoorActive(bool active)
    {
        Debug.Log("ActiveDoor");
        switch (active)
        {
            case true:
                door.SetActive(true);
                wall.SetActive(false);
                break;
            case false:
                door.SetActive(false);
                wall.SetActive(true);
                break;
        }
    }

    public void InitializeDoor(Vector2Int roomPos,Vector2Int destination)
    {
        this.roomPos = roomPos;
        this.destination = destination;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 방이동 로직
            Debug.Log(destination);
            roomManager.setMoveRoomDestination(roomPos, destination);
        }
    }
}
