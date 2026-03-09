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

        var data = player.PlayerDataOld;


        EditorGUI.BeginChangeCheck();
        float hunger = EditorGUILayout.Slider("Hunger", data.Hunger, 0, data.MaxHunger);
        float stamina = EditorGUILayout.Slider("Stamina", data.Stamina, 0, data.MaxStamina);
        int doong = EditorGUILayout.IntField("DoongDoongStat", data.DoongDoongStat);
        float move = EditorGUILayout.FloatField("Move Speed", data.MoveSpeed);
        float fish = EditorGUILayout.FloatField("Fishing Speed", data.FishingSpeed);
        data.SetHunger(hunger);
        data.SetStamina(stamina);
        data.SetDoongDoongStat(doong);
        data.SetMoveSpeed(move);
        data.SetFishingSpeed(fish);
        if (EditorGUI.EndChangeCheck())
        {
            player.PlayerDataOld = data;
        }
    }

}

