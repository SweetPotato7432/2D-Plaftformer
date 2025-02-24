using UnityEngine;

public class GameManager : MonoBehaviour
{

    PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMoveRoom(Vector3 transformPos)
    {
        playerController.transform.position = transformPos;
    }
}
