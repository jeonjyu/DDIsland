using System;
using System.Collections.Generic;

/// <summary>
/// 서버로 관리할 음반 데이터
/// </summary>
[Serializable]
public class RecordServerData
{
    // 보유 음반 조각
    public int LpPieceCount;

    // 해금된 음반 목록
    public HashSet<int> UnlockRecords = new HashSet<int>();

    // 즐겨찾기 음반 목록
    public HashSet<int> BookmarkList = new HashSet<int>();

    // todo: 플레이리스트 목록 정보 추가

    public CurrentRecordData CurrentRecordData = new CurrentRecordData();
}

/// <summary>
/// 현재 재생 음반에 관련된 정보
/// </summary>
[Serializable]
public class CurrentRecordData
{
    // 마지막으로 재생했던 음반 id
    public int RecordId;

    // 마지막으로 재생했던 현재 재생 목록
    public List<int> CurrentPlayList = new List<int>();

    // todo: 플레이 모드 정보 추가

    // 마지막으로 재생했던 음반의 재생 시점
    public float PlaybackPoint;
}