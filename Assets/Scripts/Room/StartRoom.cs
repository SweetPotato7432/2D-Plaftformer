using System.Collections.Generic;
using UnityEngine;

public class StartRoom : Room
{
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.PlayerMoveRoom(new Vector3(0,0,0),totalWidth,totalHeight,centerPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
