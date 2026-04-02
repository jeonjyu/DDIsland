using System;
using UnityEngine;

// 도움말 사용처
[Serializable]
public enum HelpLocation
{
    None = 0,
    Start = 1,            //최초 실행 도움말
    Upgrade = 2,          //업그레이드
    Shop = 3,             //상점
    Decoration = 4,       //꾸미기
    Box = 5,              //보관함
    Record = 6,           //음반
    Journal = 7,          //도감
    Quest = 8,            //퀘스트
}

// 도움말 유형
[Serializable]
public enum HelpType
{
    None = 0,
    Image = 1,       //이미지형
    Text = 2,        //텍스트형
}


[CreateAssetMenu(fileName = "HelpDataSO", menuName = "Scriptable Objects/Data/HelpDataSO")]
public class HelpDataSO : TableBase<int>
{
    // 도움말 ID
    [field: SerializeField] public int HelpID { get; private set; }

    // 도움말 사용처
    [field: SerializeField] public HelpLocation helplocationType { get; private set; }

    // 도움말 유형
    [field: SerializeField] public HelpType helpType { get; private set; }

    // 대제목
    [field: SerializeField] public string MainTitle { get; private set; }

    // 중제목
    [field: SerializeField] public string SubTitle { get; private set; }

    // 내용
    [field: SerializeField] public string Content { get; private set; }

    // 도움말 이미지 경로
    [field: SerializeField] public Sprite HelpImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => HelpID;
}
