using UnityEditor;
using UnityEngine;

/// <summary>
/// DataManager 프리팹, LocalizationManager 프리팹을 받아와 맞는 SO를 자동으로 직렬화 시켜주는 에디터 툴 창
/// </summary>
public class SerializeDataWindow : EditorWindow
{
    #region 실제 프리팹
    private GameObject dataManagerPrefab;                                           // DataManager 프리팹
    private GameObject localizationManagerPrefab;                                   // LocalizationManager 프리팹

    private const string DATAPREFABKEY = "DataManagerKey";                          // DataManager EditorPrefs 키
    private const string LOCALIZATIONPREFABKEY = "LocalizationManagerKey";          // LocalizationManager EditorPrefs 키

    private string dataManagerPath;                                                 // DataManager 프리팹 경로
    private string localizationManagerPath;                                         // LocalizationManager 프리팹 경로
    #endregion

    #region CSV가 들어있는 폴더
    private DefaultAsset databaseSOFolder;                                          // 데이터베이스 SO 폴더
    private DefaultAsset stringSOFolder;                                            // 스트링 SO 폴더

    private const string DATABASESOKEY = "DatabaseSOKey";                           // 데이터베이스 SO 폴더 키
    private const string STRINGSOKEY = "StringSOKey";                               // StringSO 폴더 키

    private string databaseSOPath = "Assets/1.Prefabs/SO/Database";                 // 데이터베이스 SO 폴더 경로
    private string stringDataSOPath = "Assets/1.Prefabs/SO/StringDataSO";           // StringSO 폴더 경로

    #endregion

    [MenuItem("Cooperation Tool/Data/Data Auto Binder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SerializeDataWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        dataManagerPrefab = (GameObject)EditorGUILayout.ObjectField("Manager Prefab", dataManagerPrefab, typeof(GameObject), false);
        databaseSOPath = EditorGUILayout.TextField("SO Folder Path", databaseSOPath);
    }

    private void BindingToPrefab()
    {

    }
}
