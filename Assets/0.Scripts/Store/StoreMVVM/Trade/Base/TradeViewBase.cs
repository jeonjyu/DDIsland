using System.ComponentModel;
using TMPro;
using UnityEngine;

// 구매/판매 시 모델 외에 모든 요소 초기화
// 모델 이름, 설명 띄우기

public class TradeViewBase : MonoBehaviour
{
    [SerializeField] protected TMP_Text itemName;
    [SerializeField] protected TMP_Text itemDesc;

    protected TradeViewModelBase viewModel;


    public virtual void Awake()
    {
        viewModel = GetComponent<TradeViewModelBase>();
    }

    public void Start()
    {
    }

    protected virtual void OnEnable()
    {
        if (viewModel == null || viewModel.Model == null) return;

        Debug.Log("이벤트 추가");
        viewModel.PropertyChanged -= OnViewModelPropChanged;

        viewModel.PropertyChanged += OnViewModelPropChanged;
        SetView();
    }

    protected virtual void OnDisable()
    {
        if (viewModel.Model != null)
        {
            Debug.Log("이벤트 제거");
            viewModel.PropertyChanged -= OnViewModelPropChanged;
        }
    }

    public virtual void SetView()
    {
        itemName.text = viewModel.Model.ItemName;
        itemDesc.text = viewModel.Model.ItemDesc;
    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.Log(this.name + " " + this.GetInstanceID() + " "+ e.PropertyName + " " + sender);
        switch (e.PropertyName)
        {
            case null:
            case "":
                SetView();
                break;
        }
    }
}
