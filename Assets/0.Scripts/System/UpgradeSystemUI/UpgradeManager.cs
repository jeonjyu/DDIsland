using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    #region UI 변수
    [Header("상점 UI")]
    public Text goldText;

    [Header("플레이어 캐릭터 프리팹")]
    public GameObject playerCharacter; // 테스트용 
    //public PlayerController playerController; // 나중에

    [Header("현재 스탯 표시")]
    public Text hungerStatText;
    public Text staminaStatText;
    public Text moveSpeedText;
    public Text fishingSpeedStatText;

    [Header("구매 버튼")]
    public Button hungerButton;
    public Button staminaButton;
    public Button moveSpeedButton;
    public Button fishingSpeedButton;

    [Header("버튼 텍스트")]
    public Text hungerButtonText;
    public Text staminaButtonText;
    public Text moveSpeedButtonText;
    public Text fishingSpeedButtonText;

    [Header("레벨 텍스트")]
    public Text hungerLevelText;
    public Text staminaLevelText;
    public Text moveSpeedLevelText;
    public Text fishingSpeedLevelText;
    #endregion



    public int BitCoin = 99999; // 테스트용 임시 골드 가상화폐 
    //업그레이드 테이블 나중에 교체 
    private List<UpgradeData> upgradeTable = new List<UpgradeData>();


    private PlayerData playerData;

    void Start()
    {
        // 테스트용 임시 (나중에 PlayerContoller에서 받아와야 함)
        playerData = new PlayerData();
        //playerData = playerController.PlayerData;

        InitTempTable(); // 테이블 임시 나중에 CSV로 교체 

        hungerButton.onClick.AddListener(BuyHunger);
        staminaButton.onClick.AddListener(BuyStamina);
        moveSpeedButton.onClick.AddListener(BuyMoveSpeed);
        fishingSpeedButton.onClick.AddListener(BuyFishingSpeed);
    }


    // 테이블 검색 
    UpgradeData FindCurrentLevelData(StatType type, int level)
    {
        foreach (var data in upgradeTable)
        {
            if (data.StatType == type && data.Level == level)
                return data;
        }
        return null;
    }
    UpgradeData FindNextLevelData(StatType type, int level)
    {
        foreach (var data in upgradeTable)
        {
            if (data.StatType == type && data.Level == level + 1)
                return data;
        }
        return null;
    }

    #region // 구매 함수들
    void BuyHunger()
    {
        int currentLevel = playerData.HungerLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseHunger, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseHunger, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (BitCoin < currentLevelData.Cost) return;

        BitCoin -= currentLevelData.Cost;

        // Add 방식: 벨류만큼 최대치와 현재치 둘다 증가 
        playerData.MaxHunger += currentLevelData.Value;
        playerData.SetHunger(playerData.Hunger + currentLevelData.Value);

        playerData.HungerLevel = currentLevel + 1;
        UpdateAllUI();
    }


    void BuyStamina()
    {
        int currentLevel = playerData.StaminaLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseStamina, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseStamina, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (BitCoin < currentLevelData.Cost) return;

        BitCoin -= currentLevelData.Cost;

        // Add 방식: 벨류만큼 최대치와 현재치 둘다 증가 
        playerData.MaxStamina += currentLevelData.Value;
        playerData.SetStamina(playerData.Stamina + currentLevelData.Value);

        playerData.StaminaLevel = currentLevel + 1;
        UpdateAllUI();
    }

    void BuyMoveSpeed()
    {
        int currentLevel = playerData.MoveSpeedLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseMoveSpeed, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseMoveSpeed, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (BitCoin < currentLevelData.Cost) return;

        BitCoin -= currentLevelData.Cost;
        // Set 방식 : 다음 레벨 벨류로 덮어씌우기 
        playerData.SetMoveSpeed(nextLevelData.Value);

        playerData.MoveSpeedLevel = currentLevel + 1; //레벨업
        UpdateAllUI();
    }


    void BuyFishingSpeed()
    {
        int currentLevel = playerData.FishingSpeedLevel;
        UpgradeData currentLevelData = FindCurrentLevelData(StatType.BaseFishingSpeed, currentLevel);
        UpgradeData nextLevelData = FindNextLevelData(StatType.BaseFishingSpeed, currentLevel);

        if (currentLevelData == null || nextLevelData == null) return;
        if (currentLevelData.IsMax) return;
        if (BitCoin < currentLevelData.Cost) return;

        BitCoin -= currentLevelData.Cost;
        // Set 방식 : 다음 레벨 벨류로 덮어씌우기 
        playerData.SetFishingSpeed(nextLevelData.Value);

        playerData.FishingSpeedLevel = currentLevel + 1; //레벨업
        UpdateAllUI();
    }
    #endregion
    void Update()
    {

    }



    // UI 업데이트
    void UpdateAllUI()
    {
        goldText.text = BitCoin.ToString();

        UpdateStatUI(StatType.BaseHunger, playerData.HungerLevel, hungerLevelText, hungerStatText, hungerButtonText, hungerButton);
        UpdateStatUI(StatType.BaseStamina, playerData.StaminaLevel, staminaLevelText, staminaStatText, staminaButtonText, staminaButton);
        UpdateStatUI(StatType.BaseMoveSpeed, playerData.MoveSpeedLevel, moveSpeedLevelText, moveSpeedText, moveSpeedButtonText, moveSpeedButton);
        UpdateStatUI(StatType.BaseFishingSpeed, playerData.FishingSpeedLevel, fishingSpeedLevelText, fishingSpeedStatText, fishingSpeedButtonText, fishingSpeedButton);
    }






    void UpdateStatUI(StatType type, int level, Text levelText, Text statText, Text buttonText, Button button)
    {
        UpgradeData currentLevelData = FindCurrentLevelData(type, level);
        if (currentLevelData == null) return;

        levelText.text = "Lv." + level;

        if (currentLevelData.IsMax) // 최대치에 도달하면 
        {
            if (currentLevelData.applyType == ApplyType.Add)
                statText.text = GetCurrentAddValue(type).ToString("F0");
            else
                statText.text = currentLevelData.Value.ToString("F2");

            buttonText.text = "Max";
            button.interactable = false; // 버튼 비활성화 
        }
        else
        {
            UpgradeData nextLevelData = FindNextLevelData(type, level);
            if (nextLevelData == null) return;

            if (currentLevelData.applyType == ApplyType.Add) // add 방식이면 
            {
                float curTotal = GetCurrentAddValue(type);
                statText.text = curTotal.ToString("F0") + "→" + (curTotal + currentLevelData.Value).ToString("F0");
            }
            else // Set 방식 
            {
                statText.text = currentLevelData.Value.ToString("F2") + "→" + nextLevelData.Value.ToString("F2");
            }

            buttonText.text = currentLevelData.Cost.ToString();
            button.interactable = (BitCoin >= currentLevelData.Cost); // 구매버튼 활성화 
        }
    }

    // add전용함수: 최종값 받아오는 용도 
    float GetCurrentAddValue(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger:
                return playerData.MaxHunger;
            case StatType.BaseStamina:
                return playerData.MaxStamina;
            default: return 0;

        }
    }

    // 팝업창이 띄워지고 호출 
    public void OnPanelOpened()
    {
        UpdateAllUI();
    }

    #region 임시 테이블 (나중에 csv로 교체) 

    void InitTempTable()
    {
        // 포만감 (ApplyType=1:Add, StatType=1:BaseHunger, Value=증가량)
        upgradeTable.Add(new UpgradeData { ID = 200010101, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 1, Cost = 300, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010102, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 2, Cost = 450, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010103, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 3, Cost = 650, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010104, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 4, Cost = 900, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010105, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 5, Cost = 1200, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010106, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 6, Cost = 1550, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010107, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 7, Cost = 2000, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010108, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 8, Cost = 2500, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010109, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 9, Cost = 3050, Value = 10f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010110, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)1, Level = 10, Cost = 3700, Value = 10f, Probability = 1f, IsMax = true });

        // 스태미나 (ApplyType=1:Add, StatType=2:BaseStamina, Value=증가량)
        upgradeTable.Add(new UpgradeData { ID = 200010201, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 1, Cost = 30, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010202, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 2, Cost = 45, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010203, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 3, Cost = 65, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010204, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 4, Cost = 90, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010205, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 5, Cost = 120, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010206, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 6, Cost = 155, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010207, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 7, Cost = 200, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010208, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 8, Cost = 250, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010209, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 9, Cost = 305, Value = 100f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010210, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)1, StatType = (StatType)2, Level = 10, Cost = 370, Value = 100f, Probability = 1f, IsMax = true });

        // 이동속도 (ApplyType=2:Set, StatType=3:BaseMoveSpeed, Value=최종값)
        upgradeTable.Add(new UpgradeData { ID = 200010301, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 1, Cost = 30, Value = 1.1f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010302, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 2, Cost = 40, Value = 1.2f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010303, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 3, Cost = 65, Value = 1.3f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010304, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 4, Cost = 90, Value = 1.4f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010305, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 5, Cost = 120, Value = 1.5f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010306, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 6, Cost = 155, Value = 1.6f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010307, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 7, Cost = 200, Value = 1.7f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010308, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 8, Cost = 250, Value = 1.8f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010309, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 9, Cost = 305, Value = 1.9f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010310, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)3, Level = 10, Cost = 370, Value = 2.0f, Probability = 1f, IsMax = true });

        // 낚시속도 (ApplyType=2:Set, StatType=4:BaseFishingSpeed, Value=최종값)
        upgradeTable.Add(new UpgradeData { ID = 200010401, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 1, Cost = 300, Value = 1.01f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010402, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 2, Cost = 450, Value = 1.02f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010403, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 3, Cost = 650, Value = 1.03f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010404, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 4, Cost = 900, Value = 1.04f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010405, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 5, Cost = 1200, Value = 1.05f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010406, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 6, Cost = 1550, Value = 1.06f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010407, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 7, Cost = 2000, Value = 1.07f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010408, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 8, Cost = 2500, Value = 1.08f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010409, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 9, Cost = 3050, Value = 1.09f, Probability = 1f, IsMax = false });
        upgradeTable.Add(new UpgradeData { ID = 200010410, GroupID = "KeyBearUpgradeGroup", applyType = (ApplyType)2, StatType = (StatType)4, Level = 10, Cost = 3700, Value = 1.1f, Probability = 1f, IsMax = true });

       
    }
    #endregion
}