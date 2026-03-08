using UnityEngine;
using System.Collections.Generic;

// 언어 타입
public enum LanguageType
{
    Kr = 0,       //한국어
    En = 1,       //영어
}

[CreateAssetMenu(fileName = "StringDataSO", menuName = "Scriptable Objects/Data/StringDataSO")]
public class StringDataSO : ScriptableObject
{
    public List<StringData> StringDatas;
    public Dictionary<string, string[]> StringDic = new SerializeDictionary<string, string[]>();

    public void Init()
    {
        foreach(var data in StringDatas)
        {
            if (StringDic.ContainsKey(data.key))
                continue;

            StringDic.Add(data.key, data.values);
        }
    }
}
