using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ExtensionManager
{
#if UNITY_EDITOR
    // 커스텀 에디터 GUI 그리기 확장 메서드
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

    // 이미지 알파 변경 확장 메서드
    public static void SetAlpha(this Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    // 오디오클립의 길이를 '00:00' 형식으로 출력하는 확장 메서드
    public static string GetClipLength(this AudioClip clip)
    {
        int length = Mathf.RoundToInt(clip.length);

        return string.Format("{0:00}:{1:00}", length / 60, length % 60);
    }

    // 오디오 소스의 현재 재생중인 길이를 '00:00' 형식으로 출력하는 확장 메서드
    public static string GetSourceLength(this AudioSource source)
    {
        int length = Mathf.RoundToInt(source.time);

        return string.Format("{0:00}:{1:00}", length / 60, length % 60);
    }
}
