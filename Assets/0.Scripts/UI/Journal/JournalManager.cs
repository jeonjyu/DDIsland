using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("대분류 탭")]
    [SerializeField] private Button questTabButton;     // 퀘스트 탭
    [SerializeField] private Button journalTabButton;   // 도감 탭
    [SerializeField] private GameObject questTabHighlight;   // 퀘스트 탭 활성화 표시
    [SerializeField] private GameObject journalTabHighlight; // 도감 탭 활성화 표시

    [Header("소분류 카테고리 버튼")]
    [SerializeField] private Button[] categoryButtons;  // 5개 카테고리 버튼
    [SerializeField] private TextMeshProUGUI[] categoryTexts; // 카테고리 버튼 텍스트
    [SerializeField] private GameObject[] categoryHighlights; // 활성화 표시

    [Header("슬롯 동적 생성")]                                  
    [SerializeField] private GameObject slotTemplate;             //  슬롯 프리팹 (비활성화 상태)
    [SerializeField] private RectTransform slotContent;           //  슬롯들의 부모
    private List<GameObject> slotObjects = new List<GameObject>(); //  생성된 슬롯 오브젝트
    private List<JournalSlot> slotUIs = new List<JournalSlot>();  //  슬롯 UI 컴포넌트
 
    [Header("스크롤")]
    [SerializeField] private ScrollRect scrollRect;     // 스크롤뷰

    [Header("필터/정렬 드롭다운")]
    [SerializeField] private TMP_Dropdown filterDropdown; // 전체/등록/미등록

    [Header("카테고리 라벨 설정")]
    [SerializeField] private string[] journalCategoryNames = { "어종", "코스튬", "인테리어", "음반", "음식" };

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


        // TODO: 음반 카테고리 비활성화, 앨범 데이터 들어오면 아래 줄 삭제
        if (categoryButtons.Length > (int)JournalCategory.Album)
            categoryButtons[(int)JournalCategory.Album].interactable = false;


       
        if (ItemManager.Instance != null) // 상점 구매 이벤트 구독
            ItemManager.Instance.OnPlayerItemAdded += OnStoreItemAdded;
        if (FishManager.Instance != null) // 낚시 이벤트 구독
            FishManager.Instance.OnFishGet += OnFishGet;
        if (CookingManager.Instance != null) // 요리 이벤트 구독
            CookingManager.Instance.OnFoodCooked += OnFoodCooked;
    }

    private void OnFishGet(int fishId)
    {
        OnItemUnlocked(JournalCategory.Fish, fishId);
    }
    private void OnFoodCooked(int foodId)
    {
        OnItemUnlocked(JournalCategory.Food, foodId);
    }

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

        filterDropdown.ClearOptions();
        filterDropdown.AddOptions(new List<string> { "전체", "등록", "미등록" });
        filterDropdown.value = 0;
        filterDropdown.onValueChanged.AddListener(OnFilterChanged);
    }


    // 도감 창 열기
    public void OpenJournal()
    {
        if (journalPanel != null)
            journalPanel.SetActive(true);
      
        // 열 때 현재 상태 갱신
        //RefreshSlots();

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

        if (detailPopup != null)
            detailPopup.Hide();
    }

    // 대분류 탭 전환
    private void SwitchMainTab(MainTab tab)
    {
        currentMainTab = tab;

        // 탭 하이라이트 전환
        if (questTabHighlight != null)
            questTabHighlight.SetActive(tab == MainTab.Quest);
        if (journalTabHighlight != null)
            journalTabHighlight.SetActive(tab == MainTab.Journal);

        // 카테고리 라벨 변경
        UpdateCategoryLabels();

        // 도감 탭이면 슬롯 갱신
        if (tab == MainTab.Journal) // 도감 모드 
        {
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
        for (int i = 0; i < categoryHighlights.Length; i++)
        {
            if (categoryHighlights[i] != null)
                categoryHighlights[i].SetActive(i == (int)category);
        }

        // 필터 초기화
        currentFilter = CollectionFilter.All;
        if (filterDropdown != null)
            filterDropdown.SetValueWithoutNotify(0);

        // 슬롯 갱신
        RefreshSlots();

        // 스크롤 위치 맨 위로
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // 필터 변경 
    private void OnFilterChanged(int index)
    {
        currentFilter = (CollectionFilter)index;
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
    }

 
    // 카테고리 라벨 업데이트
    private void UpdateCategoryLabels()
    {
        if (currentMainTab == MainTab.Journal)
        {
            for (int i = 0; i < categoryTexts.Length; i++)
            {
                if (categoryTexts[i] != null && i < journalCategoryNames.Length)
                    categoryTexts[i].text = journalCategoryNames[i];
            }

            // 음반 카테고리 비활성화
            // TODO: JournalRecordDataSO 들어오면 아래 줄 삭제
            if (categoryButtons.Length > (int)JournalCategory.Album)
                categoryButtons[(int)JournalCategory.Album].interactable = false;
        }
        else
        {
            // TODO: 퀘스트 카테고리 라벨 (상점, 낚시, 도감, 성장, 완료퀘스트)
            string[] questCategoryNames = { "상점", "낚시", "도감", "성장", "완료" };
            for (int i = 0; i < categoryTexts.Length; i++)
            {
                if (categoryTexts[i] != null && i < questCategoryNames.Length)
                    categoryTexts[i].text = questCategoryNames[i];
            }
        }
    }

    // 슬롯 클릭 시 아이템창 팝업 
    private void OnSlotClicked(JournalDataLoader.JournalItemData data)
    {
        if (detailPopup != null)
            detailPopup.Show(data);
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
                    collection._unlockedFishIds.Add(itemId);
                newUnlocked = true;
                break;
            case JournalCategory.Costume:
                if (!collection._unlockedCostumeIds.Contains(itemId))
                    collection._unlockedCostumeIds.Add(itemId);
                newUnlocked = true;
                break;
            case JournalCategory.Interior:
                if (!collection._unlockedInteriorIds.Contains(itemId))
                    collection._unlockedInteriorIds.Add(itemId);
                newUnlocked = true;
                break;
            case JournalCategory.Food:
                if (!collection._unlockedFoodIds.Contains(itemId))
                    collection._unlockedFoodIds.Add(itemId);
                newUnlocked = true;
                break;
        }

        // 현재 보고 있는 카테고리면 슬롯 갱신
        if (currentCategory == category && journalPanel.activeSelf)
        {
            RefreshSlots();
        }

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

    private void OnDestroy()
    {
        foreach (var slot in slotUIs)
        {
            if (slot != null)
                slot.OnSlotClicked -= OnSlotClicked;
        }
        // 이벤트 구독 해제
        if (ItemManager.Instance != null)
            ItemManager.Instance.OnPlayerItemAdded -= OnStoreItemAdded;
        if (FishManager.Instance != null)
            FishManager.Instance.OnFishGet -= OnFishGet;
        if (CookingManager.Instance != null)
            CookingManager.Instance.OnFoodCooked -= OnFoodCooked;
    }
}
