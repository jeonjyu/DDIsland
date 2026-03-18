using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class FirebaseMgr : MonoBehaviour
{
    static public FirebaseMgr Instance;
    private bool _isInitialized = false;

    private FirebaseUser _user;
    static public FirebaseUser User => Instance._user;

    private FirebaseDatabase _database;
    static public FirebaseDatabase Database => Instance?._database;

    public string DeviceID => SystemInfo.deviceUniqueIdentifier;


    private void Awake()
    {
        Instance = this;

        InitializeFirebase();
    }
    private void InitializeFirebase()
    {
        FirebaseApp.LogLevel = LogLevel.Error;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                _database = FirebaseDatabase.DefaultInstance;
                _isInitialized = true;
                Debug.Log("<color=green>Firebase 초기화 성공</color>");
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {task.Result}");
            }
        });
    }

    public void FirebaseDataTransfer(string json, string path)
    {
        if (!_isInitialized || _database == null)
        {
            return;
        }

        if (Database == null)
        {
            return;
        }

        DatabaseReference dbRef = Database.GetReference("Users").Child(DeviceID);

        if (!string.IsNullOrEmpty(path))
        {
            dbRef = dbRef.Child(path);
        }

        dbRef.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"<color=red>저장 실패:</color> {task.Exception?.ToString()}");
            }
            else
            {
                Debug.Log("<color=green>파이어베이스에 저장 완료!</color>");
            }
        });
        
    }
    
    public async Task<string> FirebaseDataGet(string path = "")
    {
        if (!_isInitialized || _database == null)
        {
            Debug.LogWarning("Firebase가 아직 초기화되지 않았습니다.");
            return null;
        }

        try
        {
            DatabaseReference dbRef = Database.GetReference("Users").Child(DeviceID);

            if (!string.IsNullOrEmpty(path))
            {
                dbRef = dbRef.Child(path);
            }

            DataSnapshot snapshot = await dbRef.GetValueAsync();

            if (snapshot != null && snapshot.Exists)
            {
                // JSON 문자열을 반환
                return snapshot.GetRawJsonValue();
            }
        }
        catch(Exception e)
        {
            if (e is FirebaseException firebaseEx)
            {
                // 에러코드 및 상세 메세지를 출력
                Debug.LogError($"[파이어베이스 오류] 코드: {firebaseEx.ErrorCode} / 메세지: {firebaseEx.Message}");
            }
            else
            {
                Debug.LogError($"[기타 오류] {e}");
            }
        }

        return null; // 데이터가 없으면 null 반환
    }
}
