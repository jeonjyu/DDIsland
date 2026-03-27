using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;


/// <summary>
/// Enum에 설정된 Description을 추출하기 위한 확장 클래스
/// </summary>
public static class DescriptionExtracter
{

    // 각 enum에 설정된 Description으로 리스트 반환
    // === 2026-03-27 ===
    // 다국어 전환 기능으로 용도 변경
    public static List<string> GetEnumList<T>(Array optionEnum) where T : Enum
    {

        List<string> lst = new List<string>(optionEnum.Length);

        foreach (T option in optionEnum)
            lst.Add(GetEnumDesc(option));

        return lst;
    }
    public static List<string> GetFilterEnumList<T>(Array optionEnum) where T : Enum
    {

        List<string> lst = new List<string>(optionEnum.Length);

        foreach (T option in optionEnum)
        {
            string str = GetEnumDesc(option);
            string filter = LocalizationManager.Instance.GetString(str);
            lst.Add(filter);
        }

        return lst;
    }

    // 각 enum에 지정된 description 반환
    // === ===
    // enum으로 
    public static string GetEnumDesc(Enum value)
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute description = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

        return description.Description;
    }

}
