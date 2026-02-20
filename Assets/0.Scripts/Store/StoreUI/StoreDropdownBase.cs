using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;

public class StoreDropdownBase : MonoBehaviour
{
    TMP_Dropdown dropdown;

    protected List<string> optionList = new List<string>();

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void SetOptions()
    {
        dropdown.ClearOptions();

        foreach (var option in optionList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }
        dropdown.captionText.text = optionList[1];
    }
}
