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

    [Header("화면 설정 관련")]
    [SerializeField] private Toggle topMostToggle;

    private bool isInitializing = false;

    private void Start()
    {
        isInitializing = true;

        languageDropdown.value = PlayerPrefsDataManager.Language;

        bgmSlider.value = PlayerPrefsDataManager.BgmVolume;
        sfxSlider.value = PlayerPrefsDataManager.SFXVolume;
        ambSlider.value = PlayerPrefsDataManager.BGSVolume;

        bgmToggle.isOn = PlayerPrefsDataManager.BgmVolumeMute;
        sfxToggle.isOn = PlayerPrefsDataManager.SFXVolumeMute;
        ambToggle.isOn = PlayerPrefsDataManager.BGSVolumeMute;

        OnValueChangedVolume(bgmSlider, bgmVolumeText);
        OnValueChangedVolume(sfxSlider, sfxVolumeText);
        OnValueChangedVolume(ambSlider, ambVolumeText);

        isInitializing = false;
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

    #region 화면 설정
    public void OnValueChanged_WindowTopMost()
    {
        WindowController.Instance.IsTopmost = topMostToggle.isOn;
    }

    #endregion

    #region 하단 버튼 (저장 / 게임 종료)
    public void OnClick_SaveData()
    {
        // todo: 유저 서버 데이터 저장 코드 작성
    }

    public void OnClick_GameExit()
    {
        Application.Quit();
    }
    #endregion
}
