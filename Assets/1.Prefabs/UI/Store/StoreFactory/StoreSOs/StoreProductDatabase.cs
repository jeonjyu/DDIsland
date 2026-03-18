using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StoresSO", menuName = "Scriptable Objects/StoreFactory/StoresSO")]
public class StoreProductDatabase : ScriptableObject
{
    [field :SerializeField] public List<StoreProduct> storeProducts { get; private set; }
}
