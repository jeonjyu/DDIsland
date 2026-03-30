using TMPro;
using UnityEngine;

public class UI_StoreCategoryText : UI_BaseLocalizedText
{
    public string TextId
    {
        get => textId;
        set
        {
            textId = value;
            if (textId != null) SetText();
        }
    }

    protected override void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    protected override void SetText()
    {
        if (string.IsNullOrEmpty(textId)) return;
        base.SetText();

    }
}
