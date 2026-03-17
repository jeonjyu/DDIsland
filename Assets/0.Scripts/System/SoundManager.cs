using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum Soundtype
{
    None,
    BGM,        // 배경음
    SFX,        // 효과음
    BGS,        // 환경음
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서")]
    [field: SerializeField] public AudioMixer MasterAudioMixer { get; private set; }

    [Header("브금을 재생할 오디오 소스")]
    [field: SerializeField] public AudioSource BgmSource { get; private set; }
    [field: SerializeField] public AudioSource BgsSource { get; private set; }

    [Header("효과음을 재생할 오디오 소스 프리팹")]
    [SerializeField] private AudioSource sfxSource;

    [Header("사운드 풀")]
    [SerializeField] private SoundPool soundPool;

    private List<AudioSource> sfxList = new List<AudioSource>();    // 현재 재생중인 효과음 오디오 소스를 가지고 있을 리스트
    private Coroutine sfxPlayCoroutine;                             // 현재 재생중인 효과음 오디오 소스들을 체크하는 코루틴 변수
    [SerializeField] private float clearSourceTime = 0.5f;
    private WaitForSeconds clearSourceWs;

    private Coroutine BGMPlayDoneCoroutine;         // 배경음 재생이 끝났는지 체크하는 코루틴 변수
    public event Action OnBGMPlayDone;              // 배경음 재생이 끝나면 실행할 이벤트

    protected override void Awake()
    {
        base.Awake();
        InitData();
    }

    private void Start()
    {
        SetSoundVolume(Soundtype.BGM, PlayerPrefsDataManager.BgmVolume, PlayerPrefsDataManager.BgmVolumeMute);
        SetSoundVolume(Soundtype.SFX, PlayerPrefsDataManager.SFXVolume, PlayerPrefsDataManager.SFXVolumeMute);
        SetSoundVolume(Soundtype.BGS, PlayerPrefsDataManager.BGSVolume, PlayerPrefsDataManager.BGSVolumeMute);
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
        PlaySound(Soundtype.BGM, BgmSource, clip);
        CheckBGMPlayDone();
    }

    public void PlayBGS(AudioClip clip)
    {
        PlaySound(Soundtype.BGS, BgsSource, clip);
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
    #endregion

    private void PlaySound(Soundtype type, AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;

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
                break;
            case Soundtype.SFX:
                PlayerPrefsDataManager.SFXVolume = volume;
                PlayerPrefsDataManager.SFXVolumeMute = isMute;
                SetVolume("SFX", volume, isMute);
                break;
            case Soundtype.BGS:
                PlayerPrefsDataManager.BGSVolume = volume;
                PlayerPrefsDataManager.BGSVolumeMute = isMute;
                SetVolume("BGS", volume, isMute);
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

    private void CheckBGMPlayDone()
    {
        if(BGMPlayDoneCoroutine != null)
        {
            StopCoroutine(BGMPlayDoneCoroutine);
            BGMPlayDoneCoroutine = null;
        }

        BGMPlayDoneCoroutine = StartCoroutine(Co_CheckBGMPlayDone());
    }

    private IEnumerator Co_CheckBGMPlayDone()
    {
        yield return new WaitUntil(() => BgmSource.time >= BgmSource.clip.length && !BgmSource.isPlaying);

        OnBGMPlayDone?.Invoke();
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

        if(BGMPlayDoneCoroutine != null)
        {
            StopCoroutine(BGMPlayDoneCoroutine);
            BGMPlayDoneCoroutine = null;
        }
    }
}
