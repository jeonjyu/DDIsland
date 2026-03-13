using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [field: SerializeField] public CharacterData CharacterDatabase { get; private set; }
    [field: SerializeField] public CurrencyData CurrencyDatabase { get; private set; }
    [field: SerializeField] public FishingData FishingDatabase { get; private set; }
    [field: SerializeField] public FoodData FoodDatabase { get; private set; }
    [field: SerializeField] public StoreData StoreDatabase { get; private set; }
    [field: SerializeField] public DecorationData DecorationDatabase { get; private set; }
    [field: SerializeField] public QuestData QuestDatabase { get; private set; }
    [field: SerializeField] public JournalData JournalDatabase { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }
}
