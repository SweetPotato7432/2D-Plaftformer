using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    UIManager uiManager;

    Stack<GameObject> activeUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GetComponent<UIManager>();
        activeUI = new Stack<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnOption(InputValue value)
    {
        if (value.isPressed)
        {
            uiManager.ActiveOptionUI();
        }

    }

    private void OnWorldMap(InputValue value)
    {
        if (value.isPressed)
        {
            uiManager.ActiveWorldMapUI();
        }
    }
}
