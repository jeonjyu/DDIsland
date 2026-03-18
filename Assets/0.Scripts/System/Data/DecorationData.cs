using UnityEngine;

public class DecorationData : MonoBehaviour
{
    [field: SerializeField] public InteriorDatabaseSO InteriorData { get; private set; }
    [field: SerializeField] public CostumeDatabaseSO CostumeData { get; private set; }
}
