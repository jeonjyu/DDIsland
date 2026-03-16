using UnityEngine;

public class Record : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    private void Awake()
    {
        Debug.Log(clip.length);
    }
}
