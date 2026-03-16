using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PlayRecordInfo : MonoBehaviour
{
    [Header("음반 정보")]
    [SerializeField] private Image recordImage;             // 음반 이미지
    [SerializeField] private TMP_Text titleText;            // 음반 타이틀 텍스트
    [SerializeField] private TMP_Text artistText;           // 음반 아티스트 텍스트
    [SerializeField] private Slider processBar;             // 재생바
    [SerializeField] private TMP_Text currentTimeText;      // 현재 재생 지점
    [SerializeField] private TMP_Text endTimeText;          // 총 재생 길이

    [Header("재생바 슬라이더")]
    [SerializeField] private UI_CurrentPlaySlider currentPlaySlider;

    private RecordDataSO record;

    private Coroutine playRecordCoroutine;
    private WaitForSeconds playRecordWs = new WaitForSeconds(0.5f);

    private bool isDragging;


    private void Start()
    {
        Debug.Log(currentPlaySlider);
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

        this.record = record;

        recordImage.sprite = record.RecordImgPath_Sprite;
        titleText.text = record.RecordName_String;
        artistText.text = record.RecordArtist_String;
        processBar.value = 0f;
        currentTimeText.text = SoundManager.Instance.BgmSource.GetSourceLength();
        endTimeText.text = record.RecordSoundPath_AudioClip.GetClipLength();

        if(playRecordCoroutine != null)
        {
            StopCoroutine(playRecordCoroutine);
            playRecordCoroutine = null;
        }

        playRecordCoroutine = StartCoroutine(CheckPlayTime());
    }

    /// <summary>
    /// 현재 재생중인 노래의 재생시간을 갱신하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPlayTime()
    {
        while(true)
        {
            float currentTime = SoundManager.Instance.BgmSource.time;
            float totalTime = record.RecordSoundPath_AudioClip.length;

            if (!isDragging)
            {
                processBar.value = Mathf.Clamp(currentTime / totalTime, 0f, 1f);
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
            processBar.value * record.RecordSoundPath_AudioClip.length,
            0f,
            record.RecordSoundPath_AudioClip.length - 0.1f);
    }

    private void OnDestroy()
    {
        currentPlaySlider.OnMouseDown -= StartDrag;
        currentPlaySlider.OnMouseUp -= EndDrag;
    }
}
