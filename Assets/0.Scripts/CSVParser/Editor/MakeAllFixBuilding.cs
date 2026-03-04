#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 씬에 있는 마커를 참조하여 고정 장식물들을 배치할 수 있게 해주는 에디터입니다
/// 마커에 ID를 직접입력하여 원하는 프리팹을 들고 올 수 있습니다
/// </summary>
public class MakeAllFixBuilding : EditorWindow
{
    private Vector2 _scrollPos; // 스크롤
    private GridSystem _gridSystem;
    private BuildingManager _buildManager;
    private DataManager _dataManager;

    [MenuItem("Tools/고정 장식물 배치/Bake Fix Building")]
    public static void ShowWindow()
    {
        MakeAllFixBuilding window = GetWindow<MakeAllFixBuilding>("Bake Fix Building");
        window.minSize = new Vector2(350, 400);
    }

    private void OnGUI()
    {
        DrawHeader();

        EditorGUILayout.BeginVertical("helpbox");
        {
            GUILayout.Label("필수 시스템 설정", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            // 안내 문구
            if (_gridSystem == null || _buildManager == null || _dataManager == null)
            {
                EditorGUILayout.HelpBox("아래 슬롯에 씬의 [GridSystem], [BuildingManager],[DataManager]를 드래그 앤 드롭하세요.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("시스템 연결 완료. 이제 설치를 진행할 수 있습니다.", MessageType.Info);
            }

            // 슬롯 배치
            _gridSystem = (GridSystem)EditorGUILayout.ObjectField("Grid System", _gridSystem, typeof(GridSystem), true);
            _buildManager = (BuildingManager)EditorGUILayout.ObjectField("Build Manager", _buildManager, typeof(BuildingManager), true);
            _dataManager = (DataManager)EditorGUILayout.ObjectField("Data Manager", _dataManager, typeof(DataManager), true);
        }
        EditorGUILayout.EndVertical();

        if (FixMaker.AllMarkers.Count == 0)
        {
            EditorGUILayout.HelpBox("등록된 마커가 없습니다. 씬을 새로고침 하거나 마커를 클릭해 주세요.", MessageType.Info);
        }

        DrawMarkerStats();
        DrawActionButtons();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("고정 장식물 배치", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


    }
    private void DrawMarkerStats()
    {
        var allMarkers = FixMaker.AllMarkers;

        EditorGUILayout.BeginVertical("box");

        
        
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(250));

            // ID별로 그룹화
            var groups = allMarkers.GroupBy(m => m._interiorId).OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                EditorGUILayout.BeginHorizontal("button"); // 각 줄을 버튼 느낌으로 감싸기
                GUILayout.Label($"ID {group.Key}", GUILayout.Width(60)); // 어떤게 있는지
                GUILayout.Label($": {group.Count()}개 배치됨"); //혹시 나중에 여러개 일 수도 있으니까

                // 해당 마커가 어딨는지 씬뷰 카메라로 띄워줌
                if (GUILayout.Button("찾기", GUILayout.Width(50)))
                {
                    Selection.activeGameObject = group.First().gameObject;
                    SceneView.FrameLastActiveSceneView();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.Space(15);

        GUI.color = Color.cyan; // 버튼 강조 색상
        if (GUILayout.Button("모든 마커를 프리팹으로 교체", GUILayout.Height(40)))
        {
            // 확인 팝업창
            if (EditorUtility.DisplayDialog("베이크 확인",
                "모든 마커를 실제 프리팹으로 교체하시겠습니까? \n(마커는 삭제되지만 Ctrl+Z로 되돌릴 수 있습니다.)", "진행", "취소"))
            {
                BakeAll();
            }
        }
        GUI.color = Color.white;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("베이크 이후 Ctrl + Z로 되돌릴 수 있습니다.",MessageType.Info);
    }

    private void BakeAll()
    {
        // SO로드
        var database = _dataManager.DecorationDatabase;

        if (database == null || _gridSystem == null || _buildManager == null)
        {
            Debug.LogError("필수 시스템(DB, Grid, BuildManager) 중 일부를 찾을 수 없습니다.");
            return;
        }

        // 리스트를 복사해서 에러 방지 (얕은 복사)
        var AllMakers = FixMaker.AllMarkers.ToList();
        int count = 0;


        EditorApplication.LockReloadAssemblies();

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Bake All Fix Buildings");
        int undoGroup = Undo.GetCurrentGroup();
        try
        {

            foreach (var marker in AllMakers)
            {
                // 마커에 있는 ID를 참조해서 SO 데이터로 변환
                var data = database.InteriorData[marker._interiorId];

                if (data == null)
                {
                    Debug.LogWarning($"ID {marker._interiorId}번에 해당하는 데이터를 찾을 수 없어 건너뜁니다.");
                    continue;
                }

                if (data.interior_itemType != Interior_ItemType.Fix)
                {
                    Debug.LogWarning($"ID {marker._interiorId}번 ({data.InteriorName_String}). Fix 타입이 아닙니다. 마커를 확인하세요.");
                    continue;
                }

                // 마커를 프리팹으로 교체
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(data.InteriorPath_GameObject, _gridSystem.transform);
                instance.transform.SetPositionAndRotation(marker.transform.position, marker.transform.rotation);

                Undo.RegisterCreatedObjectUndo(instance, "Bake Building");

                if (!instance.TryGetComponent(out Placeable3D placeable))
                {
                    placeable = Undo.AddComponent<Placeable3D>(instance);
                }
                if (placeable == null)
                {
                    Debug.LogError($"{instance.name}에 Placeable3D를 붙이는 데 실패했습니다!");
                    continue;
                }

                // Placeable3D 초기화
                if (placeable != null)
                {
                    // 스크립트에 데이터 주입 (BuildingManager와 비슷한 방식)
                    placeable.Initialize(_gridSystem, _buildManager, data);

                    placeable.ItemState = ItemState.Placed;

                    Vector2Int centerIndex = _gridSystem.GetGridIndex(marker.transform.position);

                    int px = Mathf.RoundToInt((data.GridSizeX - 1) / 2f);
                    int py = Mathf.RoundToInt((data.GridSizeY - 1) / 2f);

                    Vector2Int originIndex = centerIndex - new Vector2Int(px, py);
                    Vector2Int size = new (data.GridSizeX, data.GridSizeY);

                    placeable.SetBakeData(originIndex, size);

                    EditorUtility.SetDirty(instance);
                    EditorUtility.SetDirty(placeable);

                    PrefabUtility.RecordPrefabInstancePropertyModifications(placeable);

                    if (!Application.isPlaying)
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(instance.scene);
                    }
                }

                Undo.DestroyObjectImmediate(marker.gameObject);
                count++;
            }
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"실행 중 오류 발생: {e.Message}");
            Debug.LogError($"<color=red><b>[Bake 실패!]</b></color> 에러 내용: {e.Message}\n" +
                   $"<color=yellow><b>[상세 위치 (StackTrace)]</b></color>\n{e.StackTrace}");
        }
        finally
        {
            Undo.CollapseUndoOperations(undoGroup);
            EditorApplication.UnlockReloadAssemblies();
        }

        Debug.Log($" {count}개의 고정 장식물이 씬에 성공적으로 구워졌습니다!");
    }
}
#endif