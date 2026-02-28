using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;
/*
 * 주말 팀원들을 위한 설명서
 * UI_StorageSlot (슬롯 UI 1칸)
 *
 * 역할
 * - 하나의 UI 슬롯이 어떤 realIndex(실제 데이터 슬롯)를 대표하는지 기억한다.
 * - Refresh(data)로 아이콘/수량/이름/등급/가격을 그린다.
 * - 클릭하면 parentUI(UI_Storage)에 realIndex를 알려준다.
 *
 * 주의
 * - uiIndex(_slotIndex) != realIndex(_boundRealIndex)
 *   정렬 때문에 화면 순서와 실제 데이터 위치가 달라지므로 반드시 realIndex를 넘겨야 함.
 */
public class UI_StorageSlot : MonoBehaviour
{
    UI_Storage _parentUI;

    [SerializeField] Image _icon;
    [SerializeField] GameObject _emptyOverlay;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] FishAtlasProvider _provider;

    // 슬롯 내부 표시용 텍스트
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemGradeText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;

    int _boundRealIndex = -1;  // 이 UI 슬롯이 대표하는 실제 데이터 슬롯 인덱스
    int _slotIndex;

    public void Init(UI_Storage parent, int index) // 이 UI 슬롯이 대표하는 실제 데이터 슬롯 인덱스
    {
        _parentUI = parent;
        _slotIndex = index;
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
            if (_countText != null) _countText.gameObject.SetActive(false);
            return;
        }

        // 값 있는 슬롯
        if (_emptyOverlay != null) _emptyOverlay.SetActive(false);

        int id = data.Value.FishId;
        int count = data.Value.Count;

        // 수량표시
        if (_countText != null)
        {
            _countText.gameObject.SetActive(true);
            _countText.text = $"x{count}";
        }

        FishDefinition def = FishManager.Instance.GetDefinition(id);   // 정의 데이터 조회(이름/등급/스프라이트 키 등)

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
        if (_provider != null)
            sp = _provider.GetFishSprite(def);

        _icon.enabled = (sp != null);
        _icon.sprite = sp;

        if (_itemNameText != null)
            _itemNameText.text = def.FishName_String;

        if (_itemGradeText != null)
            _itemGradeText.text = def.Grade.ToString();

        //if (itemPriceText != null) itemPriceText.text = data.Value.maxPrice.ToString();
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