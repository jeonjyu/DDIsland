using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecordPreview : MonoBehaviour
{
    [Header("미리듣기 재생바 슬라이더")]
    [SerializeField] private UI_CurrentPlaySlider previewSlider;

    [Header("현재 재생 시점 텍스트")]
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text endTimeText;

    [Header("미리듣기 최대 시간(초)")]
    [SerializeField] private float maxPreviewTime = 60f;

    [Header("재생모드 관련 Image / Sprite")]
    [SerializeField] private Image playModeImg;

    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    private RecordDataSO record;

    private Coroutine playRecordCoroutine;
    private WaitForSeconds playRecordWs = new WaitForSeconds(0.1f);

    private bool isDragging;

    private void Awake()
    {
        endTimeText.text = string.Format("{0:00}:{1:00}", maxPreviewTime / 60, maxPreviewTime % 60);
    }

    private void Start()
    {
        if (previewSlider != null)
        {
            previewSlider.OnMouseDown += StartDrag;
            previewSlider.OnMouseUp += EndDrag;
        }
    }

    public void PreviewInit(RecordDataSO record)
    {
        if (record == null) return;

        SoundManager.Instance.PlayPreview(record.RecordSoundPath_AudioClip, maxPreviewTime);

        this.record = record;
        previewSlider.PlaySlider.value = 0f;
        currentTimeText.text = SoundManager.Instance.previewSource.GetSourceLength();
        playModeImg.sprite = pauseSprite;

        CheckPlayTime();
    }

    // 미리 듣기 재생이 끝났을 때 실행할 콜백 함수
    private void PreviewEnd()
    {
        playModeImg.sprite = pauseSprite;
    }

    public void OnClick_Resume()
    {
        AudioSource source = SoundManager.Instance.previewSource;

        // 미리듣기가 끝났을 때 재생 버튼을 누르면 처음부터 시작
        if (source.time >= maxPreviewTime && !source.isPlaying)
        {
            SoundManager.Instance.PlayPreview(record.RecordSoundPath_AudioClip, maxPreviewTime);
        }
        else
        {
            // 플레이 도중에 재생 버튼을 누르면 그 자리에서 일시 정지 혹은 플레이
            if (source.isPlaying)
                source.Pause();
            else
                source.Play();
        }

        playModeImg.sprite = source.isPlaying ? pauseSprite : playSprite;
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
            float currentTime = SoundManager.Instance.previewSource.time;
            float totalTime = record.RecordSoundPath_AudioClip.length;

            if (!isDragging)
            {
                previewSlider.PlaySlider.value = Mathf.Clamp(currentTime / totalTime, 0f, 1f);
                currentTimeText.text = SoundManager.Instance.previewSource.GetSourceLength();
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

        SoundManager.Instance.previewSource.time = Mathf.Clamp(
            previewSlider.PlaySlider.value * record.RecordSoundPath_AudioClip.length,
            0f,
            record.RecordSoundPath_AudioClip.length - 0.1f);
    }

    private void OnEnable()
    {
        if (record != null)
            CheckPlayTime();

        if (SoundManager.Instance != null)
            SoundManager.Instance.OnPreviewPlayDone += PreviewEnd;
    }

    private void OnDisable()
    {
        if (playRecordCoroutine != null)
        {
            StopCoroutine(playRecordCoroutine);
            playRecordCoroutine = null;
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.OnPreviewPlayDone -= PreviewEnd;
    }

    private void OnDestroy()
    {
        if (previewSlider != null)
        {
            previewSlider.OnMouseDown -= StartDrag;
            previewSlider.OnMouseUp -= EndDrag;
        }
    }
}
