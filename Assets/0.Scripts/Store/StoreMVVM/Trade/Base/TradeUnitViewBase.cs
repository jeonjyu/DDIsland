using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TradeUnitViewBase : MonoBehaviour
{
    [SerializeField] protected Button tradeBtn;
    [SerializeField] protected TMP_Text priceTxt;
    
    protected TradeUnitViewModelBase viewModel;

    public Button TradeBtn => tradeBtn;

    protected virtual void Awake()
    {
        viewModel = GetComponent<TradeUnitViewModelBase>();

        //Debug.Log("공통의 모델 이벤트 구독");
    }

    protected virtual void OnEnable()
    {
        viewModel.PropertyChanged += UpdateTradeUnitUI;
        SetEventListener();
    }

    protected virtual void OnDisable()
    {
        UnsetEventListener();
        viewModel.PropertyChanged -= UpdateTradeUnitUI;

    }

    protected virtual void SetEventListener()
    {
        //Debug.Log("이벤트 등록해주기 " + GetTradeStrategy());
        tradeBtn.onClick.AddListener(() => viewModel.ExcuteTrade(GetTradeStrategy()));
    }

    protected virtual void UnsetEventListener()
    {
        //Debug.Log("이벤트 삭제해주기 " + GetTradeStrategy());
        tradeBtn.onClick.RemoveAllListeners();
    }

    public abstract ITradeStrategy GetTradeStrategy();

    // 버튼 클릭 가능 여부 설정
    public void SetBtnInteractable(Button btn, bool isAvailable)
    {
        btn.interactable = isAvailable;
    }

    // 거래 유닛 UI의 최종 거래 가격 설정
    public void SetTotalPriceText(int price)
    {
        priceTxt.text = price.ToString();
    }

    /// <summary>
    /// 거래 유닛 UI의 거래 개수 설정
    /// </summary>
    /// <param name="count">거래할 아이템 개수</param>
    public virtual void SetTradeCountText(int count) { }


    // 버튼이 변경되어야 할 경우
    // 아이템 미보유시 > 판매 유닛 버튼들 비활성화
    // 아이템 보유 개수 최대일 경우 > 구매 유닛 버튼들 비활성화
    // 구매시 : 플레이어 골드보다 총합 가격이 높을 경우 > max, + 버튼 비활성화
    // 판매시 : 플레이어 보유 아이템 갯수보다 높을 경우 > max, + 버튼 비활성화
    /// <summary>
    /// 거래 유닛 UI별 버튼 클릭 가능 여부
    /// </summary>
    public virtual void SetButton()
    {
        SetAllButtonAvailablity(true);
    }
    
    public virtual void SetAllButtonAvailablity(bool isAvailable)
    {
        tradeBtn.interactable = isAvailable;
    }

    protected virtual void UpdateUI()
    {
        viewModel.SetTotalPrice();
        SetButton();
    }

    // TradeUnitViewModelBase의 TradeCount, ItemCount 변경시 실행
    private void UpdateTradeUnitUI(object sender, PropertyChangedEventArgs e)
    {
        //Debug.Log("공통 필드 변경되어 업데이트 " + this.name + " " + e.PropertyName);
        switch (e.PropertyName)
        {
            // 해당 유닛 뷰 요소들 TradeCount에 따라 갱신
            case nameof(viewModel.TradeCount):
                //Debug.Log("UpdateTradeUnitUI | " + nameof(viewModel.TradeCount));
                // 해당하는 거래 유형만) UI 가격, 개수, 개수 버튼
                UpdateUI();
                break;
            // 모든 유닛 뷰 요소 초기화
            case nameof(StoreManager.Instance.TradeItemCount):
                //Debug.Log("UpdateTradeUnitUI |" + nameof(StoreManager.Instance.TradeItemCount));
                UpdateUI();
                break;
        }
    }
}
