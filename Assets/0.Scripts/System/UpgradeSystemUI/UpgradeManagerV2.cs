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

    [Header("둥둥스탯 체형 이미지")]
    public Sprite[] bodySprites; // 예: [0]날씬, [1]보통, [2]뚱뚱
    public Image characterImage;
    [Header("체형 구간 기준")]
    public int[] bodyThresholds = { 200, 400 }; // 예: 0~200 = 0번, 200~400 = 1번, 400~600= 2번

    [Header("스탯 정보 표시 (우측 패널)")]
    public Image statIconImage;               // 스탯 아이콘
    public TextMeshProUGUI statNameText;      // 스탯 이름
    public Image levelFillImage;              // 피자 
    public TextMeshProUGUI levelProgressText; // 1/10 
    [Header("스탯별 이미지")]
    public Sprite[] statIcons;        // 포만감, 스태미너, 이동속도, 낚시, 휴식

    [Header("구매 버튼")]
    public Button buyButton;
    public TextMeshProUGUI buyCostText;  // 필요 재화량

    [Header("페이지 네비게이션")]
    public Button prevButton;      // < 버튼
    public Button nextButton;      // > 버튼
    public Transform dotContainer;  // 도트가 들어갈 빈 옵젝 
    public GameObject nowDot;   // 현재 페이지 
    public GameObject waitDot;  // 다른 페이지
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
        if (playerData != null) // 둥둥스탯 이미지 
            playerData.OnDoongDoongChanged += OnDoongDoongChanged;
       
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerData = playerController.PlayerData;
        else
            playerData = new PlayerData();
        InitTempTable();

       // 페이지 인디케이터 
        buyButton.onClick.AddListener(OnbuyClicked);
        prevButton.onClick.AddListener(OnPrevPage);
        nextButton.onClick.AddListener(OnNextPage);


        UpdatePage();
    }
    #region 둥둥스탯 이미지
    void OnDoongDoongChanged(int value)
    {
        if (characterImage != null && bodySprites.Length > 0)
        {
            int bodyIndex = GetBodyIndex(value);
            characterImage.sprite = bodySprites[bodyIndex];
        }
    }

    void OnDestroy()
    {
        if (playerData != null)
            playerData.OnDoongDoongChanged -= OnDoongDoongChanged;
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
            case StatType.StaminaHeal: BuyRestSpeed(); break; // 신규
        }
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
        // Set 방식 : 다음 레벨 벨류로 덮어씌우기
        playerData.SetFishingSpeed(nextLevelData.Value);

        playerData.FishingSpeedLevel = currentLevel + 1;
        UpdatePage();
    }

    // 신규: 휴식속도
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

        // 스탯 이름
        statNameText.text = GetStatDisplayName(currentStat);

        // 돈
        goldText.text = GameManager.Instance.PlayerGold.ToString();

        // 필 피자와 레벨/최대레벨
        levelFillImage.fillAmount = (float)currentLevel / maxLevel;
        levelProgressText.text = currentLevel + "/" + maxLevel;

        // 스탯 아이콘 변경
        if (statIconImage != null && currentPageIndex < statIcons.Length)
            statIconImage.sprite = statIcons[currentPageIndex];

        // 둥둥스탯 기반 데미지 변경
        if (characterImage != null && bodySprites.Length > 0)
        {
            int bodyIndex = GetBodyIndex(playerData.DoongDoongStat);
            characterImage.sprite = bodySprites[bodyIndex];
        }
        // 구매 버튼
        UpdatebuyButton(currentStat, currentLevel);

        // 인디케이터 
        UpdateDots();
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

    // 현재 레벨 가져오기 (UI용)
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

    #region 임시용 테이블 
    string GetStatDisplayName(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return "포만감";
            case StatType.BaseStamina: return "스태미너";
            case StatType.BaseMoveSpeed: return "이동속도";
            case StatType.BaseFishingSpeed: return "낚시 숙련도";
            case StatType.StaminaHeal: return "휴식속도";
            default: return "";
        }
    }

    string GetStatNameKey(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return "HungerStatDes_String";
            case StatType.BaseStamina: return "StaminaStatDes_String";
            case StatType.BaseMoveSpeed: return "MoveSpeedStatDes_String";
            case StatType.BaseFishingSpeed: return "FishingSpeedStatDes_String";
            case StatType.StaminaHeal: return "RestSpeedStatDes_String";
            default: return "";
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
                playerData = playerController.PlayerData;
            else
                playerData = new PlayerData();
        }

        currentPageIndex = 0;
        UpdatePage();
    }

    #region 임시 테이블 (나중에 CSV로 교체)

    void InitTempTable()
    {
        // 포만감 (Add, StatType=1, 레벨 10, Value=증가량 +10)
        AddUpgradeRange((ApplyType)1, (StatType)1, 200010101, "HungerStatDes_String",
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            10f);

        // 스태미너 (Add, StatType=2, 레벨 10, Value=증가량 +10)
        AddUpgradeRange((ApplyType)1, (StatType)2, 200010201, "StaminaStatDes_String",
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            10f);

        // 이동속도 (Set, StatType=3, 레벨 10, Value=최종값)
        AddUpgradeRange((ApplyType)2, (StatType)3, 200010301, "MoveSpeedStatDes_String",
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            0f, new float[] { 1.01f, 1.02f, 1.03f, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f, 1.09f, 1.10f });

        // 낚시 숙련도 (Set, StatType=4, 레벨 10, Value=최종값)
        AddUpgradeRange((ApplyType)2, (StatType)4, 200010401, "FishingSpeedStatDes_String",
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            0f, new float[] { 1.01f, 1.02f, 1.03f, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f, 1.09f, 1.10f });

        // 휴식속도 (Set, StatType=5, 레벨 3, Value=최종값)
        AddUpgradeRange((ApplyType)2, (StatType)5, 200010501, "RestSpeedStatDes_String",
            new int[] { 5000, 15000, 50000 },
            0f, new float[] { 0.03f, 0.04f, 0.05f });
    }

    // 헬퍼: 레벨 데이터 한번에 추가
    void AddUpgradeRange(ApplyType applyType, StatType statType, int startID, string statNameString,
        int[] costs, float fixedValue, float[] levelValues = null)
    {
        int totalLevels = costs.Length;
        for (int i = 0; i < totalLevels; i++)
        {
            upgradeTable.Add(new UpgradeData
            {
                ID = startID + i,
                GroupID = "501",
                applyType = applyType,
                StatType = statType,
              //  StatNameString = statNameString,
                Level = i + 1,
                Cost = costs[i],
                Value = (levelValues != null) ? levelValues[i] : fixedValue,
                Probability = 1f,
                IsMax = (i == totalLevels - 1)
            });
        }
    }
    int GetBodyIndex(int doongDoongStat)
    {
        for (int i = 0; i < bodyThresholds.Length; i++)
        {
            if (doongDoongStat < bodyThresholds[i])
                return i;
        }
        return bodySprites.Length - 1;
    }
    #endregion
}
