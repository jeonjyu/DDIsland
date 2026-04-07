using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CurrentPlaylistSlot : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text artistText;
    [SerializeField] private TMP_Text timeText;

    private UI_BGMList ui_BgmList;
    public RecordDataSO Record;

    public bool IsDeleteCheck { get; private set; }

    public void SetRecordInit(RecordDataSO data, UI_BGMList bgmList)
    {
        Record = data;
        ui_BgmList = bgmList;

        iconImg.sprite = data.RecordImgPath_Sprite;
        titleText.text = data.RecordName_String;
        artistText.text = data.RecordArtist_String;
        timeText.text = data.RecordSoundPath_AudioClip.GetClipLength();
    }

    public void Onclick_Play()
    {
        ui_BgmList.PlayBGM(Record);
    }

    public void SetDeleteCehck(Toggle toggle)
    {
        IsDeleteCheck = toggle.isOn;
    }

    public void PlayRecordSfx(AudioClip clip)
    {
        SoundManager.Instance.PlaySFX(clip);
    }
}
