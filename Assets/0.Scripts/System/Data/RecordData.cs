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
    private RecordLocalData recordLocalData = new RecordLocalData();

    public RecordDataSO CurrentRecord;

    public event Action<int> OnLPPieceChanged;

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

    // 마지막으로 재생했던 음반 id
    public int CurrentRecordId
    {
        get { return recordLocalData.CurrentRecordData.RecordId; }
        set { recordLocalData.CurrentRecordData.RecordId = value; }
    }

    // 마지막으로 재생했던 현재 재생 목록
    public List<int> CurrentPlayList
    {
        get { return recordLocalData.CurrentRecordData.CurrentPlayList; }
        set { recordLocalData.CurrentRecordData.CurrentPlayList = value; }
    }

    // 마지막으로 재생했던 음반의 재생 시점
    public float PlaybackPoint
    {
        get { return recordLocalData.CurrentRecordData.PlaybackPoint; }
        set
        {
            float time = Mathf.Round(value * 100) / 100f;
            recordLocalData.CurrentRecordData.PlaybackPoint = time;
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

        recordServerData.LpPieceCount = data._lpPieceCount;

        recordServerData.UnlockRecords = new HashSet<int>(data._unlockRecords);
    }
}
