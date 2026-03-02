using UnityEngine;

public class StoreData : MonoBehaviour
{
    [field: SerializeField] public FishingStoreDatabaseSO FishingStoreData { get; private set; }
    [field: SerializeField] public CostumeStoreDatabaseSO CostumeStoreData { get; private set; }
    [field: SerializeField] public InteriorStoreDatabaseSO InteriorStoreData { get; private set; }

    // todo: 인테리어, 코스튬 상점 데이터 추가 예정
}
