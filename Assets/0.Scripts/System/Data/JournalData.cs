using UnityEngine;

public class JournalData : MonoBehaviour
{
    [field: SerializeField] public JournalCostumeDatabaseSO JournalCostumeData { get; private set; }
    [field: SerializeField] public JournalInteriorDatabaseSO JournalInteriorData { get; private set; }
    [field: SerializeField] public JournalFoodDatabaseSO JournalFoodData { get; private set; }
    [field: SerializeField] public JournalFishDatabaseSO JournalFishData { get; private set; }
}
