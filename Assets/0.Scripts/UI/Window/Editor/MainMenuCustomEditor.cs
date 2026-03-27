using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UI_MainMenu))]
public class MainMenuCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기존 인스펙터 내용 표시
        base.OnInspectorGUI();

        UI_MainMenu script = (UI_MainMenu)target;

        RectTransform rect = script.GetComponent<RectTransform>();

        GUIStyle style = new GUIStyle();
        style.padding = new RectOffset(10, 10, 5, 5);

        EditorGUILayout.BeginHorizontal(style);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Show 상태 포지션 지정", GUILayout.Width(200), GUILayout.Height(20)))
        {
            script.SetShowPos(rect.anchoredPosition);
        }

        if (GUILayout.Button("Hide 상태 포지션 지정", GUILayout.Width(200), GUILayout.Height(20)))
        {
            script.SetHidePos(rect.anchoredPosition);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}
