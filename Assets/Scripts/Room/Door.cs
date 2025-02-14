using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    GameObject door;
    [SerializeField]
    GameObject wall;

    [SerializeField]
    BoxCollider2D teleportPoint;

    Vector2 destination;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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

    public void SetDestination(Vector2 destination)
    {
        this.destination = destination;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 방이동 로직
            Debug.Log(destination);
            
        }
    }
}
