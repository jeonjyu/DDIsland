using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TradeUnitViewBase : MonoBehaviour
{
    [SerializeField] protected Button tradeBtn;
    [SerializeField] protected Button countIncBtn;
    [SerializeField] protected Button countDecBtn;
    [SerializeField] protected Button countMaxBtn;
    [SerializeField] protected TMP_Text priceTxt;
    [SerializeField] protected TMP_Text countTxt;

    protected TradeUnitViewModelBase viewModel;

    public Button TradeBtn => tradeBtn;
    public Button CountIncBtn => countIncBtn;
    public Button CountDecBtn => countDecBtn;   
    public Button CountMaxBtn => countMaxBtn;

    protected virtual void Awake()
    {
        viewModel = GetComponent<TradeUnitViewModelBase>();
        viewModel.PropertyChanged += OnViewModelPropChanged;
    }

    private void OnEnable()
    {
        SetEventListener();
    }

    private void OnDisable()
    {
        UnsetEventListener();
    }

    protected void SetEventListener()
    {
        tradeBtn.onClick.AddListener(() => viewModel.ExcuteTrade(GetTradeStrategy()));
        countIncBtn.onClick.AddListener(() => viewModel.IncreaseCount());
        countDecBtn.onClick.AddListener(() => viewModel.DecreaseCount());
        countMaxBtn.onClick.AddListener(() => viewModel.SetMaxCount());
    }

    protected void UnsetEventListener()
    {
        tradeBtn.onClick.RemoveAllListeners();
        countIncBtn.onClick.RemoveAllListeners();
        countDecBtn.onClick.RemoveAllListeners();
    }

    public abstract ITradeStrategy GetTradeStrategy();

    // 거래 버튼 활성화 여부 설정
    public void SetBtnInteractable(Button btn, bool isAvailable)
    {
        btn.interactable = isAvailable;
    }


    // 들어온 값으로 최종 가격 설정
    public void SetTotalPriceText(int price)
    {
        priceTxt.text = price.ToString();
    }

    public void SetItemCount(int count)
    {
        countTxt.text = count.ToString(); 
    }
    public virtual void SetButton()
    {
    }
    public void SetAllButtonAvailablity(bool isAvailable)
    {
        tradeBtn.interactable = isAvailable;
        countIncBtn.interactable = isAvailable;
        countDecBtn.interactable = isAvailable;
        countMaxBtn.interactable = isAvailable;

    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case null: case " ":
                // 유닛 초기화
                //Debug.Log("[TradeUnitViewBase] 유닛 초기화");
                viewModel.SetTotalPrice();
                break;
            case nameof(viewModel.TradeCount) :
                SetItemCount(viewModel.TradeCount);
                viewModel.SetTotalPrice();
                break;

        }
    }
}
