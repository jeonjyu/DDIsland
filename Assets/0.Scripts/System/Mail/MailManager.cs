using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MailManager : Singleton<MailManager>
{
    private List<MailData> _serverMails = new();

    private HashSet<string> _readMailIDs = new();
    private HashSet<string> _claimedMailIDs = new();

    public event Action OnMailUpdated; // 새로고침용 이벤트
    #region 우편 데이터
    public List<MailData> GetAllMails()
    {
        return _serverMails;
    }

    public bool IsMailRead(string mailID)
    {
        return _readMailIDs.Contains(mailID);
    }

    public bool IsMailClaimed(string mailID)
    {
        return _claimedMailIDs.Contains(mailID);
    }
    #endregion

    private void Start()
    {
        // TODO: DataManager.Instance.Hub.OnDataLoaded += SyncMailDataLoad 이벤트 구독시키기;
    }

    public void MarkAsRead(string mailID)
    {
        if (!_readMailIDs.Contains(mailID))
        {
            _readMailIDs.Add(mailID);
            OnMailUpdated?.Invoke();

             DataManager.Instance.Hub.SaveAllData();
        }
    }

    public void ClaimReward(MailData mail)
    {
        if (_claimedMailIDs.Contains(mail._mailID) || mail._rewardItemID == 0) return;

        var currentData = DataManager.Instance.CurrencyDatabase.CurrencyInfoData[mail._rewardItemID];

        if (currentData)
        {
            if (currentData.ID == 202)
            {
                GameManager.Instance.SetGold(mail._rewardCount);
            }
            else if (currentData.ID == 201)
            {
                DataManager.Instance.RecordDatabase.LpPieceCount += mail._rewardCount;
            }
        }
        else
        {
            // 기타 아이템
        }

        _claimedMailIDs.Add(mail._mailID);
        _readMailIDs.Add(mail._mailID);

        OnMailUpdated?.Invoke();

         _ = DataManager.Instance.Hub.UploadAllData();
    }

    public void LoadGlobalMails()
    {
        DatabaseReference globalMailRef = FirebaseDatabase.DefaultInstance.GetReference("GlobalMails");

        globalMailRef.GetValueAsync().ContinueWith(task => 
        {
            if (task.IsFaulted)
            {
                return;
            }

            DataSnapshot snapshot = task.Result;
            _serverMails.Clear();

            foreach (var child in snapshot.Children)
            {
                string json = child.GetRawJsonValue();
                MailData mail = JsonUtility.FromJson<MailData>(json);

                _serverMails.Add(mail);
            }

            ThreadDispatcher.Instance.Enqueue(() => 
            {
                OnMailUpdated?.Invoke();
            });
        });
    }

#if UNITY_EDITOR
    public void AddTestMail()
    {
        MailData newMail = new()
        {
            _mailID = "test_mail_01",
            _title = "테스트 우편 도착!",
            _content = "이것은 슬롯에 데이터가 잘 들어가는지 확인하기 위한 테스트 우편입니다.",
            _rewardItemID = 202, // 골드 ID (기존 코드 기준)
            _rewardCount = 5000,
            _isRead = false,
            _isClaimed = false
        };

        _serverMails.Add(newMail);

        OnMailUpdated?.Invoke();
    }
#endif
}
