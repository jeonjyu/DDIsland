using System;
using UnityEngine;

public class EquipSellUnitView : TradeUnitViewBase
{
    SellStrategy strategy;
    public EquipTradeView _equipTradeView;
    public EquipTradeViewModel tradeViewModel;

    protected override void Awake()
    {
        base.Awake();
        strategy = GetComponent<SellStrategy>();
        //tradeViewModel = GetComponent<EquipTradeViewModel>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerManager.Instance.OnEquipChanged += EquipBtnHandler;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerManager.Instance.OnEquipChanged -= EquipBtnHandler;
    }
    public void EquipBtnHandler(Enum filter, int itemObjectId)
    {
        SetButton();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        if (strategy == null) strategy = GetComponent<SellStrategy>();
        return strategy;
    }

    public override void SetButton()
    {

        base.SetButton();

        // 거래 불가능한 아이템일 경우 판매 유닛 버튼 클릭 불가능
        if (viewModel.Model.IsSaleable == false)
        {
            Debug.Log(this + " 판매 유닛 | 판매 불가능");
            SetBtnInteractable(tradeBtn, false);
        }
        else
            SetBtnInteractable(tradeBtn, !tradeViewModel.IsEquipped && tradeViewModel.IsGained);
    }
}
