using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    // 0: main, 1: battle, 2 : ranking
    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 가바지 컬렉터에 메모리 반납
            return;
        }
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject); // 다른 씬에서도 사용

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = audioClips[0];
    }

    public void ChangeMusic(int num)
    {
        audioSource.DOFade(0f, 1f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                audioSource.clip = audioClips[num];
                //StartCoroutine(StartMusic(num));
            });
        
    }

    public void ChangeMusic(string sceneName)
    {
        audioSource.DOFade(0f, 1f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                //switch (sceneName)
                //{
                //    //case "MainScene":
                //    //    audioSource.clip = audioClips[0];
                //    //    break;
                //    //case "BattleScene":
                //    //case "TutorialScene":
                //    //    audioSource.clip = audioClips[1];
                //    //    break;
                //    //case "LeaderBoardScene":
                //    //    audioSource.clip = audioClips[2];
                //    //    break;
                //}
                //StartCoroutine(StartMusic(sceneName));
            });
    }

    IEnumerator StartMusic(string sceneName)
    {
         // 1초 동안 볼륨 감소
        //yield return new WaitForSeconds(1f);
        switch (sceneName)
        {
            case "MainScene":
                audioSource.clip = audioClips[0];
                break;
            case "BattleScene":
            case "TutorialScene":
                audioSource.clip = audioClips[1];
                break;
            case "LeaderBoardScene":
                audioSource.clip = audioClips[2];
                break;
        }
        yield return null;

    }

    IEnumerator StartMusic(int num)
    {
        //audioSource.DOFade(0f, 1f).SetUpdate(true); // 1초 동안 볼륨 감소
        //yield return new WaitForSeconds(1f);
        audioSource.clip = audioClips[num];
        yield return null;

    }



    private void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        audioSource.DOFade(1f, 1f)
            .SetUpdate(true)
            .OnStart(() =>
            {
                audioSource.Play();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
