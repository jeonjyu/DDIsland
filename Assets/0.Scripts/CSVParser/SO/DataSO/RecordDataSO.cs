using System;
using UnityEngine;

// 음반 종류
[Serializable]
public enum RecordType
{
    None = 0,
    Background = 1,      //배경음
    Ambience = 2,        //환경음
}

// 배경음 테마
[Serializable]
public enum BgTheme
{
    None = 0,
    General = 1,             //일반
    Spring = 2,              //봄
    Summer = 3,              //여름
    Autumn = 4,              //가을
    Winter = 5,              //겨울
    Collaboration = 6,       //콜라보
}

// 환경음 소스
[Serializable]
public enum AmbSource
{
    None = 0,
    Weather = 1,      //날씨
    Nature = 2,       //자연
    Life = 3,         //생활 소음
}

// 기본 환경음 유형
[Serializable]
public enum DefaultAmbType
{
    None = 0,
    Spring = 1,     //봄
    Summer = 2,     //여름
    Autumn = 3,     //가을
    Winter = 4,     //겨울
    Day = 5,        //오전
    Sunset = 6,     //오후
    Night = 7,      //밤
}


[CreateAssetMenu(fileName = "RecordDataSO", menuName = "Scriptable Objects/Data/RecordDataSO")]
public class RecordDataSO : TableBase<int>
{
    // 음반 ID
    [field: SerializeField] public int RecordID { get; private set; }

    // 음반 이름
    [SerializeField] private string recordName;
    public string RecordName_String => LocalizationManager.Instance.GetString(recordName);

    // 음반 설명
    [SerializeField] private string recordDesc;
    public string RecordDesc_String => LocalizationManager.Instance.GetString(recordDesc);

    // 아티스트
    [SerializeField] private string recordArtist;
    public string RecordArtist_String => LocalizationManager.Instance.GetString(recordArtist);

    // 음반 종류
    [field: SerializeField] public RecordType recordType { get; private set; }

    // 기본 제공 여부
    [field: SerializeField] public bool IsDefaultRecord { get; private set; }

    // 배경음 테마
    [field: SerializeField] public BgTheme bgthemeType { get; private set; }

    // 환경음 소스
    [field: SerializeField] public AmbSource ambsourceType { get; private set; }

    // 기본 환경음 유형
    [field: SerializeField] public DefaultAmbType defaultambType { get; private set; }

    // 음반 이미지 리소스 경로
    [field: SerializeField] public Sprite RecordImgPath_Sprite { get; private set; }

    // 음반 사운드 리소스 경로
    [field: SerializeField] public AudioClip RecordSoundPath_AudioClip { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => RecordID;
}
