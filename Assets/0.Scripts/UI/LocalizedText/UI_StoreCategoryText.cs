using TMPro;
using UnityEngine;

public class UI_StoreCategoryText : UI_BaseLocalizedText
{
    public string TextId
    {
        get => textId; set => textId = value;
    }

    protected override void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }
}
