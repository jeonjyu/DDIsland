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
    public TextMeshProUGUI statChangeText;    // 변동스탯 
    [Header("스탯별 이미지")]
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
            playerData = playerController.PlayerDataOld;
        else
            playerData = new PlayerData();
       
        upgradeTable = UpgradeTempData.GetAll();

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
            case StatType.StaminaHeal: BuyRestSpeed(); break; 
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
        statNameText.text = UpgradeStringData.GetDisplayName(currentStat);

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

    // 현재스탯 → 변동스탯
    void UpdateStatChangeText(StatType type, int level)
    {
        if (statChangeText == null) return; 

        UpgradeData currentLevelData = FindCurrentLevelData(type, level);
        if (currentLevelData == null) return;

        if (currentLevelData.IsMax)
        {
            if (currentLevelData.applyType == ApplyType.Add)
                statChangeText.text = GetCurrentAddValue(type).ToString("F0");
            else
                statChangeText.text = currentLevelData.Value.ToString("F2");
            return;
        }

        UpgradeData nextLevelData = FindNextLevelData(type, level);
        if (nextLevelData == null) return;

        if (currentLevelData.applyType == ApplyType.Add)
        {
            float curTotal = GetCurrentAddValue(type);
            statChangeText.text = curTotal.ToString("F0") + " → " + (curTotal + currentLevelData.Value).ToString("F0");
        }
        else
        {
            statChangeText.text = currentLevelData.Value.ToString("F2") + " → " + nextLevelData.Value.ToString("F2");
        }
    }

    // Add 전용 (현재 최종 누적값)
    float GetCurrentAddValue(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return playerData.MaxHunger;
            case StatType.BaseStamina: return playerData.MaxStamina;
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
    int GetBodyIndex(int doongDoongStat)
    {
        for (int i = 0; i < bodyThresholds.Length; i++)
        {
            if (doongDoongStat < bodyThresholds[i])
                return i;
        }
        return bodySprites.Length - 1;
    }
 
}
