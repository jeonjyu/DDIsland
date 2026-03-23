using UnityEngine;

public class UI_AMBList : UI_RecordList<UI_AMBSlot>
{
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
}
