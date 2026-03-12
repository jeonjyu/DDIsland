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
        _text = _equipButton.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        _equipButton.onClick.AddListener(ChangEquipState);
    }

    private void OnDisable()
    {
        _equipButton.onClick.RemoveAllListeners();
    }

    public void ChangeBtnText(bool isEquipped, bool isGained)
    {
        IStoreItem item = StoreManager.Instance.TradeModel;

        if (_isEquipped && isGained)
        {
            // 현재 장착중인 아이템과 현재 아이템이 동일하면 해제
            if (PlayerManager.Instance.CompareID(item.ObjectId, (CostumeType)item.Filter))
            {
                _text.text = "해제";
            }
            else
            {
                _text.text = "장착";
            }

            if (_isEquipped != isEquipped) _isEquipped = isEquipped;
        }
        else
        {
            _text.text = "장착";
            if (_isEquipped != isEquipped) _isEquipped = isEquipped;
        }
    }

    public void SetBtnAvailability(bool isGained)
    {
        _equipButton.interactable = isGained ? true : false;
    }

    /// <summary>
    /// 장착 버튼 클릭하면 PlayerManager에서 장착한 아이템 아이디 변경
    /// </summary>
    public void ChangEquipState()
    {
        IStoreItem item = StoreManager.Instance.TradeModel;
        if (!_isEquipped) // 장착
        {
            PlayerManager.Instance.SetCostume(item.ObjectId, (CostumeType)item.Filter);
            ChangeBtnText(true, true);
        }
        else // 해제
        {
            PlayerManager.Instance.SetCostume(0, (CostumeType)item.Filter);
            ChangeBtnText(false, true);
        }
    }
}
