using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManagerV2 : MonoBehaviour
{
    #region UI 변수 
    [Header("플레이어")]
    public PlayerController playerController;
    [Header("상점 UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI titleText;

    [Header("스탯 정보 표시 (우측 패널)")]
    public Image statIconImage;               // 스탯 아이콘
    public TextMeshProUGUI statNameText;      // 스탯 이름
    public Image levelFillImage;              // 피자 
    public TextMeshProUGUI levelProgressText; // 1/10                                               //    public TextMeshProUGUI statChangeText;    // 변동스탯 

    [Header("변동 스탯 표시 (좌측 패널)")]
    public TextMeshProUGUI statCurrentValueText;  // 첫째줄 현재 MAX
    public TextMeshProUGUI statNextValueText;     // 둘째줄 업글 후 MAX

    [Header("스탯별 이미지")]
    public Image characterImage;
    public Sprite[] statIcons;        // 포만감, 스태미너, 이동속도, 낚시, 휴식

    [Header("구매 버튼")]
    public Button buyButton;
    public TextMeshProUGUI buyCostText;  // 필요 재화량

    [Header("페이지 네비게이션")]
    public Button prevButton;      // < 버튼
    public Button nextButton;      // > 버튼
    public Transform dotContainer;  // 도트들이 들어갈 빈 옵젝 
    public GameObject nowDot;   // 현재 페이지 
    public GameObject waitDot;  // 다른 페이지
    [Header("캐릭터 현재 스탯")]
    public TextMeshProUGUI doongDoongStatText;  // 둥둥 스탯
    public TextMeshProUGUI currentStaminaText;  // 스태미너
    public TextMeshProUGUI currentHungerText;   // 포만감

    #endregion

    private List<UpgradeData> upgradeTable = new List<UpgradeData>();
    private PlayerData playerData;

    // 페이지 인디케이터 
    private StatType[] statPages = new StatType[]
    {
        StatType.BaseHunger,       // 포만감
        StatType.BaseStamina,      // 스태미너
        StatType.BaseMoveSpeed,    // 이동속도
        StatType.BaseFishingSpeed, // 낚시속도
        StatType.StaminaHeal       // 휴식속도 
    };
    private int currentPageIndex = 0;

    void Start()
    {
        //ResetDataAll();
        //ResetUpgradeLevels();
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerData = playerController.PlayerDataOld;
        else
            playerData = new PlayerData();

        if (playerData != null) // 둥둥스탯 이미지 
            playerData.OnDoongDoongChanged += OnDoongDoongChanged;

        upgradeTable = UpgradeTempData.GetAll();
 
        // 페이지 인디케이터 
        buyButton.onClick.AddListener(OnbuyClicked);
        prevButton.onClick.AddListener(OnPrevPage);
        nextButton.onClick.AddListener(OnNextPage);


        UpdatePage();
    }

    #region 둥둥스탯 이미지
    CharacterVisualDataSO GetVisualData(int doongDoongStat)
    {
        var visualList = DataManager.Instance.CharacterDatabase.CharacterVisualData.datas;
        foreach (var visual in visualList)
        {
            if (doongDoongStat >= visual.MinIndex && doongDoongStat < visual.MaxIndex)
                return visual;
        }
        // 못 찾으면 마지막 반환
        return visualList[visualList.Count - 1];
    }
    void OnDoongDoongChanged(int value)
    {
        if (characterImage == null) return;
        var visual = GetVisualData(value);
        if (visual != null)
            characterImage.sprite = visual.CharacterVisualImgPath_Sprite;
    }

    void OnDestroy()
    {
        if (playerData != null)
            playerData.OnDoongDoongChanged -= OnDoongDoongChanged;
        //// 이벤트 해제
        //if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        //    DataManager.Instance.Hub.OnDataLoaded -= OnDataLoaded;
    }
    #endregion

    #region 테이블 검색
    UpgradeData FindCurrentLevelData(StatType type, int level)
    {
        for (int i = 0; i < upgradeTable.Count; i++)
        {
            if (upgradeTable[i].StatType == type && upgradeTable[i].Level == level)
                return upgradeTable[i];
        }
        return null;
    }

    UpgradeData FindNextLevelData(StatType type, int level)
    {
        for (int i = 0; i < upgradeTable.Count; i++)
        {
            if (upgradeTable[i].StatType == type && upgradeTable[i].Level == level + 1)
                return upgradeTable[i];
        }
        return null;
    }

    // 해당 스탯의 최대 레벨
    int GetMaxLevel(StatType type)
    {
        int max = 0;
        for (int i = 0; i < upgradeTable.Count; i++)
        {
            if (upgradeTable[i].StatType == type && upgradeTable[i].Level > max)
                max = upgradeTable[i].Level;
        }
        return max;
    }
    #endregion

    #region 페이지 인디케이터 

    // < 버튼
    void OnPrevPage()
    {
        currentPageIndex--;
        if (currentPageIndex < 0)
            currentPageIndex = statPages.Length - 1;
        UpdatePage();
    }

    // > 버튼
    void OnNextPage()
    {
        currentPageIndex++;
        if (currentPageIndex >= statPages.Length)
            currentPageIndex = 0;
        UpdatePage();
    }


    // 도트 갱신 
    void UpdateDots()
    {
        foreach (Transform child in dotContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < statPages.Length; i++)
        {
            Instantiate(
                (i == currentPageIndex) ? nowDot : waitDot,
                dotContainer
            );
        }
    }

    #endregion

    #region 구매
    void OnbuyClicked()
    {
        StatType currentStat = statPages[currentPageIndex];

        switch (currentStat)
        {
            case StatType.BaseHunger: BuyHunger(); break;
            case StatType.BaseStamina: BuyStamina(); break;
            case StatType.BaseMoveSpeed: BuyMoveSpeed(); break;
            case StatType.BaseFishingSpeed: BuyFishingSpeed(); break;
            case StatType.StaminaHeal: BuyRestSpeed(); break;
        }
        _ = DataManager.Instance.Hub.UploadAllData();
    }

    void BuyHunger()
    {
        int currentLevel = playerData.HungerLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseHunger, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseHunger, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (GameManager.Instance.PlayerGold < currentLevelData.Cost) return;

        GameManager.Instance.SetGold(-currentLevelData.Cost);

        // Add 방식: 벨류만큼 최대치와 현재치 둘다 증가
        playerData.MaxHunger += currentLevelData.Value;
        playerData.SetHunger(playerData.Hunger + currentLevelData.Value);

        playerData.HungerLevel = currentLevel + 1;
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.HungerLevel,playerData.HungerLevel);
        UpdatePage();
    }

    void BuyStamina()
    {
        int currentLevel = playerData.StaminaLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseStamina, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseStamina, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (GameManager.Instance.PlayerGold < currentLevelData.Cost) return;

        GameManager.Instance.SetGold(-currentLevelData.Cost);

        // Add 방식: 벨류만큼 최대치와 현재치 둘다 증가
        playerData.MaxStamina += currentLevelData.Value;
        playerData.SetStamina(playerData.Stamina + currentLevelData.Value);

        playerData.StaminaLevel = currentLevel + 1;
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.StaminaLevel, playerData.StaminaLevel);
        UpdatePage();
    }

    void BuyMoveSpeed()
    {
        int currentLevel = playerData.MoveSpeedLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseMoveSpeed, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseMoveSpeed, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (GameManager.Instance.PlayerGold < currentLevelData.Cost) return;

        GameManager.Instance.SetGold(-currentLevelData.Cost);
        // Set 방식 : 다음 레벨 벨류로 덮어씌우기
        playerData.SetMoveSpeed(nextLevelData.Value);

        playerData.MoveSpeedLevel = currentLevel + 1;
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.MoveSpeedLevel, playerData.MoveSpeedLevel);
        UpdatePage();
    }

    void BuyFishingSpeed()
    {
        int currentLevel = playerData.FishingSpeedLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseFishingSpeed, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseFishingSpeed, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (GameManager.Instance.PlayerGold < currentLevelData.Cost) return;

        GameManager.Instance.SetGold(-currentLevelData.Cost);
        // Add 방식: 벨류만큼 현재치 감소
        playerData.MaxFishingSpeed -= currentLevelData.Value;   
        playerData.SetFishingSpeed(playerData.FishingSpeed - currentLevelData.Value); 

        playerData.FishingSpeedLevel = currentLevel + 1;
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.FishingSpeedLevel, playerData.FishingSpeedLevel);
        UpdatePage();
    }

    // 휴식속도
    void BuyRestSpeed()
    {
        int currentLevel = playerData.RestSpeedLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.StaminaHeal, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.StaminaHeal, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (GameManager.Instance.PlayerGold < currentLevelData.Cost) return;

        GameManager.Instance.SetGold(-currentLevelData.Cost);
        // Set 방식

        playerData.SetRestSpeed(nextLevelData.Value);

        playerData.RestSpeedLevel = currentLevel + 1;
        // TODO: 휴식속도 관련 쾌스트가 추가되면 주석해제 
       // QuestManager.Instance.SetSimpleProgress(QuestConditionKey.RestSpeedLevel, playerData.RestSpeedLevel);
        UpdatePage();
    }

    #endregion

    #region UI 업데이트
    // 현재 페이지 전체 갱신
    void UpdatePage()
    {
        StatType currentStat = statPages[currentPageIndex];
        int currentLevel = GetCurrentLevel(currentStat);
        int maxLevel = GetMaxLevel(currentStat);
        
        // 타이틀 제목 로컬라이징
        if (titleText != null)
            titleText.text = LocalizationManager.Instance.GetString("UpgradeStatDes");

        // 스탯 이름
        statNameText.text = GetStatNameSO(currentStat);

        // 돈
        goldText.text = GameManager.Instance.PlayerGold.ToString();

        // 필 피자와 레벨/최대레벨
        levelFillImage.fillAmount = (float)currentLevel / maxLevel;
        levelProgressText.text = currentLevel + "/" + maxLevel;

        // 변동 스탯
        UpdateStatChangeText(currentStat, currentLevel);

        // 스탯 아이콘 변경
        if (statIconImage != null && currentPageIndex < statIcons.Length)
            statIconImage.sprite = statIcons[currentPageIndex];

        // 둥둥스탯 기반 데미지 변경
        if (characterImage != null)                                    
        {
            var visual = GetVisualData(playerData.DoongDoongStat);
            if (visual != null)
                characterImage.sprite = visual.CharacterVisualImgPath_Sprite;
        }

        UpdateLeftPanelStats();  // 현재 스탯
        // 구매 버튼
        UpdatebuyButton(currentStat, currentLevel);

        // 인디케이터 
        UpdateDots();
    }

    // SO에서 스탯 이름 가져오기 (한/영)
    string GetStatNameSO(StatType type)
    {
        var db = DataManager.Instance.CharacterDatabase.CharacterUpgradeData;
        foreach (var so in db.datas)
        {
            if (so.statType == type)
                return so.StatName_String;
        }
        return type.ToString(); // 못 찾으면 enum 이름 반환
    }
    // 현재스탯 → 변동스탯
    void UpdateStatChangeText(StatType type, int level)
    {
        if (statCurrentValueText == null || statNextValueText == null) return;

        UpgradeData currentLevelData = FindCurrentLevelData(type, level);
        if (currentLevelData == null) return;

        // MAX 도달
        if (currentLevelData.IsMax)
        {
            //if (currentLevelData.applyType == ApplyType.Add)
            //    statCurrentValueText.text = GetCurrentAddValue(type).ToString("F0");
            //else
            //    statCurrentValueText.text = currentLevelData.Value.ToString("F2");
            statCurrentValueText.text = GetCurrentStatValue(type);  // 항상 실제 스탯에서 읽기
            statNextValueText.text = "MAX";
            return;
        }

        UpgradeData nextLevelData = FindNextLevelData(type, level);
        if (nextLevelData == null) return;

        if (currentLevelData.applyType == ApplyType.Add)
        {
            float curTotal = GetCurrentAddValue(type);
            //  statCurrentValueText.text = curTotal.ToString("F0");       // 첫째줄: 현재 MAX
            //  statNextValueText.text = (curTotal + currentLevelData.Value).ToString("F0"); // 셋째줄: 업글 후

            // 낚시속도는 소수점 표시
            if (type == StatType.BaseFishingSpeed)
                statCurrentValueText.text = curTotal.ToString("F1");
            else
                statCurrentValueText.text = curTotal.ToString("F0");
            // 낚시속도는 감소, 나머지는 증가
            if (type == StatType.BaseFishingSpeed)
                statNextValueText.text = (curTotal - currentLevelData.Value).ToString("F1");
            else
                statNextValueText.text = (curTotal + currentLevelData.Value).ToString("F0");
        }
        else
        {
            //statCurrentValueText.text = currentLevelData.Value.ToString("F2");  // 첫째줄
            statCurrentValueText.text = GetCurrentStatValue(type);  // 실제 playerData 값
            statNextValueText.text = nextLevelData.Value.ToString("F2");        // 셋째줄
        }
    }
    // Add/Set 구분 없이 playerData에서 실제 현재 스탯값 반환
    string GetCurrentStatValue(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return playerData.MaxHunger.ToString("F0");
            case StatType.BaseStamina: return playerData.MaxStamina.ToString("F0");
            case StatType.BaseMoveSpeed: return playerData.MoveSpeed.ToString("F2");
            case StatType.BaseFishingSpeed: return playerData.FishingSpeed.ToString("F2");
            case StatType.StaminaHeal: return playerData.RestSpeed.ToString("F2");
            default: return "0";
        }
    }
    // 왼쪽 패널 현재 스탯 3개 
    void UpdateLeftPanelStats()
    {
        if (playerData == null) return;

        if (doongDoongStatText != null)
            doongDoongStatText.text = playerData.DoongDoongStat.ToString();

        if (currentStaminaText != null)
            currentStaminaText.text = playerData.Stamina.ToString("F0");

        if (currentHungerText != null)
            currentHungerText.text = playerData.Hunger.ToString("F0");
    }

    // Add 전용 (현재 최종 누적값)
    float GetCurrentAddValue(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return playerData.MaxHunger;
            case StatType.BaseStamina: return playerData.MaxStamina;
            case StatType.BaseFishingSpeed: return playerData.MaxFishingSpeed;
            default: return 0;
        }
    }
    // 구매 버튼 상태 갱신
    void UpdatebuyButton(StatType type, int level)
    {
        UpgradeData currentLevelData = FindCurrentLevelData(type, level);
        if (currentLevelData == null) return;

        if (currentLevelData.IsMax)
        {
            buyCostText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            buyCostText.text = currentLevelData.Cost.ToString();
            buyButton.interactable = (GameManager.Instance.PlayerGold >= currentLevelData.Cost);
        }
    }

    // 현재 레벨 가져오기 (현재스탯, 변동스탯 Text UI용)
    int GetCurrentLevel(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return playerData.HungerLevel;
            case StatType.BaseStamina: return playerData.StaminaLevel;
            case StatType.BaseMoveSpeed: return playerData.MoveSpeedLevel;
            case StatType.BaseFishingSpeed: return playerData.FishingSpeedLevel;
            case StatType.StaminaHeal: return playerData.RestSpeedLevel;
            default: return 1;
        }
    }

    #endregion
    // 팝업창이 띄워지고 호출
    public void OnPanelOpened()
    {
        // playerData가 아직 없으면 초기화
        if (playerData == null)
        {
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                playerData = playerController.PlayerDataOld;
            else
                playerData = new PlayerData();
        }
     
        currentPageIndex = 0;
        UpdatePage();
    }

    [ContextMenu("테스트용 업그레이드 레벨 리셋")]
    void ResetUpgradeLevels()
    {
        // playerData 레벨도 같이 리셋
        if (playerData != null)
        {
            playerData.HungerLevel = 0;
            playerData.StaminaLevel = 0;
            playerData.MoveSpeedLevel = 0;
            playerData.FishingSpeedLevel = 0;
            playerData.RestSpeedLevel = 0;
        }

        var data = DataManager.Instance.Box.Character;
        if (data == null) return;

        data._hunger.Level = 0;

        data._moveSpeed.Level = 0;

        data._fishingSpeed.Level = 0;

        data._restSpeed.Level = 0;

        DataManager.Instance.SaveAll();
        DataManager.Instance.Hub.UploadAllData(); // 파베에도 반영
    }
    [ContextMenu("전체 데이터 초기화")]
    void ResetDataAll()
    {
        // 모든 유저 데이터를 새 걸로 교체
        DataManager.Instance.Hub._allUserData = new UserAllData();

        // Firebase에 빈 데이터 업로드
        DataManager.Instance.SaveAll();
        DataManager.Instance.Hub.UploadAllData();
    }
}
