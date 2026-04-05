using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// UI만 담당: 창 열기/닫기, 탭 전환, 카테고리 전환, 필터 적용
/// 데이터는 JournalDataLoader에서 가져옴
/// </summary>
public class JournalManager : MonoBehaviour
{
    [Header("매니저 참조")]
    [SerializeField] private JournalDataLoader dataLoader;
    [SerializeField] private JournalPopup detailPopup;

    [Header("도감 창")]
    [SerializeField] private GameObject journalPanel;   // 도감 전체 패널
    [SerializeField] private Button closeButton;        // 닫기 버튼
    [SerializeField] private CanvasGroup btnTrayGroup; // 메인 버튼UI들 비활성화용
    [Header("대분류 탭")]
    [SerializeField] private Button questTabButton;     // 퀘스트 탭
    [SerializeField] private Button journalTabButton;   // 도감 탭
    [SerializeField] private Image questTabBackground;    // 퀘스트 탭 활성화 표시
    [SerializeField] private Image journalTabBackground;  // 도감 탭 활성화 표시
    [SerializeField] private TextMeshProUGUI[] questTabTexts;     // 메인탭 텍스트 
    [SerializeField] private TextMeshProUGUI[] journalTabTexts;  // todo: 나중에 버튼 1개로 통합되면 배열 삭제 
    [Header("도감 영역")]
    [SerializeField] private GameObject journalArea;
    
    [Header("소분류 카테고리 버튼")]
    [SerializeField] private Button[] categoryButtons;  // 5개 카테고리 버튼
    [SerializeField] private TextMeshProUGUI[] categoryTexts; // 카테고리 버튼 텍스트

    [Header("슬롯 동적 생성")]                                  
    [SerializeField] private GameObject slotTemplate;             //  슬롯 프리팹 (비활성화 상태)
    [SerializeField] private RectTransform slotContent;           //  슬롯들의 부모
    private List<GameObject> slotObjects = new List<GameObject>(); //  생성된 슬롯 오브젝트
    private List<JournalSlot> slotUIs = new List<JournalSlot>();  //  슬롯 UI 컴포넌트
 
    [Header("스크롤")]
    [SerializeField] private ScrollRect scrollRect;     // 스크롤뷰

    [Header("필터/정렬 드롭다운")]
    [SerializeField] private JournalFilterDropdown filterDropdown; // 전체/등록/미등록

    //[Header("카테고리 라벨 설정")]
    //[SerializeField] private string[] journalCategoryNames = { "어종", "코스튬", "인테리어", "음반", "음식" };

    [Header("카테고리 버튼 색상")]
    [SerializeField] private Image[] categoryBackgrounds;  // 각 버튼의 Background Image
    [SerializeField] private Image[] categoryOutlines;     // 각 버튼의 Outline Image
    [Header("카테고리 책갈피")]
    [SerializeField] private Transform boxBackground;
    [Header("음반 해금 연동")]
    [SerializeField] private UI_Record uiRecord;

    // 탭 색상
    private readonly Color selectedTabBg = new Color(0.99f, 0.97f, 0.91f, 1f);   // 크림색
    private readonly Color unselectedTabBg = new Color(0.78f, 0.62f, 0.41f, 1f); // 갈색

    // 선택/미선택 색상 
    private readonly Color selectedBg = new Color(0.99f, 0.97f, 0.91f, 1f);      // 크림색
    private readonly Color unselectedBg = new Color(0.83f, 0.71f, 0.60f, 1f); // 베이지색
    private readonly Color selectedOutline = new Color(0.88f, 0.80f, 0.66f, 1f);  // 밝은 테두리
    private readonly Color unselectedOutline = new Color(0.56f, 0.44f, 0.29f, 1f);// 어두운 테두리

    // 현재 상태
    private MainTab currentMainTab = MainTab.Journal;
    private JournalCategory currentCategory = JournalCategory.Fish;
    private CollectionFilter currentFilter = CollectionFilter.All;
    private List<JournalDataLoader.JournalItemData> currentItems;

    private void Awake()
    {
        SetupButtons();
        SetupDropdown();
        //  SetupSlotEvents();
        // 템플릿 비활성화 (복제용으로만 사용)
        if (slotTemplate != null)
            slotTemplate.SetActive(false);
    }

    private void Start()
    {   
        // UI 라벨 세팅
        UpdateCategoryLabels();
       
        if (ItemManager.Instance != null) // 상점 구매 이벤트 구독
            ItemManager.Instance.OnPlayerItemAdded += OnStoreItemAdded;
        if (FishManager.Instance != null) // 낚시 이벤트 구독
            FishManager.Instance.OnFishGet += OnFishGet;
        if (CookingManager.Instance != null) // 요리 이벤트 구독
            CookingManager.Instance.OnFoodCooked += OnFoodCooked;
        if (uiRecord == null)
            uiRecord = FindObjectOfType<UI_Record>();
        if (uiRecord != null && uiRecord.recordUnlock != null) // 음반 이벤트 구독 
            uiRecord.recordUnlock.OnRecordUnlock += OnRecordUnlocked;
    }

    private void Update()
    {
        if (Keyboard.current != null
            && Keyboard.current.escapeKey.wasPressedThisFrame
            && journalPanel != null
            && journalPanel.activeSelf)
        {
            if (detailPopup != null && detailPopup.gameObject.activeSelf)
            {
                detailPopup.Hide();
                return;
            }

            CloseJournal();
        }
    }


    // 외부 연동, 아이템 해금 시 호출 (상점 구매, 낚시 획득, 요리 완성 등)
    public void OnItemUnlocked(JournalCategory category, int itemId)
    {
        // Collection_Data에 ID 추가 (중복 방지)
        var collection = DataManager.Instance.Box.Collection;
        bool newUnlocked = false;

        switch (category)
        {
            case JournalCategory.Fish:
                if (!collection._unlockedFishIds.Contains(itemId))
                {
                    collection._unlockedFishIds.Add(itemId);
                    newUnlocked = true;
                  
                    QuestManager.Instance.SetSimpleProgress(QuestConditionKey.FishGuideRegisteredCount, collection._unlockedFishIds.Count);
                }
                break;
            case JournalCategory.Costume:
                if (!collection._unlockedCostumeIds.Contains(itemId))
                {
                    collection._unlockedCostumeIds.Add(itemId);
                    newUnlocked = true;
              
                    QuestManager.Instance.SetSimpleProgress(QuestConditionKey.CostumeGuideRegisteredCount, collection._unlockedCostumeIds.Count);
                }
                break;
            case JournalCategory.Interior:
                if (!collection._unlockedInteriorIds.Contains(itemId))
                {
                    collection._unlockedInteriorIds.Add(itemId);
                    newUnlocked = true;
       
                    QuestManager.Instance.SetSimpleProgress(QuestConditionKey.InteriorGuideRegisteredCount, collection._unlockedInteriorIds.Count);
                }
                break;
            case JournalCategory.Album:
                if (!collection._unlockedAlbumIds.Contains(itemId))
                {
                    collection._unlockedAlbumIds.Add(itemId);
                    newUnlocked = true;
                }
                break;
            case JournalCategory.Food:
                if (!collection._unlockedFoodIds.Contains(itemId))
                {
                    collection._unlockedFoodIds.Add(itemId);
                    newUnlocked = true;
                  
                    QuestManager.Instance.SetSimpleProgress(QuestConditionKey.FoodGuideRegisteredCount, collection._unlockedFoodIds.Count);
                }
                break;
        }

        //// 현재 보고 있는 카테고리면 슬롯 갱신
        //if (currentCategory == category && journalPanel.activeSelf)
        //{
        //    RefreshSlots();
        //}

        if (newUnlocked)
        {
            DataManager.Instance.Hub.SaveAllData();

            // UI가 켜져있다면 즉시 새로고침
            if (currentCategory == category && journalPanel.activeSelf)
            {
                RefreshSlots();
            }
        }

    }

    private void OnRecordUnlocked(RecordDataSO record)
    {
        OnItemUnlocked(JournalCategory.Album, record.RecordID);
    } // 음반
    private void OnFishGet(int fishId, float length)
    {
        UpdateFishRecord(fishId, length);
        OnItemUnlocked(JournalCategory.Fish, fishId);
    } // 낚시 
    private void UpdateFishRecord(int fishId, float length) 
    {
        var records = DataManager.Instance.Box.Collection._fishRecords;
        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].FishId == fishId)
            {
                if (length > records[i].MaxLength)
                    records[i].MaxLength = length;

                DataManager.Instance.Hub.SaveAllData();
                return;
            }
        }
        records.Add(new FishRecordData { FishId = fishId, MaxLength = length });
    } 
    private void OnFoodCooked(int foodId)
    {
        OnItemUnlocked(JournalCategory.Food, foodId);
    }  // 요리 

    // 상점 구매 시 도감 해금 처리
    private void OnStoreItemAdded(IStoreItem item, StoreCat storeCat)
    {
        switch (storeCat)
        {
            case StoreCat.interior:
                OnItemUnlocked(JournalCategory.Interior, item.ObjectId);
                break;
            case StoreCat.costume:
                OnItemUnlocked(JournalCategory.Costume, item.ObjectId);
                break;
        }
    }
  
    
    // 버튼들 초기화
    private void SetupButtons()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseJournal);

        if (questTabButton != null)
            questTabButton.onClick.AddListener(() => SwitchMainTab(MainTab.Quest));

        if (journalTabButton != null)
            journalTabButton.onClick.AddListener(() => SwitchMainTab(MainTab.Journal));

        // 카테고리 버튼 연결
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            int index = i;
            if (categoryButtons[i] != null)
                categoryButtons[i].onClick.AddListener(() => SwitchCategory((JournalCategory)index));
        }
    }

    private void SetupDropdown()
    {
        if (filterDropdown == null) return;
        filterDropdown.OnFilterChanged += OnFilterChanged;
    }


    // 도감 창 열기
    public void OpenJournal()
    {
        if (journalPanel != null)
            journalPanel.SetActive(true);

        // 열 때 현재 상태 갱신
        //RefreshSlots();
        if (btnTrayGroup != null) btnTrayGroup.interactable = false;

        // 열 때 데이터 로드
        if (DataManager.Instance != null && DataManager.Instance.JournalDatabase != null)
        {
            SwitchMainTab(MainTab.Journal);
        }
    }
    // 도감 창 닫기 
    public void CloseJournal()
    {
        if (journalPanel != null)
            journalPanel.SetActive(false);
        if (btnTrayGroup != null) btnTrayGroup.interactable = true;
        if (detailPopup != null)
            detailPopup.Hide();
    }

    // 대분류 탭 전환
    private void SwitchMainTab(MainTab tab)
    {
        currentMainTab = tab;

        // 탭 하이라이트 전환
        if (questTabBackground != null)
            questTabBackground.color = (tab == MainTab.Quest) ? selectedTabBg : unselectedTabBg;
        if (journalTabBackground != null)
            journalTabBackground.color = (tab == MainTab.Journal) ? selectedTabBg : unselectedTabBg;
        // 탭 텍스트 로컬라이징
        //if (questTabText != null)
        //    questTabText.text = JournalLocalize.Tab(MainTab.Quest);
        //if (journalTabText != null)
        //    journalTabText.text = JournalLocalize.Tab(MainTab.Journal);
        //foreach (var t in questTabTexts)
        //    if (t != null) t.text = JournalLocalize.Tab(MainTab.Quest);
        //foreach (var t in journalTabTexts)
        //    if (t != null) t.text = JournalLocalize.Tab(MainTab.Journal);

        // 도감 영역 통째로 끄기
        if (journalArea != null)
            journalArea.SetActive(tab == MainTab.Journal);
        // 도감 카테고리 버튼들도 같이 숨기기
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            if (categoryButtons[i] != null)
                categoryButtons[i].gameObject.SetActive(tab == MainTab.Journal);
        }
        // 도감 탭이면 슬롯 갱신
        if (tab == MainTab.Journal) // 도감 모드 
        {
            UpdateCategoryLabels();  // 카테고리 라벨 변경
            SwitchCategory(currentCategory);
        }
        else
        {
            ClearSlots();
            // TODO: 퀘스트 탭 로직은 퀘스트 시스템에서 처리
            // 퀘스트 카테고리: 상점, 낚시, 도감, 성장, 완료 퀘스트
        }
    }

    // 소분류 카테고리 전환 
    private void SwitchCategory(JournalCategory category)
    {
        currentCategory = category;

        // 카테고리 하이라이트 전환
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            bool isSelected = (i == (int)category);

            // 배경 색 전환
            if (i < categoryBackgrounds.Length && categoryBackgrounds[i] != null)
                categoryBackgrounds[i].color = isSelected ? selectedBg : unselectedBg;

            // 테두리 색 전환
            if (i < categoryOutlines.Length && categoryOutlines[i] != null)
                categoryOutlines[i].color = isSelected ? selectedOutline : unselectedOutline;

            // 선택된 버튼을 배경 앞으로 렌더링 순서 변경
            if (boxBackground != null)
            {
                int bgIndex = boxBackground.GetSiblingIndex();
                if (isSelected)
                    categoryButtons[i].transform.SetSiblingIndex(bgIndex + 1); // 배경 뒤 = 앞에 보임
                else
                    categoryButtons[i].transform.SetSiblingIndex(bgIndex - 1); // 배경 앞 = 뒤에 가림
            }
        }

        // 필터 초기화
        currentFilter = CollectionFilter.All;
        if (filterDropdown != null)
            filterDropdown.ResetFilter();

        // 슬롯 갱신
        RefreshSlots();

    }

    // 필터 변경 
    private void OnFilterChanged(CollectionFilter filter)
    {
        currentFilter = filter;
        RefreshSlots();
    }

    // 기존 슬롯 전부 삭제 
    private void ClearSlots() // 모든 슬롯 비우기 
    {  
        for (int i = 0; i < slotObjects.Count; i++) 
        {
            Destroy(slotObjects[i]); 
        }
        slotObjects.Clear();
        slotUIs.Clear();
    }

    // 데이터 기반 슬롯 동적 생성 
    private void CreateSlots(List<JournalDataLoader.JournalItemData> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            // 템플릿 복제
            GameObject slotObj = Instantiate(slotTemplate, slotContent);
            slotObj.SetActive(true);
            slotObj.name = "JournalSlot" + i;

            // JournalSlot 컴포넌트에서 데이터 세팅
            JournalSlot slotUI = slotObj.GetComponent<JournalSlot>();
            if (slotUI != null)
            {
                slotUI.Setup(items[i]);
                slotUI.OnSlotClicked += OnSlotClicked;
            }

            slotObjects.Add(slotObj);
            slotUIs.Add(slotUI);
        }
    }

    // 데이터로더에서 데이터 가져와서 슬롯에 세팅
    private void RefreshSlots()
    {
        if (dataLoader == null) return;

        // 데이터 가져오기 (정렬 포함)
        currentItems = dataLoader.GetJournalItems(currentCategory);

        // 필터 적용
        var filteredItems = dataLoader.ApplyFilter(currentItems, currentFilter);

        // 슬롯에 데이터 세팅
        ClearSlots();
        CreateSlots(filteredItems);

        // 스크롤 위치 맨 위로
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }


    // 카테고리 라벨 업데이트
    private void UpdateCategoryLabels()
    {
        //if (currentMainTab == MainTab.Journal)
        //{
        //    for (int i = 0; i < categoryTexts.Length; i++)
        //    { 
        //        if (categoryTexts[i] != null)
        //            categoryTexts[i].text = JournalLocalize.Category((JournalCategory)i); 
        //    }
        //}
        //else
        //{
        //    // TODO: 퀘스트 카테고리 라벨 (상점, 낚시, 도감, 성장, 완료퀘스트)
        //    string[] questCategoryNames = { "상점", "낚시", "도감", "성장", "완료" };
        //    for (int i = 0; i < categoryTexts.Length; i++)
        //    {
        //        if (categoryTexts[i] != null && i < questCategoryNames.Length)
        //            categoryTexts[i].text = questCategoryNames[i];
        //    }
        //}
        for (int i = 0; i < categoryTexts.Length; i++)
        {
            if (categoryTexts[i] != null)
                categoryTexts[i].text = JournalLocalize.Category((JournalCategory)i);
        }
    }

    // 슬롯 클릭 시 아이템창 팝업 
    private void OnSlotClicked(JournalDataLoader.JournalItemData data)
    {
        if (detailPopup != null)
            detailPopup.Show(data);
    }


    
    private void OnDestroy()
    {
        foreach (var slot in slotUIs)
        {
            if (slot != null)
                slot.OnSlotClicked -= OnSlotClicked;
        }
        // 이벤트 구독 해제
        if (filterDropdown != null)
            filterDropdown.OnFilterChanged -= OnFilterChanged;
        if (ItemManager.Instance != null)
            ItemManager.Instance.OnPlayerItemAdded -= OnStoreItemAdded;
        if (FishManager.Instance != null)
            FishManager.Instance.OnFishGet -= OnFishGet;
        if (uiRecord != null && uiRecord.recordUnlock != null)
            uiRecord.recordUnlock.OnRecordUnlock -= OnRecordUnlocked;
        if (CookingManager.Instance != null)
            CookingManager.Instance.OnFoodCooked -= OnFoodCooked;
    }
}
