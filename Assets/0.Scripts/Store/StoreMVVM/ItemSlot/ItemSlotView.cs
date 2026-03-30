using TMPro;
using UnityEngine;

// 레시피, 낚시, 코스튬, 기본 아이템 슬롯
public class ItemSlotView : ItemSlotViewBase<ItemSlotViewModelBase>
{
    [SerializeField] protected TMP_Text _itemPrice;
    [SerializeField] protected TMP_Text _itemName;

    public TMP_Text ItemPrice => _itemPrice;
    public TMP_Text ItemName => _itemName;

    public override void Init()
    {
        base.Init();
        if (modelData is null)
        {
            //Debug.LogWarning("모델이 없음");
            return;
        }

        _itemName.text = modelData.ItemName;
        _itemPrice.text = modelData.PurchasePrice.ToString();
    }

    public override void ResetSlot()
    {
        base.ResetSlot();
        _itemName.text = "";
        _itemPrice.text = "";
    }


}
