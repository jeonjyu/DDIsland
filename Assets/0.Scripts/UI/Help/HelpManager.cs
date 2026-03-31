using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TempHelpData
{
    public HelpLocation _location; // 상점, 꾸미기 등
    public string _mainTitle;
    public string _subTitle;
    [TextArea(3, 5)] // 본문 쓸 때 유니티에서 엔터 칠 수 있게 창을 늘려주는 옵션
    public string _bodyText;

    public Sprite _imageSprite;
}

public class HelpManager : MonoBehaviour
{
    static public HelpManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OpenHelp(HelpLocation.Shop);
    }

    [Header("UI 연결")]
    public HelpPage _helpPageUI;

    [Header("임시 테스트 데이터 넣는 곳")]
    public List<TempHelpData> _allData = new List<TempHelpData>();

    private List<HelpPageData> _allHelpDataList = new();

    public void OpenHelp(HelpLocation targetLocation)
    {
        List<HelpPageData> helpList = new();

        foreach (var data in _allData)
        {
            if (data._location == targetLocation)
            {
                HelpPageData page = new()
                {
                    _mainTitle = data._mainTitle,
                    _subTitle = data._subTitle,
                    _bodyText = data._bodyText
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
}
