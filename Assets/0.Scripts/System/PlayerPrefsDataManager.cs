using System;
using UnityEngine;

/// <summary>
/// PlayerPrefs로 데이터 저장
/// 유저 아이디, 언어 설정, 음악 볼륨 설정, 기타 볼륨 설정, 품질 설정, 해상도 수치
/// </summary>
public static class PlayerPrefsDataManager
{
    #region PlayerPrefs Key
    private const string KEY_LANGUAGE = "Language";                     // 언어 설정 키
    private const string KEY_BGMVOLUME = "BgmVolume";                   // BGM 볼륨 키
    private const string KEY_SFXVOLUME = "SfxVolume";                   // SFX 볼륨 키
    private const string KEY_AMBVOLUME = "AmbVolume";                   // BGS 볼륨 키
    private const string KEY_PREVIEWVOLUME = "PreviewVolume";           // Preview 볼륨 키
    private const string KEY_BGMVOLUME_MUTE = "BGMVolumeMute";          // BGM 뮤트 키
    private const string KEY_SFXVOLUME_MUTE = "SFXVolumeMute";          // SFX 뮤트 키
    private const string KEY_AMBVOLUME_MUTE = "AMBVolumeMute";          // BGS 뮤트 키
    private const string KEY_BACKGROUNDPLAY = "BackgroundPlay";       // 백그라운드 소리 재생 여부
    private const string KEY_PLAYDEFAULTRECORD = "PlayDefaultRecord";   // 기본 음반 자동 재생 여부
    private const string KEY_TOPMOST = "TopMost";                     // 게임창 최상단 고정 여부
    #endregion

    public static event Action OnLanguageChanged;
    public static event Action<bool> OnPlayDefaultRecord;

    public static int Language   // StringDataSO 타입과 호환, (0 = 한국어, 1 = 영어)
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_LANGUAGE, 0);
        }

        set
        {
            if (value == PlayerPrefs.GetInt(KEY_LANGUAGE, 0)) return;

            PlayerPrefs.SetInt(KEY_LANGUAGE, value);
            OnLanguageChanged?.Invoke();
        }
    }

    public static float BgmVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(KEY_BGMVOLUME, 1f);
        }

        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_BGMVOLUME, vol);
        }
    }

    public static float SFXVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(KEY_SFXVOLUME, 1f);
        }

        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_SFXVOLUME, vol);
        }
    }

    public static float AMBVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(KEY_AMBVOLUME, 1f);
        }

        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_AMBVOLUME, vol);
        }
    }

    public static float PreviewVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(KEY_PREVIEWVOLUME, 1f);
        }

        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_PREVIEWVOLUME, vol);
        }
    }

    public static bool BgmVolumeMute
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_BGMVOLUME_MUTE, 0) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(KEY_BGMVOLUME_MUTE, value == true ? 1 : 0);
        }
    }

    public static bool SFXVolumeMute
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_SFXVOLUME_MUTE, 0) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(KEY_SFXVOLUME_MUTE, value == true ? 1 : 0);
        }
    }

    public static bool AMBVolumeMute
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_AMBVOLUME_MUTE, 0) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(KEY_AMBVOLUME_MUTE, value == true ? 1 : 0);
        }
    }

    public static bool BackgroundPlay
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_BACKGROUNDPLAY, 1) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(KEY_BACKGROUNDPLAY, value == true ? 1 : 0);
        }
    }

    public static bool PlayDefaultRecord
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_PLAYDEFAULTRECORD, 1) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(KEY_PLAYDEFAULTRECORD, value == true ? 1 : 0);
            OnPlayDefaultRecord?.Invoke(value);
        }
    }

    public static bool TopMost
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_TOPMOST, 1) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(KEY_TOPMOST, value == true ? 1 : 0);
        }
    }
}
