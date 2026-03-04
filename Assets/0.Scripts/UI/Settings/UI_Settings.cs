using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [Header("설정 패널")]
    [SerializeField] private GameObject settingPanel;

    [Header("언어 관련")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("사운드 관련")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgsSlider;

    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle bgsToggle;

    [SerializeField] private TMP_Text bgmVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;
    [SerializeField] private TMP_Text bgsVolumeText;

    private bool isInitializing = false;

    private void Start()
    {
        isInitializing = true;

        languageDropdown.value = PlayerPrefsDataManager.Language;

        bgmSlider.value = PlayerPrefsDataManager.BgmVolume;
        sfxSlider.value = PlayerPrefsDataManager.SFXVolume;
        bgsSlider.value = PlayerPrefsDataManager.BGSVolume;

        bgmToggle.isOn = PlayerPrefsDataManager.BgmVolumeMute;
        sfxToggle.isOn = PlayerPrefsDataManager.SFXVolumeMute;
        bgsToggle.isOn = PlayerPrefsDataManager.BGSVolumeMute;

        OnValueChangedVolume(bgmSlider, bgmVolumeText);
        OnValueChangedVolume(sfxSlider, sfxVolumeText);
        OnValueChangedVolume(bgsSlider, bgsVolumeText);

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

    public void OnValueChangedBGSVolume()
    {
        if (isInitializing == true) return;

        OnValueChangedVolume(bgsSlider, bgsVolumeText);
        SoundManager.Instance.SetSoundVolume(Soundtype.BGS, bgsSlider.value, bgsToggle.isOn);
    }
    #endregion
}
