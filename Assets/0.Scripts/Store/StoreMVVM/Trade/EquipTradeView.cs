using UnityEngine;

public class EquipTradeView : MonoBehaviour
{
    [SerializeField] EquipButton _equipBtn;
    protected TradeViewModelBase viewModel;

    public EquipButton EquipButton => _equipBtn;
    public void Awake()
    {
        viewModel = GetComponent<TradeViewModelBase>();
    }

    // 장착 버튼 업데이트
    // 보유 
    public void UpdateEquipBtn()
    {
        if (viewModel.Model.IsGained)
        {
            _equipBtn.SetBtnAvailability(true);
            // 장착중인 아이템의 타입과 비교
            if (PlayerManager.Instance.CompareID(viewModel.Model.ID, (CostumeType)viewModel.Model.Filter))
            {
                // 장착중
                _equipBtn.ChangeBtnText(true);
            }
            else
            {
                // 다른 아이템 장착중일 때
                _equipBtn.ChangeBtnText(false);
            }
        }
        else
        {
            _equipBtn.SetBtnAvailability(false);
        }

    }

}
