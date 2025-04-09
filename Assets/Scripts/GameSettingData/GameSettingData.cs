using UnityEngine;
using UnityEngine.Audio;

public class GameSettingData : MonoBehaviour
{
    private int[] bestScore = new int[10];

    public static GameSettingData Instance { get; private set; }
    [SerializeField]
    private AudioMixer audioMixer;

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings(); // PlayerPrefs에서 설정을 불러오기
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
    }


    // 오디오 세팅
    public void SaveAudioSettings(float masterVolume, float bgmVolume, float sfxVolume)
    {
        PlayerPrefs.SetFloat("Master", masterVolume);

        PlayerPrefs.SetFloat("BGM", bgmVolume);
        PlayerPrefs.SetFloat("SFX", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BGM", 1f); // 기본값 0.75
        float sfxVolume = PlayerPrefs.GetFloat("SFX", 1f); // 기본값 0.75
        float masterVolume = PlayerPrefs.GetFloat("Master", 1f);

        SetMasterVolume(masterVolume);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);

        //bool isBGMMuted = PlayerPrefs.GetInt("BGMMute", 0) == 1;
        //bool isSFXMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;

        //SetBGMMute(isBGMMuted);
        //SetSFXMute(isSFXMuted);
    }

    // BGM 볼륨 조절 (AudioMixer의 BGM 그룹 볼륨 조절)
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
        SaveAudioSettings(PlayerPrefs.GetFloat("Master", 0.75f), volume, PlayerPrefs.GetFloat("SFX", 0.75f));
    }

    // SFX 볼륨 조절 (AudioMixer의 SFX 그룹 볼륨 조절)
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        SaveAudioSettings(PlayerPrefs.GetFloat("Master", 0.75f), PlayerPrefs.GetFloat("BGM", 0.75f), volume);
    }

    // Master 볼륨 조절 (AudioMixer의 Master 그룹 볼륨 조절)
    public void SetMasterVolume(float volume)
    {
        Debug.Log("Set M");
        audioMixer.SetFloat("Master", volume);
        SaveAudioSettings(volume, PlayerPrefs.GetFloat("BGM", 0.75f), PlayerPrefs.GetFloat("SFX", 0.75f));
    }

    //public void SetBGMMute(bool isMuted)
    //{
    //    if (isMuted)
    //    {
    //        audioMixer.SetFloat("BGM", -80);
    //    }
    //    else
    //    {
    //        float volume = PlayerPrefs.GetFloat("BGM", 0.75f);
    //        SetBGMVolume(volume);
    //    }
    //    PlayerPrefs.SetInt("BGMMute", isMuted ? 1 : 0);// 음소거 여부 저장
    //}

    //public void SetSFXMute(bool isMuted)
    //{
    //    if (isMuted)
    //    {
    //        audioMixer.SetFloat("SFX", -80);
    //    }
    //    else
    //    {
    //        float volume = PlayerPrefs.GetFloat("SFX", 0.75f);
    //        SetSFXVolume(volume);
    //    }
    //    PlayerPrefs.SetInt("SFXMute", isMuted ? 1 : 0);// 음소거 여부 저장
    //}
}
