/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegacyShopManager : MonoBehaviour
{
    [Header("상점 UI")]
    public GameObject shopBackground; // BackGround 연결
    public Text goldText;

    [Header("현재 스탯 표시")]
    public Text damageStatText;
    public Text speedStatText;
    public Text healthStatText;
    public Text regenStatText;
    public Text expStatText;
    public Text magnetStatText;

    [Header("버튼들")]
    public Button damageButton;
    public Button speedButton;
    public Button healthButton;
    public Button regenButton;
    public Button expButton;
    public Button magnetButton;

    [Header("버튼 텍스트")]
    public Text damageButtonText;
    public Text speedButtonText;
    public Text healthButtonText;
    public Text regenButtonText;
    public Text expButtonText;
    public Text magnetButtonText;

    [Header("업그레이드 레벨")]
    private int damageLevel = 0;
    private int speedLevel = 0;
    private int healthLevel = 0;
    private int regenLevel = 0;
    private int expLevel = 0;
    private int magnetLevel = 0;

    [Header("기본 비용")]
    public int damageCostBase = 100;
    public int speedCostBase = 220;
    public int healthCostBase = 150;
    public int regenCostBase = 180;
    public int expCostBase = 250;
    public int magnetCostBase = 200;

    // 임시 변수들 (플레이어로 스탯 옮겨야 함)
    private float expAdd = 1f; // 경험치 배율
    private float regenHp = 0f; // 체력 재생 (초당)
    private float regenTimer = 0f; // 재생 타이머

    private Player player;
    private bool isShopOpen = false;

    void Start()
    {
        player = FindObjectOfType<Player>();

        if (shopBackground != null) // 상점 숨기기 
        {
            shopBackground.SetActive(false);
        }

        // 버튼 클릭 연결
        damageButton.onClick.AddListener(BuyDamage);
        speedButton.onClick.AddListener(BuySpeed);
        healthButton.onClick.AddListener(BuyHealth);
        regenButton.onClick.AddListener(BuyRegen);
        expButton.onClick.AddListener(BuyExp);
        magnetButton.onClick.AddListener(BuyMagnet);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // B버튼 
        {
            ToggleShop();
        }

        // ESC키로도 닫기
        if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen)
        {
            CloseShop();
        }

        // 상점 열려있으면 UI 업데이트
        if (isShopOpen)
        {
            UpdateAllUI();
        }

        // 임시 체젠, 체력 재생 (player.cs에서 구현해야 할 듯) 
        if (regenHp > 0 && Time.timeScale > 0f && player != null)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 1f) // 1초마다
            {
                player.Heal(regenHp);
                regenTimer = 0f;
            }
        }

    }
    // 비용 계산 
    int GetDamageCost()
    {
        return damageCostBase + (damageLevel * 10);
    }

    int GetSpeedCost()
    {
        return speedCostBase + (speedLevel * 10);
    }

    int GetHealthCost()
    {
        return healthCostBase + (healthLevel * 10);
    }

    int GetRegenCost()
    {
        return regenCostBase + (regenLevel * 10);
    }

    int GetExpCost()
    {
        return expCostBase + (expLevel * 10);
    }

    int GetMagnetCost()
    {
        return magnetCostBase + (magnetLevel * 10);
    }

    // UI 업데이트
    void UpdateAllUI()
    {
        if (player == null) return;

        // 골드 표시
        goldText.text = "Gold : " + player.gold;

        // 현재 스탯 표시
        damageStatText.text = "현재 공격력 : " + (int)player.damage;
        speedStatText.text = "현재 속도 : " + player.moveSpeed.ToString("F1");
        healthStatText.text = "현재 최대 체력 : " + (int)player.maxHp;
        regenStatText.text = "현재 체력 재생률 : " + regenHp.ToString("F1") + "%";
        expStatText.text = "현재 exp배율 : " + (int)(expAdd * 100) + "%";
        magnetStatText.text = "현재 자석 범위 : " + player.magnetRange.ToString("F1");

        // 버튼 텍스트 
        damageButtonText.text = "공격력 증가 +5\n(" + GetDamageCost() + "골드)";
        speedButtonText.text = "속도 증가 +1\n(" + GetSpeedCost() + "골드)";
        healthButtonText.text = "최대 체력 증가 +10\n(" + GetHealthCost() + "골드)";
        regenButtonText.text = "체력 재생률 +1%\n(" + GetRegenCost() + "골드)";
        expButtonText.text = "EXP 배율 +5%\n(" + GetExpCost() + "골드)";
        magnetButtonText.text = "자석 범위 증가 +1\n(" + GetMagnetCost() + "골드)";
    }

    // 구매 함수들
    void BuyDamage()
    {
        int cost = GetDamageCost();

        if (player.SpendGold(cost)) // 변경됨: Gold 체크 대신 player.SpendGold()
        {
            player.damage += 5f;
            damageLevel++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("골드 부족 (공격력 업그레이드 실패)");
        }
    }

    void BuySpeed()
    {
        int cost = GetSpeedCost();

        if (player.SpendGold(cost))
        {
            player.moveSpeed += 1f;
            speedLevel++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("골드 부족 (이동속도 업그레이드 실패)");
        }
    }

    void BuyHealth()
    {
        int cost = GetHealthCost();
        if (player.SpendGold(cost))
        {
            // 최대 체력 늘리고 현재 체력도 같이 올려줌
            player.maxHp += 10f;
            player.currentHp += 10f;
            if (player.currentHp > player.maxHp)
            {
                player.currentHp = player.maxHp;
            }

            // 체력바 즉시 갱신 (Player 쪽에 HealthBarUI.UpdateBar() 같은 함수 있으면 호출해줘도 됨)
            // 예: player.UpdateHpUI(); 라는 함수가 있으면 여기서 불러.

            healthLevel++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("골드 부족 (체력 업그레이드 실패)");
        }

    }

    void BuyRegen()
    {
        int cost = GetRegenCost();

        if (player.SpendGold(cost))
        {
            regenHp += 1f; // 초당 체력 재생량 +1
            regenLevel++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("골드 부족 (체력 재생 업그레이드 실패)");
        }
    }

    void BuyExp()
    {
        int cost = GetExpCost();

        if (player.SpendGold(cost))
        {
            expAdd += 0.05f; // 경험치 획득량 5% 증가
            expLevel++;
            UpdateAllUI();

            // 나중에 player.GainExp() 내부에서 expAdd를 곱해서
            // 경험치 아이템을 더 많이 주는 식으로 연결할 수 있음
            // (예: player.AddExpMultiplier(expAdd); 이런 식으로)
        }
        else
        {
            Debug.Log("골드 부족 (EXP 업그레이드 실패)");
        }
    }

    void BuyMagnet()
    {
        int cost = GetMagnetCost();

        if (player.SpendGold(cost))
        {
            player.magnetRange += 1f;
            magnetLevel++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("골드 부족 (마그넷 업그레이드 실패)");
        }
    }

    // 상점 열기/닫기 토글
    void ToggleShop()
    {
        if (isShopOpen)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        shopBackground.SetActive(true); // 배경 보이기 
        Time.timeScale = 0f; // 게임 멈춤 
        isShopOpen = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        UpdateAllUI();
    }

    public void CloseShop()
    {
        shopBackground.SetActive(false); // 배경 숨기기
        Time.timeScale = 1f; // 게임 재개
        isShopOpen = false;
    }
}
*/