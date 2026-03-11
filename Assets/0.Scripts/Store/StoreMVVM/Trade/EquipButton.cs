using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipButton : MonoBehaviour
{
    Button _equipButton;
    TMP_Text _text;
    bool _isEquipped = false;

    public Button EquipBtn => _equipButton;
    public TMP_Text EquipText => _text;
    private void Awake()
    {
        _equipButton = GetComponent<Button>();
        _text = GetComponent<TMP_Text>();
        //_text.text = "장착";
    }

    public void ChangeBtnText(bool isEquipped)
    {
        _text.text = isEquipped ? "해제" : "장착";
        _isEquipped = isEquipped;
    }

    public void SetBtnAvailability(bool isGained)
    {
        _equipButton.interactable = isGained ? true : false;
    }

    // 장착 버튼 클릭하면 변경하는 메서드
    public void OnPointerClick(PointerEventData eventData)
    {
        IStoreItem item = StoreManager.Instance.TradeModel;
        if (!_isEquipped) // 장착
        {
            PlayerManager.Instance.SetCostume(item.ID, (CostumeType)item.Filter);
            ChangeBtnText(true);
        }
        else // 해제
        {
            PlayerManager.Instance.SetCostume(0, (CostumeType)item.Filter);
            ChangeBtnText(false);
        }
    }
}
