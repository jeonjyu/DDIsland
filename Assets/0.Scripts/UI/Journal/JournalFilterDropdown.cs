using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class JournalFilterDropdown : MonoBehaviour
{
    [Header("드롭다운 토글")]
    [SerializeField] private Button arrowButton;        // 드롭다운 열기 
    [SerializeField] private GameObject dropdownPanel;  // 드롭다운 패널
    [SerializeField] private TextMeshProUGUI labelText; // 현재 선택된 필터 

    [Header("필터 버튼")]
    [SerializeField] private Button btnAll;      // 전체
    [SerializeField] private Button btnOwned;    // 해금 (등록)
    [SerializeField] private Button btnNotOwned; // 미해금 (미등록)
    
    //[Header("버튼 텍스트")]
    //[SerializeField] private TextMeshProUGUI textAll;
    //[SerializeField] private TextMeshProUGUI textOwned;
    //[SerializeField] private TextMeshProUGUI textNotOwned;                                       

    // StringUI 테이블 키
    private readonly string[] filterStringKeys =
    {
        "Journal_Dropdown_All_Text",       // 전체/All
        "Journal_Dropdown_Unlocked_Text",  // 등록/Unlocked
        "Journal_Dropdown_Locked_Text"     // 미등록/Locked
    };

    [Header("버튼 배경 이미지 (색상 전환용)")]
    [SerializeField] private Image bgAll;      // 전체 버튼 배경
    [SerializeField] private Image bgOwned;    // 해금 버튼 배경
    [SerializeField] private Image bgNotOwned; // 미해금 버튼 배경

    // 선택/미선택 색상
    private readonly Color selectedColor = Color.white;               
    private readonly Color unselectedColor = new Color32(0xC4, 0xBC, 0x9D, 0xFF);  // 미선택: 갈색

    // 현재 선택된 필터
    private CollectionFilter currentFilter = CollectionFilter.All;

    // 외부에서 구독할 이벤트 
    public event Action<CollectionFilter> OnFilterChanged;

    private void Awake()
    {
        // 드롭다운 열기 
        if (arrowButton != null)
            arrowButton.onClick.AddListener(ToggleDropdown);

        // 필터 버튼 클릭 이벤트
        if (btnAll != null)
            btnAll.onClick.AddListener(() => SelectFilter(CollectionFilter.All));
        if (btnOwned != null)
            btnOwned.onClick.AddListener(() => SelectFilter(CollectionFilter.Owned));
        if (btnNotOwned != null)
            btnNotOwned.onClick.AddListener(() => SelectFilter(CollectionFilter.NotOwned));

        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);

        // 드롭다운 버튼 텍스트 
        //if (textAll != null) textAll.text = JournalLocalize.Filter(CollectionFilter.All);         
        //if (textOwned != null) textOwned.text = JournalLocalize.Filter(CollectionFilter.Owned);    
        //if (textNotOwned != null) textNotOwned.text = JournalLocalize.Filter(CollectionFilter.NotOwned); 

        ApplySelection(CollectionFilter.All);
    }


    // 드롭다운 패널 토글
    private void ToggleDropdown()
    {
        if (dropdownPanel == null) return;
        bool open = !dropdownPanel.activeSelf;
        dropdownPanel.SetActive(open);

        // 드롭다운 열릴 때 한/영 텍스트 갱신
        //if (open)
        //{
        //    if (textAll != null) textAll.text = JournalLocalize.Filter(CollectionFilter.All);
        //    if (textOwned != null) textOwned.text = JournalLocalize.Filter(CollectionFilter.Owned);
        //    if (textNotOwned != null) textNotOwned.text = JournalLocalize.Filter(CollectionFilter.NotOwned);
        //}
    }

    // 드롭다운 패널 닫기
    private void HideDropdown()
    {
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
    }

    // 필터 선택 처리
    private void SelectFilter(CollectionFilter filter)
    {
        currentFilter = filter;
        ApplySelection(filter);
        HideDropdown();

        OnFilterChanged?.Invoke(filter);
    }

    private void ApplySelection(CollectionFilter filter)
    {
        // 버튼 배경 색상 전환
        if (bgAll != null)
            bgAll.color = (filter == CollectionFilter.All) ? selectedColor : unselectedColor;
        if (bgOwned != null)
            bgOwned.color = (filter == CollectionFilter.Owned) ? selectedColor : unselectedColor;
        if (bgNotOwned != null)
            bgNotOwned.color = (filter == CollectionFilter.NotOwned) ? selectedColor : unselectedColor;

        if (labelText != null)
            labelText.text = GetLocalizedFilterName((int)filter);
    }

    // StringUI 테이블에서 로컬라이징 텍스트 가져오기
    private string GetLocalizedFilterName(int index)
    {
        try
        {
            return DataManager.Instance.StringUIDatabase.StringUIInfoData[filterStringKeys[index]].ID_String;
        }
        catch
        {
            // 테이블 로드 전이거나 키가 없을 때 폴백
            string[] fallback = { "전체", "등록", "미등록" };
            return fallback[index];
        }
    }
    // 외부에서 필터 초기화할 때 호출 (카테고리 전환 시)
    public void ResetFilter()
    {
        currentFilter = CollectionFilter.All;
        ApplySelection(CollectionFilter.All);
        HideDropdown();
        
        // 리셋 시 버튼 텍스트 갱신
        //if (textAll != null) textAll.text = JournalLocalize.Filter(CollectionFilter.All);
        //if (textOwned != null) textOwned.text = JournalLocalize.Filter(CollectionFilter.Owned);
        //if (textNotOwned != null) textNotOwned.text = JournalLocalize.Filter(CollectionFilter.NotOwned);
    }

    public CollectionFilter GetCurrentFilter()
    {
        return currentFilter;
    }

    private void OnDestroy()
    {
        if (arrowButton != null)
            arrowButton.onClick.RemoveAllListeners();
        if (btnAll != null)
            btnAll.onClick.RemoveAllListeners();
        if (btnOwned != null)
            btnOwned.onClick.RemoveAllListeners();
        if (btnNotOwned != null)
            btnNotOwned.onClick.RemoveAllListeners();
    }
}
