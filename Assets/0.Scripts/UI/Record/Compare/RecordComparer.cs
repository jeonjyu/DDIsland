using System.Collections.Generic;

// 제목 순으로 정렬
public class TitleComparer : IComparer<RecordDataSO>
{
    // 1순위 → 제목 / 2순위 → 음반 ID
    public int Compare(RecordDataSO x, RecordDataSO y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        int result = string.Compare(x.RecordName_String, y.RecordName_String);

        if (result == 0)
            result = x.RecordID.CompareTo(y.RecordID);

        return result;
    }
}

// 아티스트 순으로 정렬
public class ArtistComparer : IComparer<RecordDataSO>
{
    // 1순위 → 아티스트 / 2순위 → 음반 ID
    public int Compare(RecordDataSO x, RecordDataSO y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        int result = string.Compare(x.RecordArtist_String, y.RecordArtist_String);

        if (result == 0)
            result = x.RecordID.CompareTo(y.RecordID);

        return result;
    }
}