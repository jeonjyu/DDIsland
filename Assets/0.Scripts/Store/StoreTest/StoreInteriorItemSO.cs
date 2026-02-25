using UnityEngine;

[CreateAssetMenu(fileName = "StoreInteriorItemSO", menuName = "Scriptable Objects/StoreInteriorItemSO")]
public class StoreInteriorItemSO : StoreItemBaseSO<InteriorFilter> 
{
    public StoreCat store => StoreCat.interior;
}
