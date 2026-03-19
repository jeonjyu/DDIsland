using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// 데이터를 저장하고 저장한 데이터를 다시 받아오는 매니저 클래스
/// </summary>
public class DataHub : MonoBehaviour
{
    public UserAllData _allUserData = new();
    public event Action OnRequestSave;
    public event Action OnDataLoaded;

    public bool IsQuite {get; set;}

    [Header("자동 동기화 설정")]
    [SerializeField] private float _syncInterval = 300f;
    private WaitForSeconds wait;

    public bool IsLoaded { get; private set; } = false;


    private void Awake()
    {
        wait = new(_syncInterval);
    }
    private void Start()
    {
        StartCoroutine(InitLoadingSequence());
        StartCoroutine(AutoDataBoxSyncRoutine());
    }
    private void OnApplicationQuit()
    {
        UploadAllData();
        IsQuite = true;
        Debug.Log("게임 종료. 서버에 데이터 업로드");
    }
    private IEnumerator InitLoadingSequence()
    {
        yield return null;


        LoadAllData();
    }

    private IEnumerator AutoDataBoxSyncRoutine()
    {
        while (true)
        {
            yield return wait;

            UploadAllData();
        }
    }

    public void SaveAllData()
    {
        OnRequestSave?.Invoke();

        string localJson = JsonUtility.ToJson(_allUserData);
        PlayerPrefs.SetString("LocalSaveData", localJson);
        PlayerPrefs.Save();

    }

    [ContextMenu("DB")]
    //얘를 인스펙터에서 누르시면 실제로 DB에 값이 전송됩니다
    public void UploadAllData()
    {
        OnRequestSave?.Invoke();

        string finalJson = JsonUtility.ToJson(_allUserData);
        
        FirebaseMgr.Instance.FirebaseDataTransfer(finalJson, "");
        
    }

    [ContextMenu("DBLoad")]
    //DB에서 데이터를 불러오는 메서드 입니다
    public async void LoadAllData()
    {
        // JSON 받아오기
        string json = await FirebaseMgr.Instance.FirebaseDataGet();

        if (!string.IsNullOrEmpty(json))
        {
            //JSON을 UserAllData에 있는 각각에 맞는 자료형으로 변경
            _allUserData = JsonUtility.FromJson<UserAllData>(json);


            //_allUserData.Decoration._buildings ??= new List<PlacedObject>();

            IsLoaded = true;
            OnDataLoaded?.Invoke();
        }
        else
        {
            SetNewUserData();

            UploadAllData();

            //if (_allUserData.Decoration != null)
            //{
            //    _allUserData.Decoration._buildings ??= new List<PlacedObject>();
            //}
            IsLoaded = true;
            OnDataLoaded?.Invoke();
        }
    }
    //새 유저를 위한 데이터입니다. 아직 뭘 넣어야할지 몰라 일단 환경 데이터만 넣었습니다
    private void SetNewUserData()
    {
        _allUserData = new UserAllData();

        EnvironmentModel env = new ();
        DateTime now = DateTime.UtcNow;
        env.UpdateTimeSet(now); // 계절 및 시간대 계산 실행
        _allUserData.Environment = env.SaveData();

        //_allUserData.Character = new ();
        //_allUserData.Decoration = new();
        //_allUserData.Collection = new();
    }
}
