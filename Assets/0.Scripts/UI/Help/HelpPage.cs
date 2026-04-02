using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpPageData
{
    private readonly HelpDataSO _source;

    public HelpLocation _location;
    public string MainTitle { get; private set; }
    public string SubTitle { get; private set; }
    public string BodyText { get; private set; }
    public Sprite ImageSprite { get; private set; }

    public HelpPageData(HelpDataSO source)
    {
        _source = source;
        RefreshTranslation();
    }

    public void RefreshTranslation()
    {
        MainTitle = LocalizationManager.Instance.GetString(_source.MainTitle);
        SubTitle = string.IsNullOrEmpty(_source.SubTitle) ? "" : LocalizationManager.Instance.GetString(_source.SubTitle);
        BodyText = HelpManager.Instance.ProcessDynamicText(_source.Content);
        // ImageSprite = _source.HelpImgPath_Sprite; 
    }
}

public class HelpPage : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI _mainTitleText;
    public TextMeshProUGUI _subTitleText;
    public TextMeshProUGUI _bodyText;
    public TextMeshProUGUI _pageNumberText;
    //public Image _displayImage;

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

    private void OnEnable()
    {
        PlayerPrefsDataManager.OnLanguageChanged += RefreshCurrentPages;
    }

    private void OnDisable()
    {
        PlayerPrefsDataManager.OnLanguageChanged -= RefreshCurrentPages;
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

        _mainTitleText.text = pageData.MainTitle;

        if (string.IsNullOrEmpty(pageData.SubTitle))
        {
            _subTitleText.gameObject.SetActive(false);
        }
        else
        {
            _subTitleText.gameObject.SetActive(true);
            _subTitleText.text = pageData.SubTitle;
        }

        if (pageData.ImageSprite != null)
        {
            //_displayImage.gameObject.SetActive(true);
            //_displayImage.sprite = pageData._imageSprite;
        }
        else
        {
            //_displayImage.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(pageData.BodyText))
        {
            string formattedText = pageData.BodyText.Replace("\\n", "\n");

            formattedText = formattedText.Replace("\n", "\n\n• ");

            _bodyText.text = "• " + formattedText;
        }
        else
        {
            _bodyText.text = "";
        }

        UpdatePageNumber();
    }

    private void RefreshCurrentPages()
    {
        if (_currentHelpPages == null || _currentHelpPages.Count == 0) return;

        foreach (var page in _currentHelpPages)
        {
            page.RefreshTranslation();
        }

        UpdatePageView();
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
