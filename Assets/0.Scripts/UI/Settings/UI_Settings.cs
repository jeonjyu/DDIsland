using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [Header("설정창 패널")]
    [SerializeField] private GameObject settingPanel;

    [Header("언어 설정 관련")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("사운드 설정 관련")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider ambSlider;

    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle ambToggle;

    [SerializeField] private TMP_Text bgmVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;
    [SerializeField] private TMP_Text ambVolumeText;

    [Header("음반 설정 관련")]
    [SerializeField] private Toggle backgroundPlayToggle;
    [SerializeField] private Toggle playDefaultRecordToggle;

    [Header("화면 설정 관련")]
    [SerializeField] private TMP_Dropdown monitorDropdown;      // 게임 플레이 모니터 설정 드랍다운
    [SerializeField] private Toggle topMostToggle;              // 게임 화면 최상단 여부 설정 토글

    private bool isInitializing = false;

    private void Start()
    {
        SettingInit();
    }

    private void SettingInit()
    {
        isInitializing = true;

        // 언어 설정 초기화
        languageDropdown.value = PlayerPrefsDataManager.Language;

        // 사운드 설정 초기화
        bgmSlider.value = PlayerPrefsDataManager.BgmVolume;
        sfxSlider.value = PlayerPrefsDataManager.SFXVolume;
        ambSlider.value = PlayerPrefsDataManager.AMBVolume;

        bgmToggle.isOn = PlayerPrefsDataManager.BgmVolumeMute;
        sfxToggle.isOn = PlayerPrefsDataManager.SFXVolumeMute;
        ambToggle.isOn = PlayerPrefsDataManager.AMBVolumeMute;

        OnValueChangedVolume(bgmSlider, bgmVolumeText);
        OnValueChangedVolume(sfxSlider, sfxVolumeText);
        OnValueChangedVolume(ambSlider, ambVolumeText);

        backgroundPlayToggle.isOn = PlayerPrefsDataManager.BackgroundPlay;
        playDefaultRecordToggle.isOn = PlayerPrefsDataManager.PlayDefaultRecord;

        topMostToggle.isOn = PlayerPrefsDataManager.TopMost;

        int monitorCount = WindowController.Instance.GetMonitorCount();

        monitorDropdown.options.Clear();

        List<string> options = new List<string>();

        for(int i = 0; i < monitorCount; i++)
        {
            if(PlayerPrefsDataManager.Language == 0)
            {
                options.Add($"모니터 {i}");
            }
            else
            {
                options.Add($"Monitor {i}");
            }
        }

        if(options.Count > 0)
        {
            monitorDropdown.AddOptions(options);
            monitorDropdown.value = WindowController.Instance.GetCurrentMonitor();
        }

        isInitializing = false;

        PlayerPrefsDataManager.OnPlayDefaultRecord += OnDefaultRecordChanged;
    }


    public void OnOffUISettings(bool isOn)
    {
        if (settingPanel == null) return;

        if(settingPanel.activeSelf != isOn)
            settingPanel.SetActive(isOn);
    }

    #region 언어 설정
    public void OnValueChangedLanguage()
    {
        PlayerPrefsDataManager.Language = languageDropdown.value;

        int monitorCount = WindowController.Instance.GetMonitorCount();

        monitorDropdown.options.Clear();

        List<string> options = new List<string>();

        for (int i = 0; i < monitorCount; i++)
        {
            if (PlayerPrefsDataManager.Language == 0)
            {
                options.Add($"모니터 {i}");
            }
            else
            {
                options.Add($"Monitor {i}");
            }
        }

        if (options.Count > 0)
        {
            monitorDropdown.AddOptions(options);
            monitorDropdown.value = WindowController.Instance.GetCurrentMonitor();
        }
    }
    #endregion

    #region 사운드 볼륨 설정
    public void OnValueChangedVolume(Slider slider, TMP_Text text)
    {
        text.text = $"{Mathf.RoundToInt(slider.value * 100f).ToString()}%";
    }

    public void OnValueChangedBGMVolume()
    {
        if (isInitializing == true) return;

        OnValueChangedVolume(bgmSlider, bgmVolumeText);
        SoundManager.Instance.SetSoundVolume(Soundtype.BGM, bgmSlider.value, bgmToggle.isOn);
    }

    public void OnValueChangedSFXVolume()
    {
        if (isInitializing == true) return;

        OnValueChangedVolume(sfxSlider, sfxVolumeText);
        SoundManager.Instance.SetSoundVolume(Soundtype.SFX, sfxSlider.value, sfxToggle.isOn);
    }

    public void OnValueChangedAMBVolume()
    {
        if (isInitializing == true) return;

        OnValueChangedVolume(ambSlider, ambVolumeText);
        SoundManager.Instance.SetSoundVolume(Soundtype.AMB, ambSlider.value, ambToggle.isOn);
    }
    #endregion

    #region 음반 설정
    public void OnValueChanged_BackgroundPlay()
    {
        PlayerPrefsDataManager.BackgroundPlay = backgroundPlayToggle.isOn;
    }

    public void OnValueChanged_PlayDefaultRecord()
    {
        if (isInitializing) return;

        PlayerPrefsDataManager.PlayDefaultRecord = playDefaultRecordToggle.isOn;
        if(playDefaultRecordToggle.isOn)
        {
            DataManager.Instance.RecordDatabase.CurrentPlaylistId = 0;
        }
    }
    #endregion

    #region 화면 설정
    public void OnValueChanged_SwitchMonitor()
    {
        if (isInitializing) return;

        WindowController.Instance.ChangeMonitor(monitorDropdown.value);
    }

    public void OnValueChanged_WindowTopMost()
    {
        WindowController.Instance.IsTopmost = topMostToggle.isOn;
        PlayerPrefsDataManager.TopMost = topMostToggle.isOn;
    }

    #endregion

    #region 하단 버튼 (저장 / 게임 종료)
    public async void OnClick_SaveData()
    {
        await DataManager.Instance.Hub.UploadAllData();
    }

    public void OnClick_GameExit()
    {
        _ = DataManager.Instance.Hub.UploadAllData();
        Application.Quit();
    }
    #endregion

    private void OnDefaultRecordChanged(bool isOn)
    {
        isInitializing = true;

        playDefaultRecordToggle.isOn = isOn;

        isInitializing = false;
    }

    public void PlayRecordSfx(AudioClip clip)
    {
        if (isInitializing) return;

        SoundManager.Instance.PlaySFX(clip);
    }

    private void OnDestroy()
    {
        PlayerPrefsDataManager.OnPlayDefaultRecord -= OnDefaultRecordChanged;
    }
}
