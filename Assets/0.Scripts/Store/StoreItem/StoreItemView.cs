using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreItemView : MonoBehaviour, IStoreItemView
{
    // 인테리어 타입에 따라 다른 UI 테두리 표시
    [SerializeField] private Image _slotBackground;
    [SerializeField] private TMP_Text _itemPrice;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemCount;
    [SerializeField] private Image _itemImage;

    EventTrigger eventTrigger;

    public Image SlotBackground => _slotBackground;
    public TMP_Text ItemPrice => _itemPrice;
    public TMP_Text ItemName => _itemName;
    public TMP_Text ItemCount => _itemCount;
    public Image ItemImage => _itemImage;


    void Start()
    {
        eventTrigger = gameObject.GetComponent<EventTrigger>();
        //eventTrigger.OnPointerClick(() => { }); 
        // 버튼 팝업 띄우고
        // 해당 뷰의 모델을 전달하는 메서드 
    }

    public void Init(int price, string name, int count, bool isGained, string imgPath)
    {
        UpdateSlotColor(isGained);
        _itemName.text = name;
        _itemPrice.text = price.ToString();
        _itemCount.text = count.ToString();
        //_itemImage.sprite = ;
    }

    public void UpdateItemCount(int count)
    {
        _itemCount.text = count.ToString();
    }

    public void UpdateSlotColor(bool isGained)
    {
        if (!isGained)
            _slotBackground.color = Color.grey;
        else
            _slotBackground.color = Color.white;
    }
}
