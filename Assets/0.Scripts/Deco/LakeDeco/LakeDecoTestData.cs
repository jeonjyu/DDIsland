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
            case 1011: return "작은 수초";
            case 1012: return "큰 수초";
            case 1013: return "조약돌";
            case 1014: return "산호 조각";
            case 1015: return "작은 바위";
            case 1016: return "보물 상자";
            case 1017: return "해초 군락";
            case 1018: return "모래 언덕";
            case 1019: return "조개껍데기";
            case 1020: return "물풀";
            //case 10001: return "해초";
            //case 10002: return "해초2";
            //case 10004: return "분홍 산호";
            //case 10005: return "닻";
            //case 10006: return "노란 소라";
            //case 10007: return "파란 산호";
            //case 10008: return "조개";
            //case 10009: return "분홍 소라";
            //case 10010: return "빨간 산호";
            default: return "호수아이템 " + itemId;
        }
    }

    public static Sprite GetIconSprite(int itemId)
    {
        // TODO: 나중에 실제 스프라이트 로드
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
            case 1011: return Resources.Load<Sprite>("anchor");
            case 1012: return Resources.Load<Sprite>("coral reef");
            case 1013: return Resources.Load<Sprite>("coral reef3");
            case 1014: return Resources.Load<Sprite>("seashell");
            case 1015: return Resources.Load<Sprite>("seashell with pearl");
            case 1016: return Resources.Load<Sprite>("seashells");
            case 1017: return Resources.Load<Sprite>("algae");
            case 1018: return Resources.Load<Sprite>("algae 2");
            case 1019: return Resources.Load<Sprite>("Bone");
            case 1020: return Resources.Load<Sprite>("Wood");
            //case 10001: return Resources.Load<Sprite>("algae");
            //case 10002: return Resources.Load<Sprite>("algae 2");
            //case 10004: return Resources.Load<Sprite>("coral reef");
            //case 10005: return Resources.Load<Sprite>("anchor");
            //case 10006: return Resources.Load<Sprite>("seashell with pearl");
            //case 10007: return Resources.Load<Sprite>("coral reef3");
            //case 10008: return Resources.Load<Sprite>("seashell");
            //case 10009: return Resources.Load<Sprite>("seashells");
            //case 10010: return Resources.Load<Sprite>("coral reef");
            default: return Resources.Load<Sprite>("Iron");
        }
        // return Resources.Load<Sprite>("Icons/Lake/" + itemId);
    }
    public static GameObject GetPrefab(int itemId)
    {
        switch (itemId)
        {
           // case 10001: return Resources.Load<GameObject>("PROP_Lake_Seaweed_001");
           // case 10002: return Resources.Load<GameObject>("PROP_Lake_Seaweed_001");
           //// case 10003: return Resources.Load<GameObject>("PROP_Lake_CoralRock_001");
           // case 10004: return Resources.Load<GameObject>("PROP_Lake_PinkCoral_001");
           // case 10005: return Resources.Load<GameObject>("PROP_Lake_Anchor_001");
           // case 10006: return Resources.Load<GameObject>("PROP_Lake_YellowConch_001");
           // case 10007: return Resources.Load<GameObject>("PROP_Lake_BlueCoral_001");
           // case 10008: return Resources.Load<GameObject>("PROP_Lake_Clam_001");
           // case 10009: return Resources.Load<GameObject>("PROP_Lake_PinkConch_001");
           // case 10010: return Resources.Load<GameObject>("PROP_Lake_RedCoral_001");
            default: return null;
        }
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
        list.Add(new LakeInvenSlot { itemId = 1011, quantity = 3 });
        list.Add(new LakeInvenSlot { itemId = 1012, quantity = 2 });
        list.Add(new LakeInvenSlot { itemId = 1013, quantity = 5 });
        list.Add(new LakeInvenSlot { itemId = 1014, quantity = 1 });
        list.Add(new LakeInvenSlot { itemId = 1015, quantity = 4 });
        list.Add(new LakeInvenSlot { itemId = 1016, quantity = 1 });
        list.Add(new LakeInvenSlot { itemId = 1017, quantity = 2 });
        list.Add(new LakeInvenSlot { itemId = 1018, quantity = 3 });
        list.Add(new LakeInvenSlot { itemId = 1019, quantity = 2 });
        list.Add(new LakeInvenSlot { itemId = 1020, quantity = 3 });
      //  list.Add(new LakeInvenSlot { itemId = 10001, quantity = 6 });
      //  list.Add(new LakeInvenSlot { itemId = 10002, quantity = 2 });
      ////  list.Add(new LakeInvenSlot { itemId = 10003, quantity = 6 });
      //  list.Add(new LakeInvenSlot { itemId = 10004, quantity = 2 });
      //  list.Add(new LakeInvenSlot { itemId = 10005, quantity = 6 });
      //  list.Add(new LakeInvenSlot { itemId = 10006, quantity = 2 });
      //  list.Add(new LakeInvenSlot { itemId = 10007, quantity = 6 });
      //  list.Add(new LakeInvenSlot { itemId = 10008, quantity = 2 });
      //  list.Add(new LakeInvenSlot { itemId = 10009, quantity = 2 });
      //  list.Add(new LakeInvenSlot { itemId = 10010, quantity = 2 });
        return list;
    }
}