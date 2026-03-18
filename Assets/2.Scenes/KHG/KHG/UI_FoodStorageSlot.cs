using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_FoodStorageSlot : MonoBehaviour
{
    UI_FoodStorage _parentUI;

    [SerializeField] Image _icon;
    [SerializeField] GameObject _emptyOverlay;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] GameObject _textBackground;

    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemGradeText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    int _boundRealIndex = -1;

    public void Init(UI_FoodStorage parent) // 이 UI 슬롯이 대표하는 실제 데이터 슬롯 인덱스
    {
        _parentUI = parent;
    }

    public void FoodRefresh(FoodStackSlot? data)  
    {
        if (!data.HasValue) // 빈 슬롯 처리
        {
            _icon.enabled = false;
            _icon.sprite = null;

            if (_itemNameText != null) _itemNameText.gameObject.SetActive(false);
            if (_itemGradeText != null) _itemGradeText.gameObject.SetActive(false);

            if (_emptyOverlay != null) _emptyOverlay.SetActive(true);
            if (_countText != null) _countText.gameObject.SetActive(false);
            return;
        }

        // 값 있는 슬롯
        if (_emptyOverlay != null) _emptyOverlay.SetActive(false);

        int id = data.Value.FoodId;
        int count = data.Value.Count;

        // 수량표시
        if (_countText != null)
        {
            _countText.gameObject.SetActive(true);
            _countText.text = $"x{count}";
        }

        var def = DataManager.Instance.FoodDatabase.FoodInfoData[id];

        if (def == null)
        {
            if (_emptyOverlay != null) _emptyOverlay.SetActive(true); // 데이터는 있는데 정의가 없으면 빈 슬롯처럼 처리

            if (_icon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.enabled = false;
                _icon.sprite = null;

                if (_itemNameText != null) _itemNameText.gameObject.SetActive(true);
                if (_itemGradeText != null) _itemGradeText.gameObject.SetActive(true);
                // if (itemPriceText != null) itemPriceText.gameObject.SetActive(true);
            }

            if (_countText != null) _countText.gameObject.SetActive(false);
            return;
        }
        Sprite sp = null;
        if (def.FoodImgPath_Sprite != null)
        {
            sp = def.FoodImgPath_Sprite;
        }

        _icon.enabled = (sp != null);
        _icon.sprite = sp;

        if (_itemNameText != null)
            _itemNameText.text = def.FoodName_String;

        if (_itemGradeText != null)
            _itemGradeText.text = def.foodrateType.ToString();
    }
    public void SetEmpty()
    {
        if (_icon != null)
        {
            _icon.sprite = null;
            _icon.enabled = false;
        }

        if (_countText != null)
            _countText.gameObject.SetActive(false);

        if (_itemNameText != null)
            _itemNameText.gameObject.SetActive(false);

        if (_itemGradeText != null)
            _itemGradeText.gameObject.SetActive(false);

        if (_itemPriceText != null)
            _itemPriceText.gameObject.SetActive(false);

        if (_textBackground != null)
            _textBackground.gameObject.SetActive(false);
    }

    public void BindRealIndex(int realIndex)
    {
        _boundRealIndex = realIndex;
    }
    public void OnClick()  // 버튼 OnClick에 연결될 함수
    {
        Debug.Log($"[UI슬롯 클릭됨] boundRealIndex={_boundRealIndex} name={gameObject.name}");

        if (_boundRealIndex < 0) return;

        _parentUI.OnSlotClicked(_boundRealIndex);
    }
}
