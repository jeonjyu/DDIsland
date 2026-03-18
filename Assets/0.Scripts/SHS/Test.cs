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
        if (DataManager.Instance == null || DataManager.Instance.Hub == null)
        {
            Debug.LogError("DataManager 또는 Hub가 없습니다!");
            return;
        }

        DataManager.Instance.Hub._allUserData = new UserAllData();

        DataManager.Instance.Hub.SaveAllData();

        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.playerItemDatas.Clear();
        }

        var buildingMgr = FindFirstObjectByType<BuildingManager>();
        if (buildingMgr != null)
        {
            buildingMgr.ClearAll();
        }
    }

}
