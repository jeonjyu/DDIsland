using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // 추가한 부분
    [SerializeField] private DataHub _hub;
    public DataHub Hub => _hub;
    public UserAllData Box => _hub._allUserData;


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

    // 모든 세이브와 로드를 관리
    // 데이터 박스에 저장을 시키고
    public void SaveAll() => _hub.SaveAllData();
    //얘를 통해서 Json으로 변환 후 서버로 전송
    public void UploadALl() => _hub.UploadAllData();
}
