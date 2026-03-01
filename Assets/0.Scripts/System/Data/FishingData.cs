using UnityEngine;

public class FishingData : MonoBehaviour
{
    [field: SerializeField] public FishDatabaseSO FishData { get; private set; }
    [field: SerializeField] public FishingDropDatabaseSO FishingDropData { get; private set; }
}
