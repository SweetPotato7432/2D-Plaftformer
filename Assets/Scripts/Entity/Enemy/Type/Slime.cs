using UnityEngine;

public class Slime : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public void Awake()
    {
        id = 201;

    }

    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }
}
