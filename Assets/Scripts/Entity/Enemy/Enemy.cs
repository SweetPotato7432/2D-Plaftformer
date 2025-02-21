using UnityEngine;

public class Enemy : Entity 
{ 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize(1, "test", 10, 1, 0, 1, 1, 1, 1);
    }

    // Update is called once per frame
    virtual public void Update()
    {
        base.Update();
    }
}
