using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "StoreProduct", menuName = "Scriptable Objects/StoreFactory/StoreProduct")]
public partial class StoreProduct : ScriptableObject
{
    [SerializeField] public StoreCat cat;
    [SerializeField] public GridLayoutGroup gridcontent;
    [SerializeField] public LayoutValue gridLayout;
    [SerializeField] public ItemSlotViewModelBase itemSlot;
    [SerializeField] public GameObject TradePopup;

    [ContextMenu("레이아웃 채우기")]
    public void CopySO()
    {
        if (gridcontent != null) gridLayout = new LayoutValue(gridcontent);
    }
}
