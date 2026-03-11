using System.ComponentModel;
using TMPro;
using UnityEngine;

// 구매/판매 시 모델 외에 모든 요소 초기화
// 모델 이름, 설명 띄우기

public class TradeViewBase : MonoBehaviour
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDesc;

    protected TradeViewModelBase viewModel;
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
        if(viewModel != null)
        {
            SetView();
            Debug.Log(viewModel.storeCat);
            Debug.Log(StoreCat.interior);
            if (viewModel.storeCat.ToString() == StoreCat.interior.ToString())
            {
                _equipTradeView.EquipButton.gameObject.SetActive(false);

            }
            else
            {
                _equipTradeView.EquipButton.gameObject.SetActive(true);

            }

        }
    }

    public void SetView()
    {
        itemName.text = viewModel.Model.ItemName;
        itemDesc.text = viewModel.Model.ItemDesc;
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
