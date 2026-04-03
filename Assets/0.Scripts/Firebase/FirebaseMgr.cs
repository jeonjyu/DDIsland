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

    public string DeviceID
    {

        get
        {
            if (_user != null)
                return _user.UserId;
            else
                return SystemInfo.deviceUniqueIdentifier;
        }
    }

    public bool IsInitailized => _isInitialized;

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

                FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                if (auth.CurrentUser != null)
                {
                    _user = auth.CurrentUser;
                    _isInitialized = true; 
                }
                else
                {
                   
                    auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
                    {
                        if (authTask.IsCanceled || authTask.IsFaulted)
                        {
                            return;
                        }
                        _user = authTask.Result.User;
                        _isInitialized = true; 
                    });
                }
            }
            else
            {
            }
        });
    }

    public Task FirebaseDataTransfer(string json, string path)
    {
        if (!_isInitialized || _database == null)
        {
            return Task.CompletedTask;
        }

        if (Database == null)
        {
            return Task.CompletedTask;
        }

        DatabaseReference dbRef = Database.GetReference("Users").Child(DeviceID);

        if (!string.IsNullOrEmpty(path))
        {
            dbRef = dbRef.Child(path);
        }

        return dbRef.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
            }
            else
            {
            }
        });
    }
    
    public async Task<string> FirebaseDataGet(string path = "")
    {
        if (!_isInitialized || _database == null)
        {
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

    public void FirebaseDataDelete(string path = "")
    {
        if (!_isInitialized || _database == null)
        {
            return;
        }

        DatabaseReference dbRef = Database.GetReference("Users").Child(DeviceID);

        if (!string.IsNullOrEmpty(path))
        {
            dbRef = dbRef.Child(path);
        }

        dbRef.RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
            }
            else
            {
            }
        });
    }
}
