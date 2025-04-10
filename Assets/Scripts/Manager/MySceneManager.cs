using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{

    public static MySceneManager Instance {  get; private set; }

    [SerializeField]
    private CanvasGroup fade_IMG;

    float fadeDuration = 1f;

    [SerializeField]
    private GameObject loading;
    [SerializeField]
    private TMP_Text loading_Text;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ChangeScene(string sceneName)
    {
        AudioManager.Instance.ChangeMusic(sceneName);
        fade_IMG.DOFade(1, fadeDuration)
        .SetUpdate(true)
        .OnStart(() => { 
            fade_IMG.blocksRaycasts = true;
        })
        .OnComplete(() => {
            StartCoroutine(LoadScene(sceneName));            
        });
    }

    IEnumerator LoadScene(string sceneName)
    {
        loading.SetActive(true);
        loading_Text.gameObject.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        float past_time = 0;
        float percentage = 0;

        while (!(async.isDone))
        {
            yield return null;

            //past_time += Time.deltaTime;
            past_time += Time.unscaledDeltaTime;

            if (percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if (percentage == 100)
                {
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if (percentage >= 90) past_time = 0;
            }
            loading_Text.text = percentage.ToString("0") + "%"; //로딩 퍼센트 표기
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        fade_IMG.DOFade(0, fadeDuration)
        .SetUpdate(true)
        .OnStart(() => {
            loading.SetActive(false);
            loading_Text.gameObject.SetActive(false);
        })
        .OnComplete(() => {
            fade_IMG.blocksRaycasts = false;
        });
    }
}
