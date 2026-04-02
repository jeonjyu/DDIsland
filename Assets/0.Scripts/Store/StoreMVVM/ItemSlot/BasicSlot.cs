using TMPro;
using UnityEngine;

public class BasicSlot : ItemSlotView
{
    [SerializeField] protected TMP_Text _itemCount;
    public TMP_Text ItemCount => _itemCount;

    public override void Init()
    {
        base.Init();
        if (modelData is null)
        {
            //Debug.LogWarning("모델이 없음");
            return;
        }
        _itemCount.text = modelData.ItemCount.ToString();
    }

    public override void ResetSlot()
    {
        base.ResetSlot();
        _itemCount.text = "0";
    }
    public void UpdateItemCount(int count)
    {
        _itemCount.text = viewModel.Model.ItemCount.ToString();
    }

    public override void UpdateSlotUI(int count)
    {
        base.UpdateSlotUI(count);
        UpdateItemCount(count);
        UpdateSlotColor(modelData.IsGained);

        //Debug.Log(this + " UI 갱신");

    }
}
