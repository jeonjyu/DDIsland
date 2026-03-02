using UnityEngine;

public class DecorationData : MonoBehaviour
{
    [field: SerializeField] public InteriorDatabaseSO InteriorData { get; private set; }
    [field: SerializeField] public CostumeDatabaseSO CostumeData { get; private set; }

    // todo: 코스튬, 데코 관련 필드 정의할 예정
}
