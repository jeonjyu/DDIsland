using TMPro;
using UnityEngine;

public class UI_BaseLocalizedText : MonoBehaviour
{
    [Header("해당 키가 가질 기본 스트링 키")]
    [SerializeField] protected string textId;

    protected TMP_Text text;

    protected void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    protected void Start()
    {
        SetText();
    }

    protected virtual void SetText()
    {
        text.text = $"{DataManager.Instance.StringUIDatabase.StringUIInfoData[textId].ID_String}";
    }

    protected void OnEnable()
    {
        PlayerPrefsDataManager.OnLanguageChanged += SetText;

        if(DataManager.Instance != null && DataManager.Instance.StringUIDatabase != null)
            SetText();
    }

    protected void OnDisable()
    {
        PlayerPrefsDataManager.OnLanguageChanged -= SetText;
    }
}
