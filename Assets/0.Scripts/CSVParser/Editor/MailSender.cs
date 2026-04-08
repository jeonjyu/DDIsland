using Firebase.Database;
using System;
using UnityEditor;
using UnityEngine;

public class MailSender : EditorWindow
{
    private string _mailKey = "";
    private string _title_kr = "";
    private string _content_kr = "";
    private string _title_en = "";
    private string _content_en = "";
    private string _rewardItemIDs = "";
    private string _rewardCounts = "";
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
        _mailKey = EditorGUILayout.TextField("우편 폴더명 (Key)", _mailKey);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("불러오기", GUILayout.Width(80)))
        {
            LoadMailData();
        }

        if (GUILayout.Button("서버에서 다음 순서 찾기", GUILayout.Width(150)))
        {
            FindNextMailSequence();
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        _mailID = EditorGUILayout.IntField("우편 ID", _mailID);
        GUILayout.Label("한국어 우편", EditorStyles.boldLabel);
        _title_kr = EditorGUILayout.TextField("제목 (한)", _title_kr);
        _content_kr = EditorGUILayout.TextArea(_content_kr, GUILayout.Height(60));

        EditorGUILayout.Space();

        GUILayout.Label("영어 우편", EditorStyles.boldLabel);
        _title_en = EditorGUILayout.TextField("제목 (영)", _title_en);
        _content_en = EditorGUILayout.TextArea(_content_en, GUILayout.Height(60));
        _rewardItemIDs = EditorGUILayout.TextField("보상 아이템 ID", _rewardItemIDs);
        _rewardCounts = EditorGUILayout.TextField("보상 수량", _rewardCounts);
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
            _title_kr = _title_kr,
            _title_en = _title_en,
            _content_kr = _content_kr,
            _content_en = _content_en,
            _rewardItemID = _rewardItemIDs,
            _rewardCount = _rewardCounts,
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

    private async void LoadMailData()
    {
        if (string.IsNullOrEmpty(_mailKey))
        {
            EditorUtility.DisplayDialog("알림", "불러올 우편의 Key를 입력해주세요.", "확인");
            return;
        }

        try
        {
            DataSnapshot snapshot = await FirebaseDatabase.DefaultInstance.GetReference("GlobalMails").Child(_mailKey).GetValueAsync();

            if (snapshot.Exists)
            {
                string json = snapshot.GetRawJsonValue();
                MailUploadData loadedData = JsonUtility.FromJson<MailUploadData>(json);

                _mailID = loadedData._mailID;
                _title_kr = loadedData._title_kr;
                _title_en = loadedData._title_en;
                _content_kr = loadedData._content_kr;
                _content_en = loadedData._content_en;
                _rewardItemIDs = loadedData._rewardItemID;
                _rewardCounts = loadedData._rewardCount;
                _expireDate = loadedData._expireDate;

                // 에디터 UI 새로고침 (이걸 해줘야 화면에 텍스트가 뜹니다)
                Repaint();

                EditorUtility.DisplayDialog("성공", "데이터를 성공적으로 불러왔습니다!\n수정 후 [발송] 버튼을 누르면 덮어쓰기 됩니다.", "확인");
            }
            else
            {
                EditorUtility.DisplayDialog("실패", "서버에 해당 Key를 가진 우편이 존재하지 않습니다.", "확인");
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("에러", "데이터를 불러오는 중 문제가 발생했습니다.", "확인");
        }
    }

    [Serializable]
    private class MailUploadData
    {
        public int _mailID;
        public string _title_kr;  
        public string _title_en;  
        public string _content_kr; 
        public string _content_en; 
        public string _rewardItemID;
        public string _rewardCount;
        public string _expireDate;
        public string adminKey;
    }
}
