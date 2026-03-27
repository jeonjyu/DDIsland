using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum Soundtype
{
    None,
    BGM,        // 배경음
    SFX,        // 효과음
    AMB,        // 환경음
    Preview,    // 미리듣기
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서")]
    [field: SerializeField] public AudioMixer MasterAudioMixer { get; private set; }

    [Header("브금을 재생할 오디오 소스")]
    [field: SerializeField] public AudioSource BgmSource { get; private set; }
    [field: SerializeField] public AudioSource AmbSource { get; private set; }
    [field: SerializeField] public AudioSource previewSource { get; private set; }

    [Header("효과음을 재생할 오디오 소스 프리팹")]
    [SerializeField] private AudioSource sfxSource;

    [Header("사운드 풀")]
    [SerializeField] private SoundPool soundPool;

    private List<AudioSource> sfxList = new List<AudioSource>();    // 현재 재생중인 효과음 오디오 소스를 가지고 있을 리스트
    private Coroutine sfxPlayCoroutine;                             // 현재 재생중인 효과음 오디오 소스들을 체크하는 코루틴 변수

    [SerializeField] private float clearSourceTime = 0.5f;          // 효과음 풀 정리 타이밍
    private WaitForSeconds clearSourceWs;

    private Coroutine bgmPlayDoneCoroutine;         // 배경음 재생이 끝났는지 체크하는 코루틴 변수
    public event Action OnBGMPlayDone;              // 배경음 재생이 끝나면 실행할 이벤트

    private Coroutine previewPlayDoneCoroutine;     // 미리듣기 재생이 끝났는지 체크하는 코루틴 변수
    public event Action OnPreviewPlayDone;          // 미리듣기 재생이 끝나면 실행할 이벤트

    private Coroutine PlayCoroutine;                // 곡을 재생할 때 실행할 코루틴 변수, 볼륨 페이드 아웃 등을 위해 사용

    protected override void Awake()
    {
        base.Awake();
        InitData();
    }

    private void Start()
    {
        SetSoundVolume(Soundtype.BGM, PlayerPrefsDataManager.BgmVolume, PlayerPrefsDataManager.BgmVolumeMute);
        SetSoundVolume(Soundtype.SFX, PlayerPrefsDataManager.SFXVolume, PlayerPrefsDataManager.SFXVolumeMute);
        SetSoundVolume(Soundtype.AMB, PlayerPrefsDataManager.BGSVolume, PlayerPrefsDataManager.BGSVolumeMute);
    }

    #region Init
    // 데이터 초기화
    private void InitData()
    {
        clearSourceWs = new WaitForSeconds(clearSourceTime);
    }
    #endregion

    #region 오디오 클립을 받아와 재생
    public void PlayBGM(AudioClip clip)
    {
        SafeStartCoroutine(PlayCoroutine, Co_PlayBGM(clip));
    }

    public void PlayAMB(AudioClip clip)
    {
        PlaySound(Soundtype.AMB, AmbSource, clip);
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="clip"> 재생할 오디오 클립 </param>
    public void PlaySFX(AudioClip clip)
    {
        AudioSource source = soundPool.Get(sfxSource);

        if (source == null) return;

        PlaySound(Soundtype.SFX, source, clip);
    }

    /// <summary>
    /// 미리듣기 재생
    /// </summary>
    /// <param name="clip"> 재생할 클립 </param>
    /// <param name="maxTime"> 미리듣기 시간(초) </param>
    public void PlayPreview(AudioClip clip, float maxTime)
    {
        PlaySound(Soundtype.Preview, previewSource, clip);
        CheckAudioPlayDone(previewPlayDoneCoroutine, previewSource, maxTime, OnPreviewPlayDone);
    }
    #endregion

    private void PlaySound(Soundtype type, AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) 
            return;

        if (source.clip == clip && source.time > 0f)
            source.Stop();

        source.clip = clip;
        source.Play();

        if (type == Soundtype.SFX)
            AddSfXList(source);
    }

    #region 사운드 재생 중지
    public void StopSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }
    #endregion

    #region 오디오 소스 설정
    public void SetSoundVolume(Soundtype type, float volume, bool isMute)
    {
        if (MasterAudioMixer == null) return;

        switch (type)
        {
            case Soundtype.BGM:
                PlayerPrefsDataManager.BgmVolume = volume;
                PlayerPrefsDataManager.BgmVolumeMute = isMute;
                SetVolume("BGM", volume, isMute);

                // 미리듣기는 배경음과 볼륨이 같음
                PlayerPrefsDataManager.PreviewVolume = volume;
                SetVolume("Preview", volume, false);
                break;
            case Soundtype.SFX:
                PlayerPrefsDataManager.SFXVolume = volume;
                PlayerPrefsDataManager.SFXVolumeMute = isMute;
                SetVolume("SFX", volume, isMute);
                break;
            case Soundtype.AMB:
                PlayerPrefsDataManager.BGSVolume = volume;
                PlayerPrefsDataManager.BGSVolumeMute = isMute;
                SetVolume("AMB", volume, isMute);
                break;
        }
    }

    private void SetVolume(string mixerParam, float volume, bool isMute)
    {
        MasterAudioMixer.SetFloat(mixerParam, (volume < Mathf.Epsilon || isMute) ? -80f : Mathf.Log10(volume) * 20);
    }
    #endregion

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopSound(BgmSource);
    }

    private void AddSfXList(AudioSource sfx)
    {
        if (sfx == null) return;

        sfxList.Add(sfx);

        if (sfxPlayCoroutine == null)
        {
            sfxPlayCoroutine = StartCoroutine(Co_CheckPlayingSFX());
        }
    }

    // sfxList중 재생이 완료된 오디오 소스 정리
    private IEnumerator Co_CheckPlayingSFX()
    {
        while (sfxList.Count > 0)
        {
            for (int i = sfxList.Count - 1; i >= 0; i--)
            {
                if (!sfxList[i].isPlaying)
                {
                    soundPool.Release(sfxList[i]);
                    sfxList.RemoveAt(i);
                }
            }
            yield return clearSourceWs;
        }

        sfxPlayCoroutine = null;
    }

    #region 오디오 소스 재생 체크
    // 오디오 소스 재생 완료 체크 메서드
    private void CheckAudioPlayDone(Coroutine coroutine, AudioSource source, float length, Action action)
    {
        SafeStartCoroutine(coroutine, Co_CheckAudioPlayDone(source, length, action));
    }

    /// <summary>
    /// 오디오 소스가 재생 완료되었는지 체크하는 메서드
    /// </summary>
    /// <param name="source"> 오디오 소스 </param>
    /// <param name="length"> 최대 재생 시간 </param>
    /// <returns></returns>
    private IEnumerator Co_CheckAudioPlayDone(AudioSource source, float length, Action action)
    {
        Debug.Log($"SourceTime: {source.time}");
        Debug.Log($"Length: {length}");
        yield return new WaitUntil(() => source.time >= length);

        Debug.Log("넘어옴");
        action?.Invoke();
    }
    #endregion

    #region 재생 페이드 효과
    /// <summary>
    /// 배경음 재생 메서드
    /// 이전에 재생 중이던 배경음이 있을 시 페이드 아웃으로 서서히 볼륨을 줄인 뒤 새 배경음 재생
    /// </summary>
    /// <param name="clip"> 새로 재생할 배경음 </param>
    /// <returns></returns>
    private IEnumerator Co_PlayBGM(AudioClip clip)
    {
        float vol = PlayerPrefsDataManager.BgmVolume;

        // 이미 배경음악이 재생중일 때 볼륨을 서서히 줄임
        if (BgmSource.isPlaying)
        {
            yield return StartCoroutine(Co_FadeVolume("BGM", PlayerPrefsDataManager.BgmVolume, 0f, 0.5f));
        }

        // 배경음 재생
        SetVolume("BGM", vol, PlayerPrefsDataManager.BgmVolumeMute);
        PlaySound(Soundtype.BGM, BgmSource, clip);
        CheckAudioPlayDone(bgmPlayDoneCoroutine, BgmSource, clip.length, OnBGMPlayDone);
    }

    public void FadeOutBGMVolume(bool isPlay) => SafeStartCoroutine(PlayCoroutine, Co_FadeOutBGMVolume(isPlay));
    public void FadeInBGMVolume(bool isPlay) => SafeStartCoroutine(PlayCoroutine, Co_FadeInBGMVolume(isPlay));

    private IEnumerator Co_FadeOutBGMVolume(bool isPlay)
    {
        Coroutine preVol = StartCoroutine(Co_FadeVolume("Preview", 0f, PlayerPrefsDataManager.BgmVolume, 0.5f));

        if(isPlay)
        {
            Coroutine bgmVol = StartCoroutine(Co_FadeVolume("BGM", PlayerPrefsDataManager.BgmVolume, 0f, 0.5f));

            yield return bgmVol;
        }

        yield return preVol;

        BgmSource.Pause();
    }

    private IEnumerator Co_FadeInBGMVolume(bool isPlay)
    {
        BgmSource.Play();

        yield return StartCoroutine(Co_FadeVolume("BGM", 0f, PlayerPrefsDataManager.BgmVolume, 0.75f));
    }

    /// <summary>
    ///  볼륨 페이드 효과를 주는 코루틴
    /// </summary>
    /// <param name="audioType"> 효과를 줄 볼륨 타입 (BGM, AMB, SFX, Preview) </param>
    /// <param name="startVol"> 시작 볼륨 </param>
    /// <param name="endVol"> 최종적으로 변하고자 하는 볼륨 </param>
    /// <returns></returns>

    private IEnumerator Co_FadeVolume(string audioType, float startVol, float endVol, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float vol = Mathf.Lerp(startVol, endVol, timer / duration);
            vol = Mathf.Clamp(vol, 0f, 1f);
            SetVolume(audioType, vol, PlayerPrefsDataManager.BgmVolumeMute);

            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    private void SafeStartCoroutine(Coroutine coroutine, IEnumerator coroutineMethod)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(coroutineMethod);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (sfxPlayCoroutine != null)
        {
            StopCoroutine(sfxPlayCoroutine);
            sfxPlayCoroutine = null;
        }

        if(bgmPlayDoneCoroutine != null)
        {
            StopCoroutine(bgmPlayDoneCoroutine);
            bgmPlayDoneCoroutine = null;
        }

        if(PlayCoroutine != null)
        {
            StopCoroutine(PlayCoroutine);
            PlayCoroutine = null;
        }
    }
}
