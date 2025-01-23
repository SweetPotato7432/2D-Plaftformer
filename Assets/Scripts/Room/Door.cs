using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    GameObject door;
    [SerializeField]
    GameObject wall;

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
}
