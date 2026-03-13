using System.ComponentModel;
using TMPro;
using UnityEngine;

// 구매/판매 시 모델 외에 모든 요소 초기화
// 모델 이름, 설명 띄우기

public class TradeViewBase : MonoBehaviour
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDesc;

    TradeViewModelBase viewModel;
    public EquipTradeView _equipTradeView;


    public void Awake()
    {
        viewModel = GetComponent<TradeViewModelBase>();
    }

    public void Start()
    {
        viewModel.PropertyChanged += OnViewModelPropChanged;

    }

    private void OnEnable()
    {
        SetView();

    }

    public void SetView()
    {
        itemName.text = viewModel.Model.ItemName;
        itemDesc.text = viewModel.Model.ItemDesc;
        switch (viewModel.storeCat)
        {
            case StoreCat.interior:
                _equipTradeView.EquipButton.gameObject.SetActive(false);
                break;
            default:
                _equipTradeView.EquipButton.gameObject.SetActive(true);
                break;
        }
    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case null:
            case "":
                SetView();
                break;


        }
    }
}
