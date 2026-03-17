using System.Collections.Generic;
using UnityEngine;

public class UI_RecordList<T> : MonoBehaviour where T : UI_RecordSlot
{
    [Header("각 카테고리 패널에 추가할 슬롯 프리팹")]
    [SerializeField] private T recordSlot;

    [Header("슬롯을 추가할 트랜스폼")]
    [SerializeField] private Transform slotContent;

    [Header("현재 리스트가 관리할 오디오 타입")]
    [SerializeField] private RecordType recordType;

    public List<T> recordSlotList = new List<T>();

    private void Start()
    {
        foreach(var record in DataManager.Instance.RecordDatabase.RecordInfoData.datas)
        {
            // 현재 리스트에서 취급할 음반이라면
            if(record.recordType == recordType)
            {
                T slot = Instantiate(recordSlot, slotContent);
                slot.InitData(record, this);
            }
        }
    }
}
