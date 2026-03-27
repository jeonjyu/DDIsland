using Firebase.Database;
using System;
using UnityEditor;
using UnityEngine;

public class MailSender : EditorWindow
{
    private string _mailKey = "";
    private string _title = "";
    private string _content = "";
    private int _rewardItemID;
    private int _rewardCount;
    private string _expireDate = "";
    private int _mailID;

    private string _adminKey = "";

    [MenuItem("Tools/우편 발송기")]
    public static void ShowWindow()
    {
        GetWindow<MailSender>("우편 발송");
    }

    private void OnEnable()
    {
        _adminKey = EditorPrefs.GetString("PassWord", "");

        _expireDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

        _mailKey = $"notice_{DateTime.Now:yyyyMMdd}_01";
    }

    private void OnGUI()
    {
        GUILayout.Label("서버 우편 전송용", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _adminKey = EditorGUILayout.PasswordField("패스워드입력", _adminKey);
        if (GUI.changed) EditorPrefs.SetString("Perpetual", _adminKey);

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        _mailKey = EditorGUILayout.TextField("우편 폴더명 (Key)", _mailKey);
        if (GUILayout.Button("서버에서 다음 순서 찾기", GUILayout.Width(150)))
        {
            FindNextMailSequence();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        _mailID = EditorGUILayout.IntField("우편 ID", _mailID);
        _title = EditorGUILayout.TextField("제목", _title);
        _content = EditorGUILayout.TextArea(_content, GUILayout.Height(60));
        _rewardItemID = EditorGUILayout.IntField("보상 아이템 ID", _rewardItemID);
        _rewardCount = EditorGUILayout.IntField("보상 수량", _rewardCount);
        _expireDate = EditorGUILayout.TextField("만료일", _expireDate);

        EditorGUILayout.Space();

        if (GUILayout.Button("발송", GUILayout.Height(40)))
        {
            SendMail();
        }
    }
    private async void FindNextMailSequence()
    {
        try
        {
            DataSnapshot snapshot = await FirebaseDatabase.DefaultInstance.GetReference("GlobalMails").GetValueAsync();
            int daily = 0;
            string todayStr = DateTime.Now.ToString("yyyyMMdd");

            int mailID = 10000;

            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    if (child.Key.StartsWith($"notice_{todayStr}_"))
                    {
                        string seqStr = child.Key[(child.Key.LastIndexOf('_') + 1)..];
                        if (int.TryParse(seqStr, out int seq) && seq > daily)
                        {
                            daily = seq;
                        }
                    }
                    if (child.HasChild("_mailID"))
                    {
                        string idString = child.Child("_mailID").Value.ToString();
                        if (int.TryParse(idString, out int currentID))
                        {
                            if (currentID > mailID)
                            {
                                mailID = currentID;
                            }
                        }
                    }
                }
            }

            int nextNum = daily + 1;
            _mailKey = $"notice_{todayStr}_{daily + 1:D2}";
            _mailID = mailID + 1;

            Repaint();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private async void SendMail()
    {
        if (string.IsNullOrEmpty(_adminKey))
        {
            EditorUtility.DisplayDialog("경고", "암호를 입력해주세요!", "확인");
            return;
        }

        // 전송할 데이터 포장 (암호 포함)
        MailUploadData uploadData = new()
        {
            _mailID = this._mailID,
            _title = _title,
            _content = _content,
            _rewardItemID = _rewardItemID,
            _rewardCount = _rewardCount,
            _expireDate = _expireDate,
            adminKey = _adminKey
        };

        string json = JsonUtility.ToJson(uploadData);

        try
        {
            DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.GetReference("GlobalMails").Child(_mailKey);
            await dbRef.SetRawJsonValueAsync(json);

            await dbRef.Child("adminKey").RemoveValueAsync();

            EditorUtility.DisplayDialog("성공", $"{_mailKey} 발송 완료!", "확인");
        }
        catch (Exception e)
        {
            Debug.LogError($"발송 실패: {e.Message}");
            EditorUtility.DisplayDialog("실패", "보안규칙 확인", "확인");
        }
    }

    [Serializable]
    private class MailUploadData
    {
        public int _mailID;
        public string _title;
        public string _content;
        public int _rewardItemID;
        public int _rewardCount;
        public string _expireDate;
        public string adminKey;
    }
}
