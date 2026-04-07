using System.Collections;
using UnityEngine;

public class UI_Record : MonoBehaviour
{
    [SerializeField] private UI_BGMList bgmList;
    [SerializeField] private UI_AMBList ambList;

    [field: SerializeField] public UI_RecordUnlock recordUnlock;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        bgmList.PlayBGM(DataManager.Instance.RecordDatabase.RecordInfoData[DataManager.Instance.RecordDatabase.CurrentPlayList[0]]);
    }

    public void PlayRecordSfx(AudioClip clip)
    {
        SoundManager.Instance.PlaySFX(clip);
    }
}
