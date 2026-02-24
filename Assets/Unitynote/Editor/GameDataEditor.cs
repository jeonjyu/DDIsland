using UnityEditor;
using UnityEngine;

public class GameDataEditor : EditorWindow
{
    private PlayerController player;
    private static GameDataEditor window;
    [MenuItem("Window/Game Data Editor")]
    private static void Setup()
    {
        window = GetWindow<GameDataEditor>();

        window.titleContent = new GUIContent("Game Data Editor");

        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(1920, 1080);
    }

    private void OnGUI()
    {
        GUILayout.Label("Runtime Player Data", EditorStyles.boldLabel);
        if (player == null)
        {
            if (GUILayout.Button("Find Player"))
            {
                player = FindAnyObjectByType<PlayerController>();
            }
            return;
        }

        var PlayerData = player.PlayerData;

        if (PlayerData == null) return;

        PlayerData.SetHunger(EditorGUILayout.Slider("Hunger", PlayerData.Hunger, 0, 100));
        PlayerData.SetStamina(EditorGUILayout.Slider("Stamina", PlayerData.Stamina, 0, 100));
        PlayerData.SetDoongDoongStat(EditorGUILayout.IntField("DoongDoongStat", PlayerData.DoongDoongStat));
        PlayerData.SetMoveSpeed(EditorGUILayout.FloatField("Move Speed", PlayerData.MoveSpeed));
        PlayerData.SetFishingSpeed(EditorGUILayout.FloatField("Fishing Speed", PlayerData.FishingSpeed));


    }

}

