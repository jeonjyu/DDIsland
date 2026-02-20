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

    public int SelectedOption => dropdown.value;

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChagned);
    }

    public void SetOptions()
    {
        dropdown.ClearOptions();

        foreach (var option in optionList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }
        dropdown.captionText.text = optionList[0];
    }

    public virtual void OnDropdownValueChagned(int index)
    {

    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownValueChagned);
    }
}
