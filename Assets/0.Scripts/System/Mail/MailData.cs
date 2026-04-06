using System;
using UnityEngine;

[Serializable]
public class MailData
{
    public string _mailID;
    public string _title_kr;
    public string _title_en;
    public string _content_kr;
    public string _content_en;
    public Sprite _icon;

    public string _rewardItemID;    // 보상 아이템 ID (0이면 보상 없는 순수 공지사항)
    public string _rewardCount;     // 보상 개수

    public bool _isRead;         // 읽음 여부
    public bool _isClaimed;      // 보상 수령 여부
    public string _expireDate;   // 만료(삭제) 날짜
}
