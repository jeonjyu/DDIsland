using UnityEngine;

public class StageControl : MonoBehaviour
{
    [Header("현재 씬이 시작할 때 재생할 bgm")]
    [SerializeField] private AudioClip stageBgmClip;

    [Header("현재 씬 bgm의 볼륨 수치")]
    [Range(0f, 1f)]
    [SerializeField] private float stageBgmVolume = 1f;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(stageBgmClip);
        SoundManager.Instance.SetSoundVolume(Soundtype.BGM, stageBgmVolume);
    }

    private void OnValidate()
    {
        SoundManager.Instance.SetSoundVolume(Soundtype.BGM, stageBgmVolume);
    }
}
