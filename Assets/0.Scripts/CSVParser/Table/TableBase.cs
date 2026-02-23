using System;
using UnityEngine;

/// <summary>
/// 데이터 SO가 상속받을 부모 클래스
/// </summary>
/// <typeparam name="TKey"> 아이디 컬럼 타입 </typeparam>
public abstract class TableBase<TKey> : ScriptableObject
{
    // 자식이 자신의 ID 변수를 반환하도록 강제
    public abstract TKey GetID();
}

[Flags]
public enum FlagTest
{
    None = 0,           // 아무 타입도 아님
    First = 1 << 0,     // 첫번째
    Second = 1 << 1,    // 두번째
}

// 타입
public enum Test
{
    None = 0,       // 아무 타입도 아님
    MainHouse = 1,  // 메인 집
    Floor = 2,      // 바닥
}

// 타입_1/MainHouse/아무 타입도 아님_2/Floor/바닥

// 타입

/*
 * 타입
 * 1:MainHouse(메인 집)
 * 
*/


// 1/MainHouse/메인 집

// 1
// MainHouse
// 메인 집











// 2/Floor/바닥