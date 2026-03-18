using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UI_DropdownText : UI_BaseLocalizedText
{
    private TMP_Dropdown dropdown;

    [SerializeField] private string[] keys;

    protected override void Awake()
    {
        base.Awake();
        dropdown = GetComponent<TMP_Dropdown>();
    }

    protected override void SetText()
    {
        if (keys.Length == 0) return;

        dropdown.options.Clear();

        List<string> options = new List<string>();

        foreach(string option in keys)
        {
            options.Add(DataManager.Instance.StringUIDatabase.StringUIInfoData[option].ID_String);
        }

        dropdown.AddOptions(options);
    }
}
