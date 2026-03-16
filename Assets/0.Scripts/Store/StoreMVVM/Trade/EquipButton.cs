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

    public bool IsEquipped => _isEquipped;

    private void Awake()
    {
        _equipButton = GetComponent<Button>();
        _text = _equipButton.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.OnEquipChanged += (isEquipped, isGained) => ChangeBtnText(isEquipped, isGained);

        _equipButton.onClick.AddListener(ChangeEquipStatus);
    }

    private void OnDisable()
    {
        _equipButton.onClick.RemoveAllListeners();
    }

    public void ChangeBtnText(bool isEquipped, bool isGained)
    {
        IStoreItem item = StoreManager.Instance.TradeModel;
        Debug.Log("ChangeBtnText 호출");
        if (_isEquipped && isGained)
        {
            // 현재 장착중인 아이템과 현재 아이템이 동일하면 해제
            if (PlayerManager.Instance.CompareID(item))
            {
                Debug.Log("장착중인 아이템");
                _text.text = "해제";
                //Debug.Log("ChangeBtnText 해제 " + _isEquipped + isGained);
            }
            else
            {
                Debug.Log("장착중이 아닌 아이템");
                _text.text = "장착";
                //Debug.Log("ChangeBtnText 장착 " + _isEquipped + isGained);
            }
        }
        else
        {
            Debug.Log("장착중이 아니거나 보유하지 않은 아이템");
            _text.text = "장착"; 
            //Debug.Log("ChangeBtnText 장착 " + _isEquipped + isGained);
        }
    }

    public void SetBtnAvailability(bool isGained)
    {
        _equipButton.interactable = isGained ? true : false;
    }

    /// <summary>
    /// 장착 버튼 클릭하면 PlayerManager에서 장착한 아이템 아이디 변경
    /// </summary>
    public void ChangeEquipStatus()
    {
        IStoreItem item = StoreManager.Instance.TradeModel;
        if (!_isEquipped && item.IsGained) // 장착
        {
            PlayerManager.Instance.SetEquip(item);
            _isEquipped = true;
            PlayerManager.Instance.OnEquipChanged?.Invoke(true, true);
        }
        else // 해제
        {
            PlayerManager.Instance.SetEquip(item);
            _isEquipped = false;
            PlayerManager.Instance.OnEquipChanged?.Invoke(false, true);
        }
    }
}
