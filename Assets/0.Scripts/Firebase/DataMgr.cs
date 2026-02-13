using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 데이터를 저장하고 저장한 데이터를 다시 받아오는 매니저 클래스
/// </summary>
public class DataMgr : MonoBehaviour
{
    public static DataMgr Instance;

    public UserAllData _allUserData = new();

    public event Action OnDataLoaded;

    private void Awake() => Instance = this;

    public string GetEnvJson() => JsonUtility.ToJson(_allUserData.Environment);
    public string GetCharJson() => JsonUtility.ToJson(_allUserData.Character);
    public string GetDecoJson() => JsonUtility.ToJson(_allUserData.Decoration);

    [ContextMenu("DB")]
    //얘를 인스펙터에서 누르시면 실제로 DB에 값이 전송됩니다
    public void SaveAllData()
    {
        //string enviroment = GetEnvJson();
        //string character = GetCharJson();
        //string decoration = GetDecoJson();

        string finalJson = JsonUtility.ToJson(_allUserData);

        FirebaseMgr.Instance.FirebaseDataTransfer(finalJson, "");
        Debug.Log("<color=yellow>모든 파트의 JSON 변환을 확인하고 저장을 실행합니다.</color>");
    }

    [ContextMenu("DBLoad")]
    //DB에서 데이터를 불러오는 메서드 입니다
    public async void LoadAllData()
    {
        // JSON 받아오기
        string json = await FirebaseMgr.Instance.FirebaseDataGet("");
        Debug.Log("데이터 수신 완료: " + json);

        if (!string.IsNullOrEmpty(json))
        {
            //JSON을 UserAllData에 있는 각각에 맞는 자료형으로 변경
            _allUserData = JsonUtility.FromJson<UserAllData>(json);

            
            //_allUserData.Decoration._buildings ??= new List<PlacedObject>();

            Debug.Log("<color=green>서버 데이터 로드 완료!</color>");

            OnDataLoaded?.Invoke();
        }
        else
        {
            Debug.Log("<color=yellow>저장된 데이터가 없습니다.</color>");
        }
    }
}
