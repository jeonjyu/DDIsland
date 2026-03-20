using System;
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

    protected virtual void Awake()
    {
        _equipButton = GetComponent<Button>();
        _text = _equipButton.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.OnEquipChanged += EquipStatHander;
        _equipButton.onClick.AddListener(ChangeEquipStatus);
        ChangeBtnUI(StoreManager.Instance.IsTradeItemGained);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnEquipChanged -= EquipStatHander;
        _equipButton.onClick.RemoveAllListeners();
    }

    // 아이템 필터랑 아이템 아이디 받아오면 
    private void EquipStatHander(Enum filter, int itemObjectId)
    {
        ChangeBtnUI(true); // 항상 true로 들어옴
    }

    public void ChangeBtnUI(bool isGained)
    {
        IStoreItem item = StoreManager.Instance.TradeModel;
        if (item == null)
        {
            Debug.Log("item 없음");

            return;
        }

        if (isGained)
        {
            _text.text = PlayerManager.Instance.CompareID(item) ? "해제": "장착" ;
            _equipButton.interactable = true;
            Debug.Log("isGained 변수 : " + isGained + " 현재 거래 모델 보유 여부 : " + StoreManager.Instance.TradeModel.IsGained + " 텍스트 : " + _text.text);
        }
        else
        {
            _equipButton.interactable = false;
            _text.text = "장착";
            Debug.Log("isGained 변수 : " + isGained + " 현재 거래 모델 보유 여부 : " + StoreManager.Instance.TradeModel.IsGained + " 텍스트 : " + _text.text);
        }
    }

    /// <summary>
    /// 장착 버튼 클릭하면 PlayerManager에서 장착한 아이템 아이디 변경
    /// </summary>
    public void ChangeEquipStatus()
    {
        //Debug.Log(this + " ChangeEquipStatus");
        IStoreItem item = StoreManager.Instance.TradeModel;
        if (item == null || item.IsGained == false) return;

        PlayerManager.Instance.SetEquip(item);
    }
}
