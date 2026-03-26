using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject obj2;
    [SerializeField] private GameObject obj3;
    [SerializeField] private GameObject obj4;
    [SerializeField] private GameObject obj5;

#if TESTMODE
    private void Update()
    {
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
    }
#endif

    public async void Onclick_ApplicationQuit()
    {
        await DataManager.Instance.Hub.UploadAllData();

        Application.Quit();
    }

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

}
