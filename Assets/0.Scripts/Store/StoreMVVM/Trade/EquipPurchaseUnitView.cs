using UnityEngine;

public class EquipPurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;
    public EquipTradeView _equipTradeView;
    public EquipTradeViewModel tradeViewModel;

    protected override void Awake()
    {
        base.Awake();
        strategy = GetComponent<PurchaseStrategy>();
        tradeViewModel = GetComponent<EquipTradeViewModel>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        if( strategy == null ) strategy = GetComponent<PurchaseStrategy>();
        return strategy;
    }

    public override void SetButton()
    {
        base.SetButton();

        // 구매할 아이템의 가격이 보유 골드보다 큰 경우 구매 불가
        if (GameManager.Instance.PlayerGold < strategy.GetPrice(StoreManager.Instance.TradeModel))
        {
            Debug.Log("아이템 가격이 보유 골드보다 큼");
            SetBtnInteractable(tradeBtn, false);
        }
        else
        {
            Debug.Log("아이템 가격이 보유 골드보다 작음");
            SetBtnInteractable(tradeBtn, !viewModel.IsGained);
        }

    }
}
