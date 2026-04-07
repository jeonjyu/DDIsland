using UnityEngine;

public class UI_AmbSourceText : UI_BaseLocalizedText
{
    [SerializeField] private string[] keys = new string[2];

    private UI_AMBSlot ambSlot;

    public void SetSlot(UI_AMBSlot slot) => ambSlot = slot;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void SetText()
    {
        if (ambSlot == null || ambSlot.Record == null) return;

        switch (ambSlot.Record.ambsourceType)
        {
            case AmbSource.Weather:
                text.text = DataManager.Instance.StringUIDatabase.StringUIInfoData[textId].ID_String;
                break;
            case AmbSource.Nature:
                text.text = DataManager.Instance.StringUIDatabase.StringUIInfoData[keys[0]].ID_String;
                break;
            case AmbSource.Life:
                text.text = DataManager.Instance.StringUIDatabase.StringUIInfoData[keys[1]].ID_String;
                break;
        }
    }
}
