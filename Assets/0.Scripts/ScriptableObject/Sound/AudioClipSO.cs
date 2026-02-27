using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipSO", menuName = "Scriptable Objects/AudioClipSO")]
public class AudioClipSO : ScriptableObject
{
    [field: SerializeField] public AudioClip[] BgmClips { get; private set; }       // 배경음 오디오 클립
    [field: SerializeField] public AudioClip[] SfxClips { get; private set; }       // 효과음 오디오 클립
    [field: SerializeField] public AudioClip[] BgsClips { get; private set; }       // 환경음 오디오 클립

    public void GetClips(AudioClip[] clips, Dictionary<string, AudioClip> dic)
    {
        // 딕셔너리에 오디오 클립 추가
        if (clips.Length > 0)
        {
            foreach (var so in clips)
            {
                if (so != null)
                    dic.Add(so.name, so);
            }
        }
    }
}
