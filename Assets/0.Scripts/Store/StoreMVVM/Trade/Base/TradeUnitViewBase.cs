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
        viewModel.PropertyChanged += OnViewModelPropChanged;

        //SetEventListener();
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
    // 팝업 끄기 전까지 클릭 불가능
    public void SetButtonAvailable(bool available)
    {
        tradeBtn.enabled = available;
    }


    // 들어온 값으로 최종 가격 설정
    public void SetTotalPriceText(int price)
    {
        priceTxt.text = price.ToString();
    }

    public void SetItemCount(int count)
    {
        Debug.Log("itemcount : " + count );
        countTxt.text = count.ToString(); 
    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case null: case " ":
                // 유닛 초기화
                Debug.Log("[TradeUnitViewBase] 유닛 초기화");
                viewModel.SetTotalPrice();
                break;
            case nameof(viewModel.TradeCount) :
                //viewModel.ChangeCount(viewModel.TradeCount);
                SetItemCount(viewModel.TradeCount);
                viewModel.SetTotalPrice();
                //SetTotalPriceText(viewModel.);
                break;

        }
    }
}
