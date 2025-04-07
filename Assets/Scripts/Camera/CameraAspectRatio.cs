using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void AdjustCameraAspectRatio()
    {
        float tartgetAspect = 9f / 16f;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect/tartgetAspect;

        Camera mainCamera = Camera.main;

        if(scaleHeight < 1.0f)
        {
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f/scaleHeight;
            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            mainCamera.rect = rect;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AdjustCameraAspectRatio();
    }
}
