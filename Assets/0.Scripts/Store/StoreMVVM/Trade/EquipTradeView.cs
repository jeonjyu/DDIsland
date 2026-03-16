using System.ComponentModel;
using UnityEngine;

public class EquipTradeView : MonoBehaviour
{
    [SerializeField] EquipButton _equipBtn;
    protected TradeViewModelBase viewModel;

    public EquipButton EquipButton => _equipBtn;
    public void Awake()
    {
        viewModel = GetComponent<TradeViewModelBase>();
        StoreManager.Instance.PropertyChanged += UpdateTradeModel;
    }

    // 장착 버튼 업데이트
    public void UpdateEquipBtn()
    {
        if (viewModel.Model.IsGained)
        {
            _equipBtn.SetBtnAvailability(true);
            // 장착중인 아이템의 타입과 비교
            if (PlayerManager.Instance.CompareID(viewModel.Model))
            {
                // 장착중
                Debug.Log("이거 장착중");
                _equipBtn.ChangeBtnText(true, viewModel.Model.IsGained);
            }
            else
            {
                // 다른 아이템 장착중일 때
                Debug.Log("다른거 장착중");
                _equipBtn.ChangeBtnText(false, viewModel.Model.IsGained);
            }
        }
        else
        {
            Debug.Log("장착 안함");
            _equipBtn.SetBtnAvailability(false);
        }
    }


    // todo : 보유 여부, 장착 여부 따라 변경되도록 구현하기
    // viewModel에서 
    public void UpdateTradeModel(object sender, PropertyChangedEventArgs e)
    {
        Debug.Log("장착/해제 여부 변경되니? " + e.PropertyName + " " + nameof(StoreManager.Instance.IsTradeItemGained));
        if (e.PropertyName == nameof(StoreManager.Instance.IsTradeItemGained))
        {
            Debug.Log("장착/해제 여부 변경되는듯");
            UpdateEquipBtn();
        }
    }
}
