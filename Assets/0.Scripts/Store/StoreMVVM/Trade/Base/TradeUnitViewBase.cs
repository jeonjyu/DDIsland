using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUnitViewBase : MonoBehaviour
{
    [SerializeField] Button tradeBtn;
    [SerializeField] Button countIncBtn;
    [SerializeField] Button countDecBtn;
    [SerializeField] Button countMaxBtn;
    [SerializeField] TMP_Text priceTxt;
    [SerializeField] TMP_Text countTxt;

    TradeUnitViewModelBase viewModel;

    private void Start()
    {

    }

    protected void SetEventListener()
    {
        tradeBtn.onClick.AddListener(() => viewModel.Trade(viewModel.TradeCount));
        countIncBtn.onClick.AddListener(() => viewModel.ChangeCount(1));
        countDecBtn.onClick.AddListener(() => viewModel.ChangeCount(-1));
    }

    public void SetTotalPrice(int price)
    {
        countTxt.text = price.ToString();
    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName))
        {
            // 유닛 초기화
        }
        else if(e.PropertyName == "totalPrice")
        {
            SetTotalPrice(viewModel.TotalPrice);
        }
    }
}
