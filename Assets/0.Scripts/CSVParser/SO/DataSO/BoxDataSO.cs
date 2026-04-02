using System;
using UnityEngine;

// 보관타입
[Serializable]
public enum StorageType
{
    None = 0,
}


[CreateAssetMenu(fileName = "BoxDataSO", menuName = "Scriptable Objects/Data/BoxDataSO")]
public class BoxDataSO : TableBase<int>
{
    // 박스ID
    [field: SerializeField] public int BoxID { get; private set; }

    // 박스 레벨
    [field: SerializeField] public int BoxLevel { get; private set; }

    // 보관타입
    [field: SerializeField] public StorageType storageType { get; private set; }

    // 박스 슬롯
    [field: SerializeField] public int SlotCount { get; private set; }

    // 업그레이드 비용
    [field: SerializeField] public int ExpansionCost { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => BoxID;
}
