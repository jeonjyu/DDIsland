using System;
using UnityEngine;

// 부위별 파츠
[Serializable]
public enum CostumeCategory
{
    None = 0,
    Head = 1,         //머리 장식
    Body = 2,         //한벌옷
    Tool = 3,         //도구 스킨
}


[CreateAssetMenu(fileName = "CostumeDataSO", menuName = "Scriptable Objects/Data/CostumeDataSO")]
public class CostumeDataSO : TableBase<int>
{
    // 코스튬 ID
    [field: SerializeField] public int CostumeID { get; private set; }

    // 코스튬 이름
    [SerializeField] private string costumeName;
    public string CostumeName_String => LocalizationManager.Instance.GetString(costumeName);

    // 설명 텍스트
    [SerializeField] private string costumeDesc;
    public string CostumeDesc_String => LocalizationManager.Instance.GetString(costumeDesc);

    // 부위별 파츠
    [field: SerializeField] public CostumeCategory costumecategoryType { get; private set; }

    // 이미지 리소스
    [field: SerializeField] public GameObject CostumeImgPath_GameObject { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => CostumeID;
}
