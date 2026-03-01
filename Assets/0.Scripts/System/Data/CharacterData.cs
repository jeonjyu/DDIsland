using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [field: SerializeField] public CharacterDatabaseSO CharacterInfoData { get; private set; }
    [field: SerializeField] public CharacterUpgradeDatabaseSO CharacterUpgradeData { get; private set; }
    [field: SerializeField] public CharacterVisualDatabaseSO CharacterVisualData { get; private set; }
}
