using UnityEngine;

public class UI_ReplacingButtonText : UI_BaseLocalizedText
{
    [Header("변경될 스트링 키")]
    [SerializeField] protected string replaceTextId;
    bool isReplaced = false;

    // 기본키랑 대체 키가 있고 
    // 키를 대체해서 SetText로 text에 넣어주기
    // 

    public void SetTextRuntime(bool isApplied)
    {
        // 적용되었으면 대체
        if(isReplaced != isApplied)
            isReplaced = isApplied;
        SetText();
    }

    protected override void SetText()
    {
        if (isReplaced)
            text.text = $"{DataManager.Instance.StringUIDatabase.StringUIInfoData[replaceTextId].ID_String}";
        else
            text.text = $"{DataManager.Instance.StringUIDatabase.StringUIInfoData[textId].ID_String}";
    }
}
