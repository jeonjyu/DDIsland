using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;

public class StoreDropdownBase : MonoBehaviour
{
    TMP_Dropdown dropdown;

    List<string> optionList = new List<string>();

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
    }


    public void GetOptionList<T>(Array optionEnum) where T : Enum
    {
        //Enum[] optionArr = (Enum[])Enum.GetValues(typeof(T));
        List<string> lst = new List<string>(optionEnum.Length);

        foreach(T option in optionEnum)
            lst.Add(GetEnumDesc(option));

        optionList = new List<string>(lst);
    }

    // 각 enum에 지정된 description 반환
    public string GetEnumDesc<T>(T value) where T : Enum 
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute description = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

        return description.Description;
    }
}
