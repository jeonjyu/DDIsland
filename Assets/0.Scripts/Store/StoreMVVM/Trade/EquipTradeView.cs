using System.ComponentModel;
using UnityEngine;

public class EquipTradeView : TradeViewBase
{
    [SerializeField] EquipButton _equipBtn;
    EquipTradeViewModel equipViewModel;
    public EquipButton EquipButton => _equipBtn;

    public override void Awake()
    {
        base.Awake();
        equipViewModel = GetComponent<EquipTradeViewModel>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StoreManager.Instance.PropertyChanged += UpdateTradeModel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StoreManager.Instance.PropertyChanged -= UpdateTradeModel;
    }



    // 장착 버튼 업데이트
    public void UpdateEquipBtn()
    {
        if(_equipBtn != null)
        {
            //Debug.Log("보유 여부 :" + equipViewModel.IsGained);
            _equipBtn.ChangeBtnUI(equipViewModel.IsGained);
        }
    }

    //public override void SetView()
    //{
    //    base.SetView();
    //    Debug.Log("SetView override");
    //}

    // todo : 보유 여부, 장착 여부 따라 변경되도록 구현하기
    // viewModel에서 
    public void UpdateTradeModel(object sender, PropertyChangedEventArgs e)
    {
        if(_equipBtn == null)
        {
            Debug.Log("장착 버튼이 없음");
            return;
        }
        //Debug.Log("장착/해제 여부 변경되니? " + e.PropertyName + " " + nameof(StoreManager.Instance.IsTradeItemGained));
        if(e.PropertyName == nameof(StoreManager.Instance.TradeItemCount))
        {
            
            UpdateEquipBtn();
        }
    }
}
