using UnityEngine;

public class MailData
{
    public string _mailID;       
    public string _title;        
    public string _content;
    public Sprite _icon;

    public int _rewardItemID;    // 보상 아이템 ID (0이면 보상 없는 순수 공지사항)
    public int _rewardCount;     // 보상 개수

    public bool _isRead;         // 읽음 여부
    public bool _isClaimed;      // 보상 수령 여부
    public string _expireDate;   // 만료(삭제) 날짜
}
