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

}
