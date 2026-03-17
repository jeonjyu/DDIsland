using UnityEngine;

public class UI_AMBSlot : UI_RecordSlot
{
    [SerializeField] private GameObject playObj;
    [SerializeField] private GameObject pauseObj;

    private UI_AMBList ambList;

    public override void InitData<T>(RecordDataSO record, UI_RecordList<T> recordList)
    {
        base.InitData(record, recordList);

        ambList = recordList as UI_AMBList;
    }

    public override void CheckUserData()
    {
        
    }

    public override void OnClick_Slot()
    {
        if (Record == null) return;

        if (SoundManager.Instance.BgsSource.clip != Record.RecordSoundPath_AudioClip)
        {
            SoundManager.Instance.PlayBGS(Record.RecordSoundPath_AudioClip);
        }
        else if (SoundManager.Instance.BgsSource.isPlaying)
        {
            SoundManager.Instance.BgsSource.Pause();
        }
        else
        {
            SoundManager.Instance.BgsSource.Play();
        }


        playObj.SetActive(SoundManager.Instance.BgsSource.clip != Record.RecordSoundPath_AudioClip || !SoundManager.Instance.BgsSource.isPlaying);
        pauseObj.SetActive(SoundManager.Instance.BgsSource.clip == Record.RecordSoundPath_AudioClip && SoundManager.Instance.BgsSource.isPlaying);
    }
}
