using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

/// <summary>
/// Enum에 설정된 Description을 추출하기 위한 확장 클래스
/// </summary>
public static class DescriptionExtracter
{

    // 각 enum에 설정된 Description으로 리스트 반환
    public static List<string> GetEnumList<T>(Array optionEnum) where T : Enum
    {

        List<string> lst = new List<string>(optionEnum.Length);

        foreach (T option in optionEnum)
            lst.Add(GetEnumDesc(option));

        return lst;
    }

    // 각 enum에 지정된 description 반환
    public static string GetEnumDesc(Enum value)
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute description = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

        return description.Description;
    }
}
