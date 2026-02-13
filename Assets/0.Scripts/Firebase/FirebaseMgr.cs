using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseMgr : MonoBehaviour
{
    static public FirebaseMgr Instance;

    private FirebaseUser _user;
    static public FirebaseUser User => Instance._user;

    private FirebaseDatabase _database;
    static public FirebaseDatabase Database => Instance?._database;

    public string DeviceID => SystemInfo.deviceUniqueIdentifier;


    private void Awake()
    {
        Instance = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread
        (task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                //Firebase 초기화 성공
                _database = FirebaseDatabase.DefaultInstance;
                Debug.Log("<color=green>Firebase 초기화 성공</color>");
            }
            else
            {
                //Firebase 초기화 실패
                Debug.LogError($"<color=red>Firebase 초기화 실패: {task.Result}</color>");
            }
        }
        );
    }

    public void FirebaseDataTransfer(string json, string path)
    {
        if (_database == null)
        {
            Debug.LogError("<color=red>Firebase Database가 초기화되지 않았습니다.</color>");
            return;
        }

        DatabaseReference dbRef = Database.GetReference("Users").Child(DeviceID);

        if (!string.IsNullOrEmpty(path))
        {
            dbRef = dbRef.Child(path);
        }

        dbRef.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"<color=yellow>데이터 전송 시도</color>");
            if (task.IsCompleted)
            {
                Debug.Log("<color=green>파이어베이스에 저장 완료!</color>");
            }
            else
            {
                Debug.LogError("<color=red>저장 실패</color>");
            }
        });

    }
    public async Task<string> FirebaseDataGet(string path)
    {
        if (_database == null) return null;

        DataSnapshot snapshot = await _database.GetReference("Users").Child(DeviceID).GetValueAsync();

        if (snapshot != null && snapshot.Exists)
        {
            // JSON 문자열을 반환
            return snapshot.GetRawJsonValue();
        }

        return null; // 데이터가 없으면 null 반환
    }
}
