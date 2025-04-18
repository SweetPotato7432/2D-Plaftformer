using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    [SerializeField]
    GameObject pauseUI;
    [SerializeField]
    GameObject worldmapUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            if (pauseUI.activeSelf)
            {
                pauseUI.SetActive(false);
            }
            else
            {
                pauseUI.SetActive(true);
            }
        }

    }

    private void OnWorldMap(InputValue value)
    {
        if (value.isPressed)
        {
            if (worldmapUI.activeSelf)
            {
                worldmapUI.SetActive(false);
            }
            else
            {
                worldmapUI.SetActive(true);
            }
        }
    }
}
