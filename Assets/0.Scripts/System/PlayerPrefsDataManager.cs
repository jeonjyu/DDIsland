using System;
using Unity.VisualScripting;
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
    private const string KEY_BGSVOLUME = "BgsVolume";                   // BGS 볼륨 키
    private const string KEY_BGMVOLUME_MUTE = "BGMVolumeMute";          // BGM 뮤트 키
    private const string KEY_SFXVOLUME_MUTE = "SFXVolumeMute";          // SFX 뮤트 키
    private const string KEY_BGSVOLUME_MUTE = "BGSVolumeMute";          // BGS 뮤트 키
    private const string KEY_GRAPHICQUALITY = "GraphicQuality";         // 그래픽 품질 설정 키
    private const string KEY_RESOLUTIONWIDTH = "ResolutionWidth";       // 가로 해상도 수치 키
    private const string KEY_RESOLUTIONHEIGHT = "ResolutionHeight";     // 세로 해상도 수치 키
    private const string KEY_RESOLUTIONWINDOW = "ResolutionWindow";     // 세로 해상도 수치 키
    private const string KEY_RESOLUTIONHZ = "ResolutionHz";             // 주사율
    private const string KEY_MouseSensivity = "MouseSensitivity";       // 마우스 감도 키
    #endregion

    public static event Action OnLanguageChanged;

    public static int Language   // StringDataSO 타입과 호환
    {
        get { return PlayerPrefs.GetInt(KEY_LANGUAGE, 0); }

        set
        {
            if (value == PlayerPrefs.GetInt(KEY_LANGUAGE, 0)) return;

            PlayerPrefs.SetInt(KEY_LANGUAGE, value);
            OnLanguageChanged?.Invoke();
        }
    }

    public static float BgmVolume
    {
        get { return PlayerPrefs.GetFloat(KEY_BGMVOLUME, 1f); }

        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_BGMVOLUME, vol);
        }
    }

    public static float SFXVolume
    {
        get { return PlayerPrefs.GetFloat(KEY_SFXVOLUME, 1f); }

        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_SFXVOLUME, vol);
        }
    }

    public static float BGSVolume
    {
        get { return PlayerPrefs.GetFloat(KEY_BGSVOLUME, 1f); }
        set
        {
            float vol = Mathf.Clamp(value, 0f, 1f);
            PlayerPrefs.SetFloat(KEY_BGSVOLUME, vol);
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
            Debug.Log("저장");
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

    public static bool BGSVolumeMute
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_BGSVOLUME_MUTE, 0) == 1 ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt(KEY_BGSVOLUME_MUTE, value == true ? 1 : 0);
        }
    }

    public static int GraphicQuality
    {
        get { return PlayerPrefs.GetInt(KEY_GRAPHICQUALITY, QualitySettings.GetQualityLevel()); }

        set
        {
            int index = Mathf.Clamp(value, 0, QualitySettings.names.Length - 1);
            PlayerPrefs.SetInt(KEY_GRAPHICQUALITY, index);
        }
    }

    public static float MouseSensitivity
    {
        get { return PlayerPrefs.GetFloat(KEY_MouseSensivity, 50f); }

        set
        {
            float sensitive = Mathf.Clamp(value, 0f, 100f);
            PlayerPrefs.SetFloat(KEY_MouseSensivity, sensitive);
        }
    }

    public static int ResolutionWidth
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_RESOLUTIONWIDTH);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_RESOLUTIONWIDTH, value);
        }
    }
    public static int ResolutionHeight
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_RESOLUTIONHEIGHT);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_RESOLUTIONHEIGHT, value);
        }
    }
    public static float ResolutionHz
    {
        get
        {
            return PlayerPrefs.GetFloat(KEY_RESOLUTIONHZ);
        }
        set
        {
            PlayerPrefs.SetFloat(KEY_RESOLUTIONHZ, value);
        }
    }

    public static int ResolutionWindow
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_RESOLUTIONWINDOW);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_RESOLUTIONWINDOW, value);
        }
    }
}
