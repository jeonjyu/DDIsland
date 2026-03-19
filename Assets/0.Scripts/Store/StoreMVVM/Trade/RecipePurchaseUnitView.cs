using UnityEngine;

// 구매만 가능한 팝업 뷰

public class RecipePurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;


    protected override void Awake()
    {
        base.Awake();
        strategy = GetComponent<PurchaseStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }

    public override void SetButton()
    {
        base.SetButton();

        if(viewModel.ItemCount == 1 && viewModel.IsGained == true)
        {
            //SetAllButtonAvailablity(false);
            //Debug.Log(this + " 레시피 구매 유닛 | 구매 가능");

        }
        else
        {
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
            //SetAllButtonAvailablity(true);
            //Debug.Log(this + " 레시피 구매 유닛 | 구매 불가");
        }



    }

}
