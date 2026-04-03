using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject obj2;
    [SerializeField] private GameObject obj3;
    [SerializeField] private GameObject obj4;
    [SerializeField] private GameObject obj5;

    [SerializeField] private GameObject debugUI;

    [SerializeField] private TMP_Text timeText;

    private float timeMultiplier = 60f;  // 시간 배율

    private const string TIMEKEY = "TimeKey";

    private float playTimer = 0f;

    public float TotalPlayTime
    {
        get
        {
            return PlayerPrefs.GetFloat(TIMEKEY, 0f);
        }

        set
        {
            PlayerPrefs.SetFloat(TIMEKEY, value);
        }
    }

    public void OnClick_NormalMulti() => Time.timeScale = 1f;
    public void OnClick_FastMulti() => Time.timeScale = timeMultiplier;
    public void OnClick_TimeReset()
    {
        playTimer = 0f;
        TotalPlayTime = 0f;
    }

#if TESTMODE
    private void Awake()
    {
        playTimer = TotalPlayTime;
    }

    private void Update()
    {
        playTimer += Time.deltaTime;

        TimeSpan span = TimeSpan.FromSeconds(playTimer);
        timeText.text = string.Format("{0}일 {1:D2}시간 {2:D2}분 {3:D2}초", span.Days, span.Hours, span.Minutes, span.Seconds);

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            obj2.SetActive(true);
            obj3.SetActive(true);
            obj4.SetActive(true);
            obj5.SetActive(true);
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            obj2.SetActive(false);
            obj3.SetActive(false);
            obj4.SetActive(false);
            obj5.SetActive(false);
        }

        // F5키 누를 시 골드 증가
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            GameManager.Instance.SetGold(5000);
        }

        // F6키 누를 시 음반 조각 증가
        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            DataManager.Instance.RecordDatabase.LpPieceCount += 1;
        }

        // F7키 누를 시 골드 초기화
        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            GameManager.Instance.SetGold(-GameManager.Instance.PlayerGold);
        }

        // F8키 누를 시 음반 조각 초기화
        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {
            DataManager.Instance.RecordDatabase.LpPieceCount = 0;
        }

        // F10키 누를 시 디버그 로그 UI 껐다키기
        if (Keyboard.current.f10Key.wasPressedThisFrame)
        {
            debugUI.SetActive(!debugUI.activeSelf);
        }
    }
#endif

    public void OnClick_ClearPlayerPrefsData()
    {
        PlayerPrefs.DeleteAll();

        PlayerPrefs.Save();
    }
    public void OnClick_ClearFirebaseData()
    {
        FirebaseMgr.Instance.FirebaseDataDelete();
    }
    public void OnClick_SaveData()
    {
        _ = DataManager.Instance.Hub.UploadAllData();
    }
    public void OnClick_LoadData()
    {
        DataManager.Instance.Hub.LoadAllData();
    }

    private void OnApplicationQuit()
    {
        TotalPlayTime = playTimer;
    }
}
