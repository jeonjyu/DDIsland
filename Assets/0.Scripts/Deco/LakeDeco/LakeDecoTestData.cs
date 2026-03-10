using UnityEngine;
using System.Collections.Generic;

/// 호수 꾸미기 전용 테스트 데이터
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
public static class LakeDecoTestData
{
    public static string GetItemName(int itemId)
    {
        try
        {
            var data = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
            if (data != null) return data.InteriorName_String;
        }
        catch { }
        return "호수아이템 " + itemId; // 폴백 
    }
    public static Sprite GetIconSprite(int itemId)
    {
        try
        {
            var data = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
            if (data != null && data.InteriorPath_GameObject != null)
            {
                // 2D 프리팹의 SpriteRenderer에서 스프라이트 추출
                var sr = data.InteriorPath_GameObject.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && sr.sprite != null) return sr.sprite;

                // SpriteRenderer 없으면 Image 컴포넌트에서 시도
                var img = data.InteriorPath_GameObject.GetComponentInChildren<UnityEngine.UI.Image>();
                if (img != null && img.sprite != null) return img.sprite;
            }
        }
        catch { }
        return Resources.Load<Sprite>("Iron"); // 폴백 
    }
    public static GameObject GetPrefab(int itemId)
    {
        try
        {
            var data = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
            if (data != null) return data.InteriorPath_GameObject;
        }
        catch { }
        return null;
    } 
    // 배치템이 차지하는 크기
    public static Vector2Int GetItemSize(int itemId) 
    {
        try
        {
            var data = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
            if (data != null) return new Vector2Int(data.GridSizeX, data.GridSizeY);
        }
        catch { }
        return new Vector2Int(1, 1); // 폴백 
    }
 

    public static List<LakeInvenSlot> CreateTestInventory()
    {
        List<LakeInvenSlot> list = new List<LakeInvenSlot>();
        list.Add(new LakeInvenSlot { itemId = 20036, quantity = 3 }); // 빨간색 산호초 2x1
        list.Add(new LakeInvenSlot { itemId = 20037, quantity = 3 }); // 파란색 산호초 2x1
        list.Add(new LakeInvenSlot { itemId = 20038, quantity = 3 }); // 핑크 산호초 2x1
        list.Add(new LakeInvenSlot { itemId = 20039, quantity = 3 }); // 노란색 산호초 2x1
        list.Add(new LakeInvenSlot { itemId = 20040, quantity = 5 }); // 노란색 고동 1x1
        list.Add(new LakeInvenSlot { itemId = 20041, quantity = 4 }); // 핑크색 고동 1x1
        list.Add(new LakeInvenSlot { itemId = 20042, quantity = 2 }); // 닻 2x1
        list.Add(new LakeInvenSlot { itemId = 20043, quantity = 6 }); // 조개 1x1
        list.Add(new LakeInvenSlot { itemId = 20044, quantity = 2 }); // 산호바위 3x1
        list.Add(new LakeInvenSlot { itemId = 20045, quantity = 3 }); // 미역 1x1
        return list;
    }
}