using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class EquipTradeViewModel : TradeViewModelBase
{
    public bool IsEquipped => PlayerManager.Instance.CompareID(Model);

    protected override void OnEnable()
    {
        base.OnEnable();
        StoreManager.Instance.PropertyChanged += OnGainedChanged;
        PlayerManager.Instance.OnEquipChanged += OnEquipStateChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StoreManager.Instance.PropertyChanged -= OnGainedChanged;
        PlayerManager.Instance.OnEquipChanged -= OnEquipStateChanged;
    }

    private void OnGainedChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StoreManager.Instance.IsTradeItemGained))
            OnPropertyChanged(nameof(IsGained));
    }

    private void OnEquipStateChanged(Enum filter, int itemObjectId)
    {
        OnPropertyChanged(nameof(IsEquipped));
        OnPropertyChanged(nameof(IsGained));
    }
}
