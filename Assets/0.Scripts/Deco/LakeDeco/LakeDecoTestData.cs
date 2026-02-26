using UnityEngine;
using System.Collections.Generic;

/// 호수 꾸미기 전용 테스트 데이터, 섬은 데이터 따로 만들어줘야 함 
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
public static class LakeDecoTestData
{
    public static string GetItemName(int itemId)
    {
        switch (itemId)
        {
            case 1001: return "작은 수초";
            case 1002: return "큰 수초";
            case 1003: return "조약돌";
            case 1004: return "산호 조각";
            case 1005: return "작은 바위";
            case 1006: return "보물 상자";
            case 1007: return "해초 군락";
            case 1008: return "모래 언덕";
            case 1009: return "조개껍데기";
            case 1010: return "물풀";
            default: return "호수아이템 " + itemId;
        }
    }

    public static Sprite GetIconSprite(int itemId)
    {
        // TODO: 나중에 실제 스프라이트 로드
        // return Resources.Load<Sprite>("Icons/Lake/" + itemId);
        switch (itemId) //랜덤 
        {
            case 1001: return Resources.Load<Sprite>("Leaf");
            case 1002: return Resources.Load<Sprite>("Leaf");
            case 1003: return Resources.Load<Sprite>("Bone");
            case 1004: return Resources.Load<Sprite>("Gold");
            case 1005: return Resources.Load<Sprite>("Iron");
            case 1006: return Resources.Load<Sprite>("Gold");
            case 1007: return Resources.Load<Sprite>("Whool-Indigo");
            case 1008: return Resources.Load<Sprite>("Silver");
            case 1009: return Resources.Load<Sprite>("Bone");
            case 1010: return Resources.Load<Sprite>("Wood");
            default: return Resources.Load<Sprite>("Iron");
        }
        //return Resources.Load<Sprite>("Wood");
    }

    public static List<LakeInvenSlot> CreateTestInventory()
    {
        List<LakeInvenSlot> list = new List<LakeInvenSlot>();
        list.Add(new LakeInvenSlot { itemId = 1001, quantity = 3 });
        list.Add(new LakeInvenSlot { itemId = 1002, quantity = 2 });
        list.Add(new LakeInvenSlot { itemId = 1003, quantity = 5 });
        list.Add(new LakeInvenSlot { itemId = 1004, quantity = 1 });
        list.Add(new LakeInvenSlot { itemId = 1005, quantity = 4 });
        list.Add(new LakeInvenSlot { itemId = 1006, quantity = 1 });
        list.Add(new LakeInvenSlot { itemId = 1007, quantity = 2 });
        list.Add(new LakeInvenSlot { itemId = 1008, quantity = 3 });
        list.Add(new LakeInvenSlot { itemId = 1009, quantity = 6 });
        list.Add(new LakeInvenSlot { itemId = 1010, quantity = 2 });
        return list;
    }
}