using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class StoreDropdownBase : MonoBehaviour
{
    [SerializeField] protected TMP_Dropdown dropdown;
    [SerializeField] protected StoreListViewModel storeListViewModel;

    protected List<string> optionList = new List<string>();

    public int SelectedOption => dropdown.value;

    public void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChagned);
    }

    public virtual void SetOptions()
    {
        //Debug.Log("[StoreDropdownBase] SetOptions");

        if (dropdown != null)
        {
            dropdown.ClearOptions();
            //Debug.Log("[StoreDropdownBase] SetOptions | dropdown이 존재하지 않음");
        }

        foreach (var option in optionList)
        {
            //Debug.Log(option.ToString());
            dropdown.options.Add(new TMP_Dropdown.OptionData(option.ToString()));
        }

        //if(optionList.Count > 0) dropdown.captionText.text = optionList[0];
    }

    // 드롭다운 값이 변경될 경우 실행되어야 할 메서드
    public virtual void OnDropdownValueChagned(int index)
    {
        dropdown.value = index;

    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownValueChagned);
    }
}
