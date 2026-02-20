using UnityEngine;

[CreateAssetMenu(fileName = "StoreCostumeItemSO", menuName = "Scriptable Objects/StoreCostumeItemSO")]
public class StoreCostumeItemSO : StoreItemBaseSO<CostumeFilter>
{
    public StoreCat store => StoreCat.costume;
}
