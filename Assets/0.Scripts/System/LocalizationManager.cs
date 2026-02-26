using UnityEngine;
using System;

[Serializable]
public class StringData
{
    public string key;
    public string[] values;
}

/// <summary>
/// 언어 기능을 담당하는 클래스
/// </summary>
public class LocalizationManager : Singleton<LocalizationManager>
{
    [field: SerializeField] public StringDataSO StringDataSO { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public string GetString(string key)
    {
        if(StringDataSO.StringDic.ContainsKey(key))
        {
            return StringDataSO.StringDic[key][0];
        }

        return null;
    }
}
