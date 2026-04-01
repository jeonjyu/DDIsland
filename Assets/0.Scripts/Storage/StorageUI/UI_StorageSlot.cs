using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class UI_StorageSlot : MonoBehaviour
{
    UI_Storage _parentUI;

    [SerializeField] Image _icon;
    [SerializeField] GameObject _emptyOverlay;

    // 슬롯 내부 표시용 텍스트
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemGradeText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;

    [SerializeField] private AudioClip _ButtonSFX;

    int _boundRealIndex = -1;  // 이 UI 슬롯이 대표하는 실제 데이터 슬롯 인덱스

    public void Init(UI_Storage parent) // 이 UI 슬롯이 대표하는 실제 데이터 슬롯 인덱스
    {
        _parentUI = parent;
    }

    public void Refresh(FishStackSlot? data)   // 실제 데이터(FishStackSlot?)를 받아서 슬롯 화면을 갱신
    {
        if (!data.HasValue) // 빈 슬롯 처리
        {
            _icon.enabled = false;
            _icon.sprite = null;

            if (_itemNameText != null) _itemNameText.gameObject.SetActive(false);
            if (_itemGradeText != null) _itemGradeText.gameObject.SetActive(false);
            // if (itemPriceText != null) itemPriceText.gameObject.SetActive(false);

            if (_emptyOverlay != null) _emptyOverlay.SetActive(true);
            return;
        }

        // 값 있는 슬롯
        if (_emptyOverlay != null) _emptyOverlay.SetActive(false);

        int id = data.Value.FishId;

        var def = DataManager.Instance.FishingDatabase.FishData[id];

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
            return;
        }
        Sprite sp = null;
        if (def.FishImgPath_Sprite != null)
        {
            sp = def.FishImgPath_Sprite;
        }

        _icon.enabled = (sp != null);
        _icon.sprite = sp;

        if (_itemNameText != null)
            _itemNameText.text = def.FishName_String;

        if (_itemGradeText != null)
            _itemGradeText.text = def.gradeType.ToString();

        //if (itemPriceText != null) itemPriceText.text = data.Value.maxPrice.ToString();
    }
    public void SetEmpty()
    {
        if (_icon != null)
        {
            _icon.sprite = null;
            _icon.enabled = false;
        }

        if (_itemNameText != null)
            _itemNameText.gameObject.SetActive(false);

        if (_itemGradeText != null)
            _itemGradeText.gameObject.SetActive(false);

        if (_itemPriceText != null)
            _itemPriceText.gameObject.SetActive(false);
    }


    public void BindRealIndex(int realIndex)
    {
        _boundRealIndex = realIndex;
    }

    public void OnClick()  // 버튼 OnClick에 연결될 함수
    {
        Debug.Log($"[UI슬롯 클릭됨] boundRealIndex={_boundRealIndex} name={gameObject.name}");

        if (_boundRealIndex < 0) return;
        SoundManager.Instance.PlaySFX(_ButtonSFX);
        _parentUI.OnSlotClicked(_boundRealIndex);
    }
}