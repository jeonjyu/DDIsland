using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 스트링 테이블 참조하기 전 임시 열거형
public enum HelpLocation
{
    Start =1,
    Upgrade,
    Shop,
    Decoration,
    Box,
    Record,
    Journal,
    Quest,
}

public class HelpPageData
{
    public HelpLocation _location;
    public string _mainTitle;
    public string _subTitle;
    public string _bodyText;
    public Sprite _imageSprite;
}

public class HelpPage : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI _mainTitleText;
    public TextMeshProUGUI _subTitleText;
    public TextMeshProUGUI _bodyText;
    public TextMeshProUGUI _pageNumberText;
    public Image _displayImage;

    [Header("UI 버튼 연결")]
    public Button _prevButton;
    public Button _nextButton;

    private List<HelpPageData> _currentHelpPages = new();
    private int _currentPageIndex = 0;

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        _prevButton.onClick.AddListener(ShowPrevPage);
        _nextButton.onClick.AddListener(ShowNextPage);
    }

    public void OpenHelpWindow(List<HelpPageData> pages)
    {
        _currentHelpPages = pages;
        _currentPageIndex = 0;

        UpdatePageView();
    }

    private void UpdatePageView()
    {
        if (_currentHelpPages == null || _currentHelpPages.Count == 0) return;

        HelpPageData pageData = _currentHelpPages[_currentPageIndex];

        _mainTitleText.text = pageData._mainTitle;

        if (string.IsNullOrEmpty(pageData._subTitle))
        {
            _subTitleText.gameObject.SetActive(false);
        }
        else
        {
            _subTitleText.gameObject.SetActive(true);
            _subTitleText.text = pageData._subTitle;
        }

        if (pageData._imageSprite != null)
        {
            _displayImage.gameObject.SetActive(true);
            _displayImage.sprite = pageData._imageSprite;
        }
        else
        {
            _displayImage.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(pageData._bodyText))
        {
            string formattedText = pageData._bodyText.Replace("\\n", "\n");

            formattedText = formattedText.Replace("\n", "\n• ");

            _bodyText.text = "• " + formattedText;
        }
        else
        {
            _bodyText.text = "";
        }

        UpdatePageNumber();
    }

    private void UpdatePageNumber()
    {
        if (_currentHelpPages.Count == 0)
        {
            _pageNumberText.text = "0 / 0";
            return;
        }

        _pageNumberText.text = $"{_currentPageIndex + 1} / {_currentHelpPages.Count}";
    }

    private void ShowPrevPage()
    {
        if (_currentPageIndex > 0)
        {
            _currentPageIndex--;
        }
        else 
        {
            _currentPageIndex = _currentHelpPages.Count - 1; 
        }
            UpdatePageView();
    }

    private void ShowNextPage()
    {
        if (_currentPageIndex < _currentHelpPages.Count - 1)
        {
            _currentPageIndex++;
        }
        else
        {
            _currentPageIndex = 0;
        }
            UpdatePageView();
    }
}
