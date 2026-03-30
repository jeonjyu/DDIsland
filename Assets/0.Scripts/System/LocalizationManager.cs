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
        StringDataSO.Init();
    }

    public string GetString(string key)
    {
        if(StringDataSO.StringDic.ContainsKey(key))
        {
            return StringDataSO.StringDic[key][PlayerPrefsDataManager.Language];
        }

        return null;
    }

    public void SetUserLanguage()
    {
        SystemLanguage language = Application.systemLanguage;

        switch(language)
        {
            case SystemLanguage.Korean:
                PlayerPrefsDataManager.Language = 0;
                break;
            case SystemLanguage.English:
            default:
                PlayerPrefsDataManager.Language = 1;
                break;
        }
    }
}
