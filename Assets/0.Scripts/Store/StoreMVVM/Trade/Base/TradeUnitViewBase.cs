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


    protected virtual void Start()
    {
        Debug.Log("[TradeUnitViewBase] Start");

        viewModel = GetComponent<TradeUnitViewModelBase>();

        SetEventListener();
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
        countIncBtn.onClick.AddListener(() => viewModel.ChangeCount(1));
        countDecBtn.onClick.AddListener(() => viewModel.ChangeCount(-1));
        //countMaxBtn.onClick.AddListener(() => viewModel.ChangeCount(-1));
    }

    protected void UnsetEventListener()
    {
        tradeBtn.onClick.RemoveAllListeners();
        countIncBtn.onClick.RemoveAllListeners();
        countDecBtn.onClick.RemoveAllListeners();
    }

    public abstract ITradeStrategy GetTradeStrategy();

    // 들어온 값으로 최종 가격 설정
    public void SetTotalPriceText(int price)
    {
        countTxt.text = price.ToString();
    }

    // 거래 버튼 활성화 여부 설정
    // 팝업 끄기 전까지 클릭 불가능
    public void SetButtonAvailable(bool available)
    {
        tradeBtn.enabled = available;
    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case null: case " ":
                // 유닛 초기화
                break;
            case nameof(viewModel.TradeCount) :
                viewModel.SetTotalPrice();
                viewModel.ChangeCount(viewModel.TradeCount);
                break;

        }
    }
}
