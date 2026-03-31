using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HelpManager : MonoBehaviour
{
    static public HelpManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    [Header("UI 연결")]
    public HelpPage _helpPageUI;

    //public void OpenHelpByInt(int locationIndex)
    //{
    //    OpenHelp((HelpLocation)locationIndex);
    //}

    public void OpenHelp(HelpLocation targetLocation)
    {
        var helpList = new List<HelpPageData>();
        var allHelpData = DataManager.Instance.HelpDatabase.HelpInfoData.datas;

        foreach (var data in allHelpData)
        {
            if (data.helplocationType == targetLocation)
            {
                HelpPageData page = new()
                {
                    _mainTitle = LocalizationManager.Instance.GetString(data.MainTitle),
                    _subTitle = string.IsNullOrEmpty(data.SubTitle) ? "" : LocalizationManager.Instance.GetString(data.SubTitle),
                    _bodyText = ProcessDynamicText(data.Content),
                    //_imageSprite = data.HelpImgPath_Sprite
                };
                helpList.Add(page);
            }
        }

        if (helpList.Count > 0)
        {
            _helpPageUI.gameObject.SetActive(true);
            _helpPageUI.OpenHelpWindow(helpList);
        }
    }


    /// <summary>
    /// {}로 감싸인 텍스트를 해당 키의 문자열로 번역해주는 메서드
    /// </summary>
    /// <param name="contentKey"></param>
    /// <returns></returns>
    private string ProcessDynamicText(string contentKey)
    {
        string rawText = LocalizationManager.Instance.GetString(contentKey);

        if (string.IsNullOrEmpty(rawText)) return "";

        string processedText = Regex.Replace(rawText, @"\{([^}]+)\}", match =>
        {
            string innerKey = match.Groups[1].Value;

            return LocalizationManager.Instance.GetString(innerKey);
        });

        return processedText;
    }
}
