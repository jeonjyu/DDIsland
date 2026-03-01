using UnityEngine;
using System.Collections.Generic;

/// 섬 꾸미기 전용 테스트 데이터
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
public static class IslandDecoTestData
{
    public static string GetItemName(int itemId)
    {
        switch (itemId)
        {
            case 2001: return "큐브 건물";
            case 2002: return "구체 건물";
            case 2003: return "캡슐 건물";
            default: return "섬아이템 " + itemId;
        }
    }

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

    /// 핵심: itemId에 3D프리팹 매핑
    /// Resources 폴더에 프리팹이 있어야 함
    public static GameObject GetPrefab(int itemId)
    {
        switch (itemId)
        {
            case 2001: return Resources.Load<GameObject>("TestBuild/TestBuild Copy");
            case 2002: return Resources.Load<GameObject>("TestBuild/TestBuild Variant1 Copy");
            case 2003: return Resources.Load<GameObject>("TestBuild/TestBuild Variant2 Copy");
            default: return null;
        }
    }

    public static List<LakeInvenSlot> CreateTestInventory()
    {
        List<LakeInvenSlot> list = new List<LakeInvenSlot>();
        list.Add(new LakeInvenSlot { itemId = 2001, quantity = 3 });
        list.Add(new LakeInvenSlot { itemId = 2002, quantity = 2 });
        list.Add(new LakeInvenSlot { itemId = 2003, quantity = 1 });
        return list;
    }
}
