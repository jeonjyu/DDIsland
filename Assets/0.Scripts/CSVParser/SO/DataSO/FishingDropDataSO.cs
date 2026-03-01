using System;
using UnityEngine;


[CreateAssetMenu(fileName = "FishingDropDataSO", menuName = "Scriptable Objects/Data/FishingDropDataSO")]
public class FishingDropDataSO : TableBase<int>
{
    // id
    [field: SerializeField] public int ID { get; private set; }

    // 드랍 테이블 그룹
    [field: SerializeField] public int RewardGroup { get; private set; }

    // 아이템
    [field: SerializeField] public int RewardItem { get; private set; }

    // 봄
    [field: SerializeField] public bool IsSpring { get; private set; }

    // 여름
    [field: SerializeField] public bool IsSummer { get; private set; }

    // 가을
    [field: SerializeField] public bool IsAutumn { get; private set; }

    // 겨울
    [field: SerializeField] public bool IsWinter { get; private set; }

    // 오전
    [field: SerializeField] public bool IsMorning { get; private set; }

    // 오후
    [field: SerializeField] public bool IsAfternoon { get; private set; }

    // 밤
    [field: SerializeField] public bool IsNight { get; private set; }

    // 민물
    [field: SerializeField] public bool IsLake { get; private set; }

    // 바다
    [field: SerializeField] public bool IsSea { get; private set; }

    // 벚꽃
    [field: SerializeField] public bool IsCherryblossom { get; private set; }

    // 비
    [field: SerializeField] public bool IsRain { get; private set; }

    // 단풍
    [field: SerializeField] public bool IsMaple { get; private set; }

    // 눈
    [field: SerializeField] public bool IsSnow { get; private set; }

    // 보유 골드
    [field: SerializeField] public int GoldAmount { get; private set; }

    // 등장 시간
    [field: SerializeField] public int Time { get; private set; }

    // 계절 변화 횟수
    [field: SerializeField] public int SeasonChange { get; private set; }

    // 드랍률 가중치
    [field: SerializeField] public float Probability { get; private set; }

    // 확률 증가
    [field: SerializeField] public float UpProbability { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
