using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class UI_AlbumDetail : MonoBehaviour
{
    [Header("음반 정보")]
    [SerializeField] private Image recordImage;             // 음반 이미지
    [SerializeField] private TMP_Text titleText;            // 음반 타이틀 텍스트
    [SerializeField] private TMP_Text artistText;           // 음반 아티스트 텍스트
    [SerializeField] private TMP_Text currentTimeText;      // 현재 재생 지점
    [SerializeField] private TMP_Text endTimeText;          // 총 재생 길이
    [SerializeField] private Toggle favoriteToggle;         // 즐겨찾기 토글

    [Header("재생바 슬라이더")]
    [SerializeField] private UI_CurrentPlaySlider currentPlaySlider;    // 재생바

    [Header("재생모드 관련 Image / Sprite")]
    [SerializeField] private Image playModeImg;

    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    [SerializeField] private Image repeatImg;
    [SerializeField] private Sprite repeatOneSprite;
    [SerializeField] private Sprite RepeatAllSprite;

    [SerializeField] private Toggle shuffleToggle;

    [Header("BGM 재생 리스트 클래스")]
    [SerializeField] private UI_BGMList bgmList;

    [SerializeField] private AudioClip sfxClip;

    private RecordDataSO record;

    private Coroutine playRecordCoroutine;
    private WaitForSeconds playRecordWs = new WaitForSeconds(0.1f);

    private bool isDragging;

    private bool isInitialize;

    public bool IsFavorite { get; private set; }    // 즐겨찾기 여부

    private void Start()
    {
        currentPlaySlider.OnMouseDown += StartDrag;
        currentPlaySlider.OnMouseUp += EndDrag;
    }

    /// <summary>
    /// 음반 데이터 설정 및 UI 값 설정
    /// </summary>
    /// <param name="record"> 설정할 음반 데이터 </param>
    public void SetRecordData(RecordDataSO record)
    {
        if (record == null) return;

        isInitialize = true;

        this.record = record;

        recordImage.sprite = record.RecordImgPath_Sprite;
        playModeImg.sprite = pauseSprite;
        titleText.text = record.RecordName_String;
        artistText.text = record.RecordArtist_String;
        currentPlaySlider.PlaySlider.value = 0f;
        currentTimeText.text = SoundManager.Instance.BgmSource.GetSourceLength();
        endTimeText.text = record.RecordSoundPath_AudioClip.GetClipLength();
        favoriteToggle.isOn = DataManager.Instance.RecordDatabase.BookmarkRecords.Contains(record.RecordID);

        CheckPlayTime();

        isInitialize = false;
    }

    private void LocalizeRecordInfo()
    {
        titleText.text = record.RecordName_String;
        artistText.text = record.RecordArtist_String;
    }

    private void CheckPlayTime()
    {
        if (!gameObject.activeInHierarchy) return;

        if (playRecordCoroutine != null)
        {
            StopCoroutine(playRecordCoroutine);
            playRecordCoroutine = null;
        }

        playRecordCoroutine = StartCoroutine(Co_CheckPlayTime());
    }

    /// <summary>
    /// 현재 재생중인 노래의 재생시간을 갱신하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator Co_CheckPlayTime()
    {
        while (true)
        {
            float currentTime = SoundManager.Instance.BgmSource.time;
            float totalTime = record.RecordSoundPath_AudioClip.length;

            if (!isDragging)
            {
                currentPlaySlider.PlaySlider.value = Mathf.Clamp(currentTime / totalTime, 0f, 1f);
                currentTimeText.text = SoundManager.Instance.BgmSource.GetSourceLength();
            }

            if (currentTime / totalTime >= 1)
                yield break;

            yield return playRecordWs;
        }
    }

    public void OnValueChanged_FavoriteToggle()
    {
        if (!isInitialize)
            SoundManager.Instance.PlaySFX(sfxClip);

        IsFavorite = favoriteToggle.isOn;

        if (favoriteToggle.isOn)
        {
            if (!DataManager.Instance.RecordDatabase.BookmarkRecords.Contains(record.RecordID))
            {
                DataManager.Instance.RecordDatabase.BookmarkRecords.Add(record.RecordID);
            }
        }
        else
        {
            if (DataManager.Instance.RecordDatabase.BookmarkRecords.Contains(record.RecordID))
            {
                DataManager.Instance.RecordDatabase.BookmarkRecords.Remove(record.RecordID);
            }
        }

        bgmList.UpdateFavoriteRecord(record);
    }

    private void StartDrag()
    {
        isDragging = true;
    }

    private void EndDrag()
    {
        isDragging = false;

        SoundManager.Instance.BgmSource.time = Mathf.Clamp(
            currentPlaySlider.PlaySlider.value * record.RecordSoundPath_AudioClip.length,
            0f,
            record.RecordSoundPath_AudioClip.length - 0.1f);

        CheckPlayTime();
    }

    #region 재생 모드 컨트롤 관련
    public void OnClick_PlayButton(int type) => bgmList.Click_PlayButton(type);
    public void OnClick_Resume()
    {
        if (SoundManager.Instance.BgmSource.isPlaying)
            SoundManager.Instance.BgmSource.Pause();
        else
            SoundManager.Instance.BgmSource.Play();

        playModeImg.sprite = SoundManager.Instance.BgmSource.isPlaying ? pauseSprite : playSprite;
    }

    public void OnClick_RepeatButton()
    {
        repeatImg.sprite = bgmList.IsPlayRepeat ? repeatOneSprite : RepeatAllSprite;
    }
    #endregion

    public void OnClick_AddPlaylist()
    {
        bgmList.AddPlaylist(record);
    }


    private void OnEnable()
    {
        if (record != null)
            CheckPlayTime();

        PlayerPrefsDataManager.OnLanguageChanged += LocalizeRecordInfo;

        if (record != null)
        {
            LocalizeRecordInfo();
        }

        shuffleToggle.isOn = bgmList.IsPlayShuffle;
    }

    private void OnDisable()
    {
        if (playRecordCoroutine != null)
        {
            StopCoroutine(playRecordCoroutine);
            playRecordCoroutine = null;
        }

        PlayerPrefsDataManager.OnLanguageChanged -= LocalizeRecordInfo;
    }

    private void OnDestroy()
    {
        currentPlaySlider.OnMouseDown -= StartDrag;
        currentPlaySlider.OnMouseUp -= EndDrag;
    }
}
