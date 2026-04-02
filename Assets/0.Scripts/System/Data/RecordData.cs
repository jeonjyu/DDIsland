using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecordData : MonoBehaviour
{
    #region SO 데이터 파일
    [field: SerializeField] public RecordDatabaseSO RecordInfoData { get; private set; }
    #endregion

    private RecordServerData recordServerData = new RecordServerData();

    public RecordDataSO CurrentRecord;
    public List<int> DefaultRecords { get; private set; } = new List<int>();    // todo: 임시 기본 재생 목록
    public event Action<int> OnLPPieceChanged;
    //추가한 부분
    public event Action OnRecordsUpdated;

    private void OnEnable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += SyncRecordSaveData;
    }
    private void OnDisable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave -= SyncRecordSaveData;
    }

    private void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnDataLoaded += SyncRecordLoadData;
        }
    }

    #region 프로퍼티
    // 보유 음반 조각
    public int LpPieceCount
    {
        get { return recordServerData.LpPieceCount; }
        set
        {
            if (value < 0) return;

            recordServerData.LpPieceCount = value;
            OnLPPieceChanged?.Invoke(value);
        }
    }

    // 해금된 음반 목록
    public HashSet<int> UnlockRecords
    {
        get { return recordServerData.UnlockRecords; }
        set { recordServerData.UnlockRecords = value; }
    }

    public HashSet<int> BookmarkRecords
    {
        get { return recordServerData.BookmarkList; }
        set { recordServerData.BookmarkList = value; }
    }

    // 마지막으로 재생했던 음반 id
    public int CurrentRecordId
    {
        get { return recordServerData.CurrentRecordData.RecordId; }
        set { recordServerData.CurrentRecordData.RecordId = value; }
    }

    // 마지막으로 재생했던 현재 재생 목록
    public List<int> CurrentPlayList
    {
        get
        {
            if (recordServerData.CurrentRecordData.CurrentPlayList.Count == 0)
                return DefaultRecords;
            else
                return recordServerData.CurrentRecordData.CurrentPlayList;
        }
        set { recordServerData.CurrentRecordData.CurrentPlayList = value; }
    }

    // 마지막으로 재생했던 음반의 재생 시점
    public float PlaybackPoint
    {
        get { return recordServerData.CurrentRecordData.PlaybackPoint; }
        set
        {
            float time = Mathf.Round(value * 100) / 100f;
            recordServerData.CurrentRecordData.PlaybackPoint = time;
        }
    }
    #endregion
    private void SyncRecordSaveData()
    {
        var data = DataManager.Instance.Box.Record;

        data._lpPieceCount = recordServerData.LpPieceCount;
        data._unlockRecords = recordServerData.UnlockRecords.ToList();
    }

    private void SyncRecordLoadData()
    {
        var data = DataManager.Instance.Box.Record;

        LpPieceCount = data._lpPieceCount;

        UnlockRecords = new HashSet<int>(data._unlockRecords ?? new List<int>());

        //추가한 부분
        OnRecordsUpdated?.Invoke();
    }

    private void Awake()
    {
        foreach (RecordDataSO record in RecordInfoData.datas)
        {
            if (record.recordType == RecordType.Background && record.IsDefaultRecord)
                DefaultRecords.Add(record.RecordID);
        }
    }
}
