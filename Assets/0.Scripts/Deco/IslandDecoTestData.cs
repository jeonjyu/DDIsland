using UnityEngine;
using System.Collections.Generic;

/// 섬 꾸미기 전용 테스트 데이터
/// 나중에 파베/CSV 연결하면 이 파일만 교체하면 됨
public static class IslandDecoTestData
{
    //아직 스프라이트 없어서 기본 로직 유지 중
    public static Sprite GetIconSprite(int itemId)
    {
        //// TODO: 나중에 실제 스프라이트 로드
        //switch (itemId)
        //{
        //    //case 20007: return Resources.Load<Sprite>("Gold");
        //    //case 20008: return Resources.Load<Sprite>("Gold");
        //    //case 20009: return Resources.Load<Sprite>("Gold");
        //    //case 20010: return Resources.Load<Sprite>("Gold");
        //    //case 20016: return Resources.Load<Sprite>("Gold");
        //    //case 20017: return Resources.Load<Sprite>("Gold");
        //    //case 20018: return Resources.Load<Sprite>("Gold");
        //    //case 20025: return Resources.Load<Sprite>("Gold");
        //    //case 20026: return Resources.Load<Sprite>("Gold");
        //    //case 20027: return Resources.Load<Sprite>("Gold");
        //    //case 20028: return Resources.Load<Sprite>("Gold");
        //    //case 20029: return Resources.Load<Sprite>("Gold");
        //    //case 20030: return Resources.Load<Sprite>("Gold");
        //    //case 20031: return Resources.Load<Sprite>("Gold");

        //    default: return Resources.Load<Sprite>("Iron");
        //}
             
        try
        {
            var data = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
            if (data != null)
            {
                // Fix(고정장식물)
                if (data.interior_itemType == Interior_ItemType.Fix)
                    return Resources.Load<Sprite>("Iron");

                // Free(자유장식물)
                if (data.InteriorPath_GameObject != null)
                {
                    var col = data.InteriorPath_GameObject.GetComponentInChildren<Collider>();
                    if (col != null)
                        return Resources.Load<Sprite>("Gold");   // 콜라이더 있음
                    else
                        return Resources.Load<Sprite>("Silver"); // 콜라이더 없음
                }
            }
        }
        catch { }
        return Resources.Load<Sprite>("Iron"); // 폴백
    }


    public static List<LakeInvenSlot> CreateInventory()
    {
        List<LakeInvenSlot> list = new List<LakeInvenSlot>();

        // Free (자유장식물) 
        list.Add(new LakeInvenSlot { itemId = 20025, quantity = 5 }); // 빨간색 꽃
        list.Add(new LakeInvenSlot { itemId = 20026, quantity = 5 }); // 핑크색 꽃
        list.Add(new LakeInvenSlot { itemId = 20027, quantity = 5 }); // 보라색 꽃
        list.Add(new LakeInvenSlot { itemId = 20028, quantity = 5 }); // 회색 꽃
        list.Add(new LakeInvenSlot { itemId = 20029, quantity = 5 }); // 흰색 꽃
        list.Add(new LakeInvenSlot { itemId = 20030, quantity = 5 }); // 노란색 꽃
        list.Add(new LakeInvenSlot { itemId = 20031, quantity = 5 }); // 파란색 꽃
        // free인데 콜라이더가 없나봄 
        list.Add(new LakeInvenSlot { itemId = 20007, quantity = 3 }); // 웃는화분
        list.Add(new LakeInvenSlot { itemId = 20008, quantity = 3 }); // 자고 있는 화분
        list.Add(new LakeInvenSlot { itemId = 20009, quantity = 2 }); // 중간 크기 화분
        list.Add(new LakeInvenSlot { itemId = 20010, quantity = 2 }); // 큰 사이즈 화분
        list.Add(new LakeInvenSlot { itemId = 20016, quantity = 2 }); // 토끼 인형
        list.Add(new LakeInvenSlot { itemId = 20017, quantity = 2 }); // 판다 인형
        list.Add(new LakeInvenSlot { itemId = 20018, quantity = 1 }); // 큰 사이즈 토끼인형
        // Fix (고정장식물)
        list.Add(new LakeInvenSlot { itemId = 20001, quantity = 1 }); // 기본 집
        list.Add(new LakeInvenSlot { itemId = 20002, quantity = 1 }); // 중간 크기 집
        list.Add(new LakeInvenSlot { itemId = 20003, quantity = 1 }); // 큰 사이즈 집
        list.Add(new LakeInvenSlot { itemId = 20004, quantity = 1 }); // 기본 모닥불
        list.Add(new LakeInvenSlot { itemId = 20005, quantity = 1 }); // 기본 보관함
        list.Add(new LakeInvenSlot { itemId = 20006, quantity = 1 }); // 아이스박스
        list.Add(new LakeInvenSlot { itemId = 20011, quantity = 2 }); // 기본 울타리
        list.Add(new LakeInvenSlot { itemId = 20012, quantity = 1 }); // 기본 LP 플레이어
        list.Add(new LakeInvenSlot { itemId = 20013, quantity = 1 }); // 앰프
        list.Add(new LakeInvenSlot { itemId = 20014, quantity = 1 }); // 스피커
        list.Add(new LakeInvenSlot { itemId = 20015, quantity = 1 }); // 기본 우체통
        list.Add(new LakeInvenSlot { itemId = 20019, quantity = 1 }); // 봄) 소나무
        list.Add(new LakeInvenSlot { itemId = 20020, quantity = 1 }); // 봄) 나무
        list.Add(new LakeInvenSlot { itemId = 20021, quantity = 1 }); // 가을) 소나무
        list.Add(new LakeInvenSlot { itemId = 20022, quantity = 1 }); // 가을) 나무
        list.Add(new LakeInvenSlot { itemId = 20023, quantity = 1 }); // 겨울) 소나무
        list.Add(new LakeInvenSlot { itemId = 20024, quantity = 1 }); // 겨울) 나무
        list.Add(new LakeInvenSlot { itemId = 20032, quantity = 1 }); // 기본 침대
        list.Add(new LakeInvenSlot { itemId = 20033, quantity = 1 }); // 싱글 베드
        list.Add(new LakeInvenSlot { itemId = 20034, quantity = 1 }); // 퀸 베드
        list.Add(new LakeInvenSlot { itemId = 20035, quantity = 1 }); // 킹 베드
        
        // TODO: 20046~20048 프리팹 넣으면 주석 해제
        // list.Add(new LakeInvenSlot { itemId = 20046, quantity = 1 }); // 낚시의자
        // list.Add(new LakeInvenSlot { itemId = 20047, quantity = 1 }); // 식사공간
        // list.Add(new LakeInvenSlot { itemId = 20048, quantity = 1 }); // 상점
       
        return list;
    }
}
