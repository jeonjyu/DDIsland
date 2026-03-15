using System.Collections.Generic;
using UnityEngine;
public enum FixGroup
{
    None = 0,
    House = 1,
    Box = 2,
    LpPlayer = 3,
    Bed = 4
}
public class FixedBuilding : MonoBehaviour
{
    [Header("고유 식별 번호 (예: 1번 자리 집, 2번 자리 헛간)")]
    public int LocationID;

    [Header("현재 적용된 아이템 ID")]
    public int CurrentItemID;

    // 초기화용 메서드
    public void Setup(int locationId, int itemId)
    {
        LocationID = locationId;
        CurrentItemID = itemId;
    }
}
