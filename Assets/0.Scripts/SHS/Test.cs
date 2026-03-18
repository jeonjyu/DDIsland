using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    private void Update()
    {
        // F5키 누를 시 골드 증가
        if(Keyboard.current.f5Key.wasPressedThisFrame)
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

    public void Onclick_ApplicationQuit()
    {
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
        DataManager.Instance.Hub.UploadAllData();
    }
    public void OnClick_LoadData()
    {
        DataManager.Instance.Hub.LoadAllData();
    }

}
