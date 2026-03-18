using UnityEngine;

public class Test : MonoBehaviour
{
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
