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
            case 20025: return Resources.Load<Sprite>("Iron");
            case 20001: return Resources.Load<Sprite>("Silver");
            case 20002: return Resources.Load<Sprite>("Gold");
            case 20003: return Resources.Load<Sprite>("Silver");
            case 20004: return Resources.Load<Sprite>("Gold");
            case 20005: return Resources.Load<Sprite>("Silver");
            case 20006: return Resources.Load<Sprite>("Gold");
            case 20007: return Resources.Load<Sprite>("Silver");
            case 20008: return Resources.Load<Sprite>("Gold");
            case 20009: return Resources.Load<Sprite>("Gold");
            case 20010: return Resources.Load<Sprite>("Gold");
            case 20011: return Resources.Load<Sprite>("Gold");
            case 20012: return Resources.Load<Sprite>("Silver");
            case 20013: return Resources.Load<Sprite>("Gold");
            case 20014: return Resources.Load<Sprite>("Gold");
            case 20015: return Resources.Load<Sprite>("Gold");
          
            default: return Resources.Load<Sprite>("Iron");
        }
    }
    public static List<LakeInvenSlot> CreateInventory()
    {
        List<LakeInvenSlot> list = new()
        {
            new LakeInvenSlot { itemId = 20025, quantity = 90 },
            new LakeInvenSlot { itemId = 20001, quantity = 2 },
            new LakeInvenSlot { itemId = 20002, quantity = 2 },
            new LakeInvenSlot { itemId = 20003, quantity = 2 },
            new LakeInvenSlot { itemId = 20004, quantity = 2 },
            new LakeInvenSlot { itemId = 20005, quantity = 2 },
            new LakeInvenSlot { itemId = 20006, quantity = 2 },
            new LakeInvenSlot { itemId = 20007, quantity = 2 },
            new LakeInvenSlot { itemId = 20008, quantity = 2 },
            new LakeInvenSlot { itemId = 20009, quantity = 2 },
            new LakeInvenSlot { itemId = 20010, quantity = 2 },
         
        };
        return list;
    }
}
