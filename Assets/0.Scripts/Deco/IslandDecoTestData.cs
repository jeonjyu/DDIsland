using UnityEngine;
using System.Collections.Generic;

/// 섬 꾸미기 전용 테스트 데이터
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
public static class IslandDecoTestData
{
    //아직 스프라이트 없어서 기본 로직 유지 중
    public static Sprite GetIconSprite(int itemId)
    {
        // TODO: 나중에 실제 스프라이트 로드
        switch (itemId)
        {
            case 20001: return Resources.Load<Sprite>("Iron");
            case 20002: return Resources.Load<Sprite>("Silver");
            case 20025: return Resources.Load<Sprite>("Gold");
            default: return Resources.Load<Sprite>("Iron");
        }
    }
    public static List<LakeInvenSlot> CreateInventory()
    {
        List<LakeInvenSlot> list = new()
        {
            new LakeInvenSlot { itemId = 20001, quantity = 1 },
            new LakeInvenSlot { itemId = 20002, quantity = 2 },
            new LakeInvenSlot { itemId = 20025, quantity = 3 }
        };
        return list;
    }
}
