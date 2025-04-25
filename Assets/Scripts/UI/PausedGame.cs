using UnityEngine;

public class PausedGame : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("Pause");
        Time.timeScale = 0f;
    }

    private void Update()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Debug.Log("Unpaused");
        Time.timeScale = 1.0f;
    }

}
