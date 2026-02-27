using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
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
    [SerializeField] private AudioClip test;

    [Header("오디오 믹서")]
    [field: SerializeField] public AudioMixer MasterAudioMixer { get; private set; }

    [Header("브금을 재생할 오디오 소스")]
    [field: SerializeField] public AudioSource BgmSource { get; private set; }
    [field: SerializeField] public AudioSource BgsSource { get; private set; }

    [Header("효과음을 재생할 오디오 소스 프리팹")]
    [SerializeField] private AudioSource sfxSource;

    [Header("사운드 풀")]
    [SerializeField] private SoundPool soundPool;

    [Header("오디오 클립 모음 SO 파일")]
    [SerializeField] private AudioClipSO audioClipSO;

    // 배경음, 효과음 오디오 클립을 저장할 딕셔너리
    private Dictionary<string, AudioClip> bgmDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> bgsDic = new Dictionary<string, AudioClip>();

    private List<AudioSource> sfxList = new List<AudioSource>();    // 현재 재생중인 효과음 오디오 소스를 가지고 있을 리스트
    private Coroutine sfxPlayCoroutine;                             // 현재 재생중인 효과음 오디오 소스들을 체크하는 코루틴 변수

    [SerializeField] private float clearSourceTime = 0.5f;
    private WaitForSeconds clearSourceWs;

    protected override void Awake()
    {
        base.Awake();
        InitData();
    }

    private void Update()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame)
            PlaySFX(test);
    }

    #region Init
    // 데이터 초기화
    private void InitData()
    {
        if (audioClipSO.BgmClips == null || audioClipSO.SfxClips == null)
        {
            return;
        }

        // 배경음 딕셔너리에 오디오 클립 추가
        audioClipSO.GetClips(audioClipSO.BgmClips, bgmDic);
        // 효과음 딕셔너리에 오디오 클립 추가
        audioClipSO.GetClips(audioClipSO.SfxClips, sfxDic);
        // 환경음 딕셔너리에 오디오 클립 추가
        audioClipSO.GetClips(audioClipSO.BgsClips, bgsDic);

        clearSourceWs = new WaitForSeconds(clearSourceTime);
    }
    #endregion

    #region 오디오 클립을 받아와 재생
    public void PlayBGM(AudioClip clip)
    {
        PlaySound(Soundtype.BGM, BgmSource, clip);
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

    #region 오디오 클립 이름(string)을 받아와 재생
    public void PlayBGM(string bgmName)
    {
        if (BgmSource == null || !bgmDic.TryGetValue(bgmName, out var clip)) return;

        PlaySound(Soundtype.BGM, BgmSource, clip);
    }

    public void PlayBGS(string bgsName)
    {
        if (BgsSource == null || !bgsDic.TryGetValue(bgsName, out var clip)) return;

        PlaySound(Soundtype.BGS, BgsSource, clip);
    }

    public void PlaySFX(string sfxName)
    {
        AudioSource source = soundPool.Get(sfxSource);

        if (source == null || !sfxDic.TryGetValue(sfxName, out var clip)) return;

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
    public void SetSoundVolume(Soundtype type, float volume, bool mute = false)
    {
        if (MasterAudioMixer == null) return;

        switch (type)
        {
            case Soundtype.BGM:
                SetVolume("BGM", volume);
                if (!mute)
                    PlayerPrefsDataManager.BgmVolume = volume;
                break;
            case Soundtype.SFX:
                SetVolume("SFX", volume);
                if (!mute)
                    PlayerPrefsDataManager.SFXVolume = volume;
                break;
            case Soundtype.BGS:
                SetVolume("BGS", volume);
                if (!mute)
                    PlayerPrefsDataManager.BGSVolume = volume;
                break;
        }
    }

    private void SetVolume(string mixerParam, float volume)
    {
        MasterAudioMixer.SetFloat(mixerParam, Mathf.Log10(volume) * 20);
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
            sfxPlayCoroutine = StartCoroutine(CheckPlayingSFX());
        }
    }

    // sfxList중 재생이 완료된 오디오 소스 정리
    private IEnumerator CheckPlayingSFX()
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (sfxPlayCoroutine != null)
            StopCoroutine(sfxPlayCoroutine);
    }
}
