using UnityEngine;

public class SceneChange : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        Debug.Log(MySceneManager.Instance == null ? "MySceneManager is NULL" : "MySceneManager is OK");


        MySceneManager.Instance.ChangeScene(sceneName);
        //AudioManager.Instance.ChangeMusic(sceneName);
    }

    public void ExitGame()
    {
        GameSettingData.Instance.InitializeCurrentPlayer();
        Application.Quit();
    }
}
