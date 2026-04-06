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
    public List<int> DefaultRecords { get; set; } = new List<int>();
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
            if (recordServerData.CurrentRecordData.CurrentPlaylistId == 0)
                return DefaultRecords;
            else
                return recordServerData.Playlists[recordServerData.Playlists.FindIndex(x => x.Id == recordServerData.CurrentRecordData.CurrentPlaylistId)].RecordLists;
        }
    }

    public int CurrentPlaylistId
    {
        get { return recordServerData.CurrentRecordData.CurrentPlaylistId; }
        set { recordServerData.CurrentRecordData.CurrentPlaylistId = value; }
    }

    // 플레이리스트 목록
    public List<PlaylistData> PlaylistDatas
    {
        get { return recordServerData.Playlists; }
        set { recordServerData.Playlists = value; }
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
}
