using UnityEngine;
using System.Collections.Generic;

/// 호수 배경 전용 테스트 데이터
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
/// 아이템 추가는 GetSpriteMap()에 항목만 늘리면 됨
public static class LakeDecoDataV2
{
    public static Dictionary<int, Sprite> GetSpriteMap()
    {
        var map = new Dictionary<int, Sprite>();

        // TODO: 나중에 실제 스프라이트 경로로 교체

        // 슬롯 0용 (장식물 1번)
        map[1001] = Resources.Load<Sprite>("01Theme_1");  
        map[1002] = Resources.Load<Sprite>("02Theme_1");  
        map[1003] = Resources.Load<Sprite>("03Theme_1");  
        //map[1004] = Resources.Load<Sprite>("");  
        //map[1005] = Resources.Load<Sprite>("");  


        // 슬롯 1용 (장식물 2번)
        map[2001] = Resources.Load<Sprite>("01Theme_2"); 
        map[2002] = Resources.Load<Sprite>("02Theme_2"); 
        map[2003] = Resources.Load<Sprite>("03Theme_2"); 
        //map[2004] = Resources.Load<Sprite>(""); 
        //map[2005] = Resources.Load<Sprite>(""); 
      
        return map;
    }

    // 슬롯 0 아이템 ID 목록
    public static List<int> GetSlot0Ids()
    {
        return new List<int> { 1001, 1002, 1003 };//, 1004, 1005 };
    }

    // 슬롯 1 아이템 ID 목록
    public static List<int> GetSlot1Ids()
    {
        return new List<int> { 2001, 2002, 2003 };//, 2004, 2005 };
    }
}