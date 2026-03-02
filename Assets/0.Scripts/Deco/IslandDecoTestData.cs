using UnityEngine;
using System.Collections.Generic;

/// 섬 꾸미기 전용 테스트 데이터
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
public static class IslandDecoTestData
{

    private static Dictionary<int, InteriorDataSO> _data;

    public static void Initialize(InteriorDatabaseSO database)
    {
        if (database == null) return;

        _data = new Dictionary<int, InteriorDataSO>();

        foreach (var d in database.datas)
        {
            if (d != null && !_data.ContainsKey(d.InteriorID))
                _data.Add(d.InteriorID, d);
        }

        Debug.Log($"SO 연동: {_data.Count}개 로드됨");
    }
    /// 핵심: itemId에서 들고온 데이터 반환
    public static InteriorDataSO GetData(int itemId)
    {
        if (_data != null && _data.TryGetValue(itemId, out var so))
        {
            return so;
        }
        return null;
    }

    //아직 스프라이트 없어서 기본 로직 유지 중
    public static Sprite GetIconSprite(int itemId)
    {
        // TODO: 나중에 실제 스프라이트 로드
        switch (itemId)
        {
            case 2001: return Resources.Load<Sprite>("Iron");
            case 2002: return Resources.Load<Sprite>("Silver");
            case 2003: return Resources.Load<Sprite>("Gold");
            default: return Resources.Load<Sprite>("Iron");
        }
    }

    
   

    public static List<LakeInvenSlot> CreateInventory()
    {
        List<LakeInvenSlot> list = new()
        {
            new LakeInvenSlot { itemId = 2001, quantity = 3 },
            new LakeInvenSlot { itemId = 2002, quantity = 2 },
            new LakeInvenSlot { itemId = 2003, quantity = 1 }
        };
        return list;
    }
}
