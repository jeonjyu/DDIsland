using System;
using UnityEngine;


//public enum ApplyType { None = 0, Add=1, Set=2 }
//public enum StatType { None =0, BaseHunger=1, BaseStamina=2, BaseMoveSpeed=3, BaseFishingSpeed=4 }


[System.Serializable]
public class UpgradeData
{ 
    public int ID;           // 식별자 
    public string GroupID;       // 스탯 그룹 ID
    public ApplyType applyType; // 적용 타입 (0=Add 추가, 1=Set 설정)
    public StatType StatType; // 스탯 타입 (1=포만감, 2=스태미나, 3=이동속도, 4=낚시속도) 
    public int Level;         // 현재 업그레이드 단계
    public int Cost;          // 다음 레벨로 가는 비용
    public float Value;       // Add면 증가량, Set이면 설정값 
    public bool IsMax;        // 최대치 여부, 마지막 레벨인지 
    public float Probability = 1f; // 업그레이드 성공 확률
}

