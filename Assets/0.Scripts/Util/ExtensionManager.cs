using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ExtensionManager
{
#if UNITY_EDITOR
    public static void DrawGUI(this DefaultAsset folderAsset, string label, string path, string key)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        folderAsset = (DefaultAsset)EditorGUILayout.ObjectField(
            path,
            folderAsset,
            typeof(DefaultAsset),
            false
            );

        if (folderAsset == null)
        {
            path = null;
            EditorPrefs.DeleteKey(key);
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(folderAsset);

        if (!AssetDatabase.IsValidFolder(assetPath))
        {
            EditorGUILayout.HelpBox("폴더만 지정할 수 있습니다.", MessageType.Error);
            EditorPrefs.DeleteKey(key);
            return;
        }

        path = assetPath;
        EditorPrefs.SetString(key, assetPath);
    }
#endif

    public static void SetAlpha(this Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
