using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [Header("설정 패널")]
    [SerializeField] private GameObject settingPanel;

    [Header("투명도 관련")]
    [SerializeField] private Image transparencyPanel;
    [SerializeField] private Slider transparencySlider;
    [SerializeField] private TMP_Text transparencyText;
    [SerializeField] private float minValue = 0.3f;
    [SerializeField] private float maxValue = 1.0f;

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

        OnValueChangedVolume(transparencySlider, transparencyText);
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

    #region
    public void OnValueChangedTransparency()
    {
        if (isInitializing == true) return;

        OnValueChangedVolume(transparencySlider, transparencyText);

        Color color = transparencyPanel.color;
        color.a = Mathf.Clamp(transparencySlider.value, minValue, maxValue);
        transparencyPanel.color = color;

        PlayerPrefsDataManager.Transparency = transparencyPanel.color.a;
    }
    #endregion

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
        SoundManager.Instance.SetSoundVolume(Soundtype.AMB, bgsSlider.value, bgsToggle.isOn);
    }
    #endregion
}
