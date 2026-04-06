using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CurrentPlaylist : MonoBehaviour
{
    [SerializeField] private UI_CurrentPlaylistSlot slot;
    [SerializeField] private Transform slotTrans;

    [SerializeField] private TMP_Text currentTimeText;      // 현재 재생 지점
    [SerializeField] private TMP_Text endTimeText;          // 총 재생 길이

    [SerializeField] private TMP_Text playlistTitleText;

    [SerializeField] private UI_Playlist ui_Playlist;

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

    private List<UI_CurrentPlaylistSlot> slots = new List<UI_CurrentPlaylistSlot>();

    private RecordDataSO record;

    private bool isDragging;

    private Coroutine playRecordCoroutine;
    private WaitForSeconds playRecordWs = new WaitForSeconds(0.1f);

    private PlaylistData playlist;

    private void Start()
    {
        currentPlaySlider.OnMouseDown += StartDrag;
        currentPlaySlider.OnMouseUp += EndDrag;
    }

    public void ShowCurrentPlaylist(PlaylistData playlist)
    {
        this.playlist = playlist;
        playlistTitleText.text = playlist.Name;

        if(slots.Count > 0)
        {
            foreach(var slot in slots)
            {
                Destroy(slot.gameObject);
            }

            slots.Clear();
        }

        foreach(var recordId in playlist.RecordLists)
        {
            UI_CurrentPlaylistSlot currentSlot = Instantiate(slot, slotTrans);
            currentSlot.SetRecordInit(DataManager.Instance.RecordDatabase.RecordInfoData[recordId], bgmList);
            slots.Add(currentSlot);
        }

        if(DataManager.Instance.RecordDatabase.CurrentPlaylistId != playlist.Id && playlist.RecordLists.Count > 0)
        {
            DataManager.Instance.RecordDatabase.CurrentPlaylistId = playlist.Id;
            bgmList.PlayBGM(DataManager.Instance.RecordDatabase.RecordInfoData[DataManager.Instance.RecordDatabase.CurrentPlayList[0]]);

            if (PlayerPrefsDataManager.PlayDefaultRecord)
                PlayerPrefsDataManager.PlayDefaultRecord = false;
        }
    }

    public void SetRecordData(RecordDataSO record)
    {
        this.record = record;
        endTimeText.text = record.RecordSoundPath_AudioClip.GetClipLength();
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
    #endregion

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

    public void OnClick_DeleteSlots()
    {
        for(int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].IsDeleteCheck)
            {
                playlist.RecordLists.Remove(slots[i].Record.RecordID);
                Destroy(slots[i].gameObject);
                slots.Remove(slots[i]);
            }
        }

        ui_Playlist.slotList[ui_Playlist.slotList.FindIndex(x => x.Playlist == playlist)].PlaylistSlotInit(playlist, ui_Playlist);
    }

    public void OnClick_RepeatButton()
    {
        repeatImg.sprite = bgmList.IsPlayRepeat ? repeatOneSprite : RepeatAllSprite;
    }

    private void OnEnable()
    {
        if (record != null)
            CheckPlayTime();

        shuffleToggle.isOn = bgmList.IsPlayShuffle;
    }

    private void OnDisable()
    {
        if (playRecordCoroutine != null)
        {
            StopCoroutine(playRecordCoroutine);
            playRecordCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        currentPlaySlider.OnMouseDown -= StartDrag;
        currentPlaySlider.OnMouseUp -= EndDrag;
    }
}
