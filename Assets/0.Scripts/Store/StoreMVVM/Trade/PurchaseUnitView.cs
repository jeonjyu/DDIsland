using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PurchaseStrategy))]
public class PurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;

    private void Start()
    {
        strategy = GetComponent<PurchaseStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }
    public override void SetButton()
    {
        //base.SetButton();
        // 거래 불가능한 아이템일 경우 거래 유닛 버튼 클릭 불가능
        if (viewModel.Model.IsSaleable == false)
        {
            Debug.Log("거래 불가능");
            SetAllButtonAvailablity(false);
            return;
        }
    }
}
