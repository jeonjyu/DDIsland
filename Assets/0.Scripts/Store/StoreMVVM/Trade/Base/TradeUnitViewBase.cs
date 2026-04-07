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
        tradeBtn.onClick.AddListener(() => viewModel.ExcuteTrade(GetTradeStrategy()));
    }

    protected virtual void UnsetEventListener()
    {
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
        Debug.Log(e.PropertyName);
        UpdateUI();
    }
}
