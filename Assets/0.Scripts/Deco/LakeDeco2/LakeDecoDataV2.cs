using UnityEngine;
using System.Collections.Generic;

/// 호수 배경 전용 데이터
public static class LakeDecoDataV2
{
    // InteriorDatabaseSO에서 호수용 스프라이트 가져오기
    public static Dictionary<int, Sprite> GetSpriteMap(InteriorDatabaseSO database)
    {
        var map = new Dictionary<int, Sprite>();
        if (database == null || database.datas == null) return map;

        foreach (var data in database.datas)
        {
            if (data == null) continue;
            if (data.locationType != LocationType.Lake) continue;
            if (data.InteriorPath_Sprite == null) continue;
            map[data.InteriorID] = data.InteriorPath_Sprite;
        }
        return map;
    }

    // SO 기반 슬롯별 ID 목록
    // 바닥재 (LakeLayerType.Terrain)
    public static List<int> GetSlot0Ids(InteriorDatabaseSO database)
    {
        var ids = new List<int>();
        if (database == null || database.datas == null) return ids;

        foreach (var data in database.datas)
        {
            if (data == null) continue;
            if (data.locationType != LocationType.Lake) continue;
            if (data.lakelayerType == LakeLayerType.Terrain)
                ids.Add(data.InteriorID);
        }
        return ids;
    }

    // 장식물 (LakeLayerType.Decorations)
    public static List<int> GetSlot1Ids(InteriorDatabaseSO database)
    {
        var ids = new List<int>();
        if (database == null || database.datas == null) return ids;

        foreach (var data in database.datas)
        {
            if (data == null) continue;
            if (data.locationType != LocationType.Lake) continue;
            if (data.lakelayerType == LakeLayerType.Decorations)
                ids.Add(data.InteriorID);
        }
        return ids;
    }
}