using UnityEngine;

public class UI_AMBList : UI_RecordList<UI_AMBSlot>
{
    /// <summary>
    /// 환경음 재생 메서드
    /// </summary>
    /// <param name="slot"> 클릭한 환경음 슬롯 </param>
    public void PlayAMB(UI_AMBSlot slot)
    {
        AudioSource source = SoundManager.Instance.BgsSource;

        if(currentSlot != null)
        {
            if(currentSlot == slot)
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

            currentSlot.SetPlayModeImg(false);
        }

        SoundManager.Instance.PlayBGS(slot.Record.RecordSoundPath_AudioClip);
        slot.SetPlayModeImg(true);
        currentSlot = slot;
    }
}
