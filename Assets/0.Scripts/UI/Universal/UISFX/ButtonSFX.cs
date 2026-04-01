using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] AudioClip clickaudio;
    [SerializeField] Button button;

    private void Awake()
    {
        if (button == null) 
            button = GetComponentInChildren<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(PlayClickSound);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(PlayClickSound);
    }

    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySFX(clickaudio);
    }
}
