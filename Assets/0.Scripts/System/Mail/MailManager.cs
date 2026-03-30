using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MailManager : Singleton<MailManager>
{
    private List<MailData> _serverMails = new();

    private HashSet<string> _readMailIDs = new();
    private HashSet<string> _claimedMailIDs = new();
    private HashSet<string> _deletedMailIDs = new();

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
    public bool IsMailDeleted(string mailID)
    {
        return _deletedMailIDs.Contains(mailID);
    }
    #endregion

    private void Start()
    {
        if (DataManager.Instance.Hub.IsLoaded)
        {
            InitMailData();
        }
        else
        {
            DataManager.Instance.Hub.OnDataLoaded += InitMailData;
        }

        DataManager.Instance.Hub.OnRequestSave += SyncMailDataSave;
        LoadGlobalMails();

        foreach(var a in _deletedMailIDs)
        {
            Debug.Log(a);
        }
    }

    private void InitMailData()
    {
        var mailData = DataManager.Instance.Hub._allUserData.Mail;
        if (mailData == null) return;

        _readMailIDs = new HashSet<string>(mailData._readMailIDs ?? new List<string>());
        _claimedMailIDs = new HashSet<string>(mailData._claimedMailIDs ?? new List<string>());
        _deletedMailIDs = new HashSet<string>(mailData._deletedMailIDs ?? new List<string>());

        OnMailUpdated?.Invoke();
    }

    public void MarkAsRead(string mailID)
    {
        if (!_readMailIDs.Contains(mailID))
        {
            _readMailIDs.Add(mailID);
            OnMailUpdated?.Invoke();

             _ = DataManager.Instance.Hub.UploadAllData();
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
                RewardEffect.Instance.PlayQuestGoldEffect();
            }
            else if (currentData.ID == 201)
            {
                DataManager.Instance.RecordDatabase.LpPieceCount += mail._rewardCount;
                RewardEffect.Instance.PlayQuestLpEffect();
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

    public void ClaimAllRewards()
    {
        bool hasClaimed = false;

        foreach (var mail in _serverMails)
        {
            // 우편함에 내용물이 존재하고, 받지 않았으며, 지워지지 않은 물품 들 중
            if (mail._rewardItemID != 0 && !_claimedMailIDs.Contains(mail._mailID) && !_deletedMailIDs.Contains(mail._mailID))
            {
                var currentData = DataManager.Instance.CurrencyDatabase.CurrencyInfoData[mail._rewardItemID];
                //재화만 획득 처리
                if (currentData)
                {
                    if (currentData.ID == 202)
                    {
                        GameManager.Instance.SetGold(mail._rewardCount);
                        RewardEffect.Instance.PlayQuestGoldEffect();
                    }
                    else if (currentData.ID == 201)
                    {
                        DataManager.Instance.RecordDatabase.LpPieceCount += mail._rewardCount;
                        RewardEffect.Instance.PlayQuestLpEffect();
                    }
                }

                _claimedMailIDs.Add(mail._mailID);
                _readMailIDs.Add(mail._mailID);
                hasClaimed = true;
            }
        }

        if (hasClaimed)
        {
            OnMailUpdated?.Invoke();
            _ = DataManager.Instance.Hub.UploadAllData();
        }
    }

    public void DeleteAllReadMails()
    {
        bool hasDeleted = false;

        foreach (var mail in _serverMails)
        {
            // 이미 삭제된 건 무시
            if (_deletedMailIDs.Contains(mail._mailID)) continue;

            //만약 아이템이 없거나 단순 공지용이 읽은 상태일 경우
            if ((mail._rewardItemID == 0 && _readMailIDs.Contains(mail._mailID)) ||
                _claimedMailIDs.Contains(mail._mailID))
            {
                _deletedMailIDs.Add(mail._mailID);
                hasDeleted = true;
            }
        }

        if (hasDeleted)
        {
            OnMailUpdated?.Invoke();
            _ = DataManager.Instance.Hub.UploadAllData();
        }
    }

    public void LoadGlobalMails()
    {
        DatabaseReference globalMailRef = FirebaseDatabase.DefaultInstance.GetReference("GlobalMails");

        globalMailRef.KeepSynced(true);

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
                EnforceMail();
                OnMailUpdated?.Invoke();
            });
        });
    }

    private void EnforceMail()
    {
        int maxMail = 100;

        var visibleMails = _serverMails.Where(m => !_deletedMailIDs.Contains(m._mailID)).ToList();

        if (visibleMails.Count <= maxMail) return;

        int excessCount = visibleMails.Count - maxMail;

        var deleteCandidates = visibleMails.Where(m =>
            _claimedMailIDs.Contains(m._mailID)).ToList();

        deleteCandidates.Sort((a, b) =>
        {
            DateTime.TryParse(a._expireDate, out DateTime dateA);
            DateTime.TryParse(b._expireDate, out DateTime dateB);
            return dateA.CompareTo(dateB);
        });

        bool isDeleted = false;

        for (int i = 0; i < deleteCandidates.Count && i < excessCount; i++)
        {
            _deletedMailIDs.Add(deleteCandidates[i]._mailID);
            isDeleted = true;
        }

        if (isDeleted)
        {
            _ = DataManager.Instance.Hub.UploadAllData();
        }
    }

    private void SyncMailDataSave()
    {
        DataManager.Instance.Hub._allUserData.Mail._readMailIDs = new List<string>(_readMailIDs);
        DataManager.Instance.Hub._allUserData.Mail._claimedMailIDs = new List<string>(_claimedMailIDs);
        DataManager.Instance.Hub._allUserData.Mail._deletedMailIDs = new List<string>(_deletedMailIDs);
    }
}
