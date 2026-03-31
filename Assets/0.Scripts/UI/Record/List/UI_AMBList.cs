using TMPro;
using UnityEngine;

public class UI_AMBList : UI_RecordList<UI_AMBSlot>
{
    [Header("필터 드랍다운")]
    [SerializeField] private TMP_Dropdown filterDropdown;

    /// <summary>
    /// 환경음 재생 메서드
    /// </summary>
    /// <param name="slot"> 클릭한 환경음 슬롯 </param>
    public void PlayAMB(UI_AMBSlot slot)
    {
        AudioSource source = SoundManager.Instance.AmbSource;

        if(CurrentSlot != null)
        {
            if(CurrentSlot == slot)
            {
                if(source.isPlaying)
                {
                    source.Pause();
                    slot.SetPlayModeImg(false);
                }
                else
                {
                    source.Play();
                    slot.SetPlayModeImg(true);
                }
                return;
            }

            CurrentSlot.SetPlayModeImg(false);
        }

        SoundManager.Instance.PlayAMB(slot.Record.RecordSoundPath_AudioClip);
        slot.SetPlayModeImg(true);
        CurrentSlot = slot;
    }

    public void OnValueChanged_FilterDropdown()
    {
        foreach (var slot in recordSlotList)
        {
            switch (filterDropdown.value)
            {
                case 0:     // 전체
                    slot.gameObject.SetActive(true);
                    break;
                case 1:     // 날씨
                    slot.gameObject.SetActive(slot.Record.ambsourceType == AmbSource.Weather);
                    break;
                case 2:     // 자연
                    slot.gameObject.SetActive(slot.Record.ambsourceType == AmbSource.Nature);
                    break;
                case 3:     // 생활 소음
                    slot.gameObject.SetActive(slot.Record.ambsourceType == AmbSource.Life);
                    break;
            }
        }
    }
}
