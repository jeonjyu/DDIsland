using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// [호수 편집 모드] 타일 그리드 생성 및 관리
public class LakeGridManager : MonoBehaviour
{
    [Header("그리드 설정")]
    public int gridWidth = 20;   // 가로 타일 수 (나중에 바꿀 수 있음)
    public int gridHeight = 2;   // 세로 타일 수

    [Header("타일 색상")]
    public Color emptyColor = new Color(1f, 1f, 1f, 0.4f);       // 흰색, 반투명
    public Color fixedColor = new Color(1f, 0f, 0f, 0.6f);       // 빨간색
    public Color occupiedColor = new Color(0.5f, 0.5f, 0.5f, 0.6f); // 회색
    public Color previewColor = new Color(0f, 1f, 0f, 0.5f);     // 초록색
    public Color invalidColor = new Color(1f, 0f, 0f, 0.8f);     // 진한 빨간색

    [Header("프리팹 (없으면 코드로 생성)")]
    public GameObject tilePrefab;  // 타일재
    public GameObject itemPrefab;  // 배치템 

    //  내부 변수 
    LakeTileData[,] tileDataArray;        // 타일 데이터 2차원 배열
    GameObject[,] tileObjectArray;        // 타일 게임오브젝트 2차원 배열
    List<LakePlacedObjectData> placedObjects = new List<LakePlacedObjectData>();
    Dictionary<string, GameObject> placedVisuals = new Dictionary<string, GameObject>(); // 배치된 실제 오브젝트 관리
    List<GameObject> blockedIcons = new List<GameObject>();
    public GameObject blockedIconPrefab; // 접근금지 이미지 프리팹 
    int placedIdCounter = 0; // 배치 ID
    List<LakePlacedObjectData> gridSnapshot = null; // 배치 스냅샷 

    // 타일 크기 (부모에 맞게 자동 계산, 가로세로 다를 수 있음 = 직사각형)
    float tileWidth;
    float tileHeight;

    // 중앙 정렬용 시작 오프셋
    float offsetX;
    float offsetY;
    float currentGap;  // 현재 격자 간격 (0이면 격자 안 보임)

    // 초기화
    void Start()
    {
        CreateGrid();
        SetupTest();
        HideGrid();
    }

    // 그리드 생성 (부모 크기를 타일 수로 나눠서 자동 계산)
    public void CreateGrid()
    {
        // 부모(GridArea) 크기 가져오기
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        float areaWidth = parentRect.rect.width;
        float areaHeight = parentRect.rect.height;

        // 타일 크기 = 부모 크기 / 타일 수 (가로세로 따로 계산, 직사각)
        tileWidth = areaWidth / gridWidth;
        tileHeight = areaHeight / gridHeight;

        // 전체 그리드 크기 (= 부모 크기와 같아짐)
        float totalGridWidth = tileWidth * gridWidth;
        float totalGridHeight = tileHeight * gridHeight;

        // GridContainer를 부모 중앙에 맞추기
        RectTransform myRect = GetComponent<RectTransform>();
        myRect.anchorMin = new Vector2(0.5f, 0.5f);
        myRect.anchorMax = new Vector2(0.5f, 0.5f);
        myRect.pivot = new Vector2(0.5f, 0.5f);
        myRect.anchoredPosition = Vector2.zero;
        myRect.sizeDelta = new Vector2(totalGridWidth, totalGridHeight);

        // 타일 오프셋
        offsetX = -totalGridWidth / 2f;
        offsetY = -totalGridHeight / 2f;

        // 배열 초기화
        tileDataArray = new LakeTileData[gridWidth, gridHeight];
        tileObjectArray = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // 타일 데이터 생성 
                LakeTileData data = new LakeTileData();
                data.gridPos = new Vector2Int(x, y);
                data.state = LakeTileState.Empty;
                data.placedObjectId = "";
                tileDataArray[x, y] = data;

                // 타일 오브젝트 생성 
                GameObject tileObj = MakeTileObject(x, y);
                tileObj.transform.SetParent(this.transform, false);
                tileObjectArray[x, y] = tileObj;

                // 위치 시작점 (직사각형 타일)
                RectTransform rt = tileObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0f, 0f);  // 좌하단 기준
                rt.sizeDelta = new Vector2(tileWidth, tileHeight); // 가로세로 다를 수 있음
                rt.anchoredPosition = new Vector2(
                    offsetX + (x * tileWidth),
                    offsetY + (y * tileHeight)
                );

                // 색상 적용 (처음엔 투명해서 격자 안보이게) 
                Image img = tileObj.GetComponent<Image>();
                img.color = new Color(0, 0, 0, 0);
            }
        }
    }

    // 타일 오브젝트 하나 생성
    GameObject MakeTileObject(int x, int y)
    {
        GameObject tileObj;

        if (tilePrefab != null)
        {
            tileObj = Instantiate(tilePrefab);
        }
        else // 타일 프리팹 없으면 그냥 코드로 만듦
        {
            tileObj = new GameObject("Tile_" + x + "_" + y);
            tileObj.AddComponent<RectTransform>();
            Image img = tileObj.AddComponent<Image>();
            img.color = emptyColor;
        }

        tileObj.name = "Tile_" + x + "_" + y;
        return tileObj;
    }

    // 타일 색상을 상태에 따라 
    public void UpdateTileColor(int x, int y)
    {
        if (IsOutOfBounds(x, y)) return;

        Image img = tileObjectArray[x, y].GetComponent<Image>();
        LakeTileState state = tileDataArray[x, y].state;

        switch (state)
        {
            case LakeTileState.Empty:
                img.color = emptyColor;
                break;
            case LakeTileState.Fixed:
                img.color = fixedColor;
                break;
            case LakeTileState.Occupied:
                img.color = occupiedColor;
                break;
            case LakeTileState.Preview:
                img.color = previewColor;
                break;
            case LakeTileState.Invalid:
                img.color = invalidColor;
                break;
        }
    }

    // 타일 전부 색깔 새로고침
    public void RefreshAllTileColors()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                UpdateTileColor(x, y);
            }
        }
    }

    //  배치 가능 여부 확인
    public bool CanPlace(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                // 그리드 밖이면 불가
                if (IsOutOfBounds(x, y)) return false;

                // 비어있지 않으면 불가
                if (tileDataArray[x, y].state != LakeTileState.Empty)
                    return false;
            }
        }
        return true;
    }

    //  배치 미리보기 (초록/빨강 표시)
    public void ShowPreview(int startX, int startY, int sizeX, int sizeY)
    {
        // 이전 미리보기 지우기
        ClearPreview();

        bool canPlace = CanPlace(startX, startY, sizeX, sizeY);

        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                if (IsOutOfBounds(x, y)) continue;

                // 빈 타일만 미리보기 색상 변경 (이미 뭐 있는 타일은 건드리지 않음)
                if (tileDataArray[x, y].state == LakeTileState.Empty)
                {
                    tileDataArray[x, y].state = canPlace ? LakeTileState.Preview : LakeTileState.Invalid;
                    UpdateTileColor(x, y);
                }
                else if (tileDataArray[x, y].state == LakeTileState.Fixed)
                {
                    // 접근 금지 표시
                    if (blockedIconPrefab != null)
                    {
                        GameObject icon = Instantiate(blockedIconPrefab, tileObjectArray[x, y].transform);
                        blockedIcons.Add(icon);
                    }
                }
            }
        }
    }

    // 미리보기 색상 전부 지우기 (Empty로)
    public void ClearPreview()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (tileDataArray[x, y].state == LakeTileState.Preview ||
                    tileDataArray[x, y].state == LakeTileState.Invalid)
                {
                    tileDataArray[x, y].state = LakeTileState.Empty;
                    UpdateTileColor(x, y);
                }
            }
        }
        for (int i = 0; i < blockedIcons.Count; i++)
            Destroy(blockedIcons[i]);
        blockedIcons.Clear(); // 접근금지 아이콘 지우기
    }

    // 오브젝트 배치 (타일 상태 변경 + 오브젝트 생성)
    public bool PlaceObject(int itemId, int startX, int startY, int sizeX, int sizeY)
    {
        // 놓을 수 있는지 한번 더 확인
        if (!CanPlace(startX, startY, sizeX, sizeY))
            return false;

        // 미리보기 지우기
        ClearPreview();

        // 배치 ID 만들기
        string objectId = "lake_obj_" + placedIdCounter;
        placedIdCounter++;

        // 타일 상태 변경
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                tileDataArray[x, y].state = LakeTileState.Occupied;
                tileDataArray[x, y].placedObjectId = objectId;
                UpdateTileColor(x, y);
            }
        }

        // 배치 데이터 저장
        LakePlacedObjectData placed = new LakePlacedObjectData();
        placed.objectId = objectId;
        placed.itemId = itemId;
        placed.gridPos = new Vector2Int(startX, startY);
        placed.size = new Vector2Int(sizeX, sizeY);
        placedObjects.Add(placed);

        // 오브젝트 생성
        GameObject visual = CreatePlacedVisual(placed);
        placedVisuals.Add(objectId, visual);

        return true;
    }

    // 배치 오브젝트 생성 
    GameObject CreatePlacedVisual(LakePlacedObjectData data)
    {
        GameObject obj;

        if (itemPrefab != null) // 인스펙터에 프리팹 넣으면 프리팹 생성 
        {
            // TODO: 실제 스프라이트 로드 (나중에 ObjectTable 연동)
            obj = Instantiate(itemPrefab);
            obj.name = "Placed_" + data.objectId;
            obj.transform.SetParent(this.transform, false);
        }
        else  // 프리팹 없을시 더미 
        {
            obj = new GameObject("Placed_" + data.objectId);
            obj.transform.SetParent(this.transform, false);
            obj.AddComponent<RectTransform>();

            Image img = obj.AddComponent<Image>();
            Sprite sprite = LakeDecoTestData.GetIconSprite(data.itemId);
            if (sprite != null) 
            {
                img.sprite = sprite;
                img.color = Color.white;
            }
            else // 폴백할 스프라이트도 없으면 랜덤색 타일
            {
                img.color = GetDummyItemColor(data.itemId);
            }

            img.raycastTarget = true;
        }

        // 위치와 크기 세팅 (프리팹이든 더미든 동일)
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0f, 0f);

        float objWidth = tileWidth * data.size.x;
        float objHeight = tileHeight * data.size.y;
        rt.sizeDelta = new Vector2(objWidth, objHeight);

        rt.anchoredPosition = new Vector2(
            offsetX + (data.gridPos.x * tileWidth),
            offsetY + (data.gridPos.y * tileHeight)
        );
     
        return obj;
    }

    // 오브젝트 회수 (배치 ID로)
    public bool RecallObject(string objectId)
    {
        // 배치 목록에서 찾기
        LakePlacedObjectData found = null;
        for (int i = 0; i < placedObjects.Count; i++)
        {
            if (placedObjects[i].objectId == objectId)
            {
                found = placedObjects[i];
                placedObjects.RemoveAt(i);
                break;
            }
        }

        if (found == null) return false; // id 못찾으면 

        // 타일 상태 비우기
        for (int x = found.gridPos.x; x < found.gridPos.x + found.size.x; x++)
        {
            for (int y = found.gridPos.y; y < found.gridPos.y + found.size.y; y++)
            {
                if (IsOutOfBounds(x, y)) continue;

                tileDataArray[x, y].state = LakeTileState.Empty;
                tileDataArray[x, y].placedObjectId = "";
                UpdateTileColor(x, y);
            }
        }

        // 오브젝트 삭제 
        if (placedVisuals.ContainsKey(objectId))
        {
            Destroy(placedVisuals[objectId]);
            placedVisuals.Remove(objectId);
        }

        return true;
    }

    // 회수된 오브젝트의 itemId 가져오기 (인벤 템갯수 복구용)
    public int GetItemIdByObjectId(string objectId)
    {
        for (int i = 0; i < placedObjects.Count; i++)
        {
            if (placedObjects[i].objectId == objectId)
                return placedObjects[i].itemId;
        }
        return -1;
    }

    // 전체 회수 (고정물 제외)
    public void RecallAll()
    {
        // 뒤에서부터 지워야 인덱스가 안 꼬임
        for (int i = placedObjects.Count - 1; i >= 0; i--)
        {
            RecallObject(placedObjects[i].objectId);
        }
    }

    // 특정 타일을 고정(Fixed) 상태로 고정물 설정 (LakeFix 오브젝트용)
    public void SetFixed(int x, int y)
    {
        if (IsOutOfBounds(x, y)) return;

        tileDataArray[x, y].state = LakeTileState.Fixed;
        tileDataArray[x, y].placedObjectId = "fixed";
        UpdateTileColor(x, y);
    }

    /// 테스트용: 더미 고정물 배치 
    public void SetupTest()
    {
        SetFixed(5, 0);
        SetFixed(5, 1);
        SetFixed(15, 0);
        PlaceObject(1001, 8, 0, 2, 2);
      //  ShowPreview(12, 0, 2, 1);
    }

    // 격자 보이기 (편집 모드 ON) // 이미지에 격자가 있는게 아니라, 중간중간 띄워서 백그라운드가 보이게 하는 방식
    public void ShowGrid(float gap = 2f)
    {
        currentGap = gap;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                RectTransform rt = tileObjectArray[x, y].GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileWidth - gap, tileHeight - gap); // gap만큼 타일 줄여서 틈 생성
                UpdateTileColor(x, y);
            }
        }

        // 배치된 오브젝트 보이기
        //foreach (var visual in placedVisuals.Values)
        //{
        //    if (visual != null) visual.SetActive(true);
        //}
    }

    // 격자 숨기기 (편집 모드 OFF)
    public void HideGrid()
    {
        ClearPreview(); // 미리보기 정리 

        currentGap = 0f; 

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                RectTransform rt = tileObjectArray[x, y].GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileWidth, tileHeight); // gap 없앰

                // 투명하게
                Image img = tileObjectArray[x, y].GetComponent<Image>();
                img.color = new Color(0, 0, 0, 0);
            }
        }
        // TODO : 이 구문은 나중에 섬쪽 편집모드 들어가면 타일 안보이게 가져다 쓰면 될 듯
        //// 배치된 오브젝트 숨기기 (편집 모드 아닐 땐 안 보이게) 
        //foreach (var visual in placedVisuals.Values)
        //{
        //    if (visual != null) visual.SetActive(false);
        //}
    }

    // 마우스 위치를 그리드 좌표 변환
    public Vector2Int MouseToGridPos()
    {
        Vector2 localPos;
        RectTransform rt = GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt, Mouse.current.position.ReadValue(), null, out localPos);

        // 중앙 기준으로 오프셋 보정
        float adjustedX = localPos.x - offsetX;
        float adjustedY = localPos.y - offsetY;

        int gridX = Mathf.FloorToInt(adjustedX / tileWidth);
        int gridY = Mathf.FloorToInt(adjustedY / tileHeight);

        return new Vector2Int(gridX, gridY);
    }

    // 클릭한 위치의 배치 오브젝트 ID 가져오기
    public string GetObjectIdAtMouse()
    {
        Vector2Int pos = MouseToGridPos();
        if (IsOutOfBounds(pos.x, pos.y)) return "";
        return tileDataArray[pos.x, pos.y].placedObjectId;
    }
    #region 스냅샷 
    // 배치 스냅샷 저장 
    public void SaveSnapshot()
    {
        gridSnapshot = new List<LakePlacedObjectData>();
        for (int i = 0; i < placedObjects.Count; i++)
        {
            var src = placedObjects[i];
            gridSnapshot.Add(new LakePlacedObjectData
            {
                objectId = src.objectId,
                itemId = src.itemId,
                gridPos = src.gridPos,
                size = src.size
            });
        }
    }
  
    //  배치 스냅샷으로 복원 
    public void LoadSnapshot()
    {
        // 현재 배치된 거 전부 제거
        RecallAll();

        if (gridSnapshot == null) return;

        // 스냅샷에서 다시 배치
        for (int i = 0; i < gridSnapshot.Count; i++)
        {
            var snap = gridSnapshot[i];
            PlaceObject(snap.itemId, snap.gridPos.x, snap.gridPos.y, snap.size.x, snap.size.y);
        }
    }
    #endregion  
    public Vector2 GridToSnapPos(int gx, int gy, int sizeX, int sizeY)
    {
        return new Vector2(
            offsetX + (gx * tileWidth) + (sizeX * tileWidth * 0.5f),
            offsetY + (gy * tileHeight) + (sizeY * tileHeight * 0.5f)
        );
    }
    // 헬퍼 함수
    // 그리드 범위 밖인지 확인
    public bool IsOutOfBounds(int x, int y)
    {
        if (x < 0) return true;            // 왼쪽 밖
        if (x >= gridWidth) return true;    // 오른쪽 밖
        if (y < 0) return true;            // 아래쪽 밖
        if (y >= gridHeight) return true;  // 위쪽 밖

        return false;
    }

    // 특정 좌표의 타일 데이터 가져오기
    public LakeTileData GetTileData(int x, int y)
    {
        if (IsOutOfBounds(x, y)) return null;
        return tileDataArray[x, y];
    }
    public GameObject GetVisual(string objectId)
    {
        return placedVisuals.ContainsKey(objectId) ? placedVisuals[objectId] : null;
    }
    /// 배치된 오브젝트 목록 가져오기 (저장용)
    public List<LakePlacedObjectData> GetPlacedObjects()
    {
        return placedObjects;
    }

    // 더미 오브젝트 
    Color GetDummyItemColor(int itemId)
    {
        switch (itemId % 7)
        {
            case 0: return new Color(0.2f, 0.7f, 0.3f, 0.85f);  
            case 1: return new Color(0.6f, 0.5f, 0.4f, 0.85f);  
            case 2: return new Color(0.9f, 0.5f, 0.6f, 0.85f);  
            case 3: return new Color(0.5f, 0.5f, 0.5f, 0.85f);  
            case 4: return new Color(0.3f, 0.8f, 0.5f, 0.85f);  
            case 5: return new Color(0.4f, 0.6f, 0.8f, 0.85f);  
            case 6: return new Color(0.7f, 0.4f, 0.9f, 0.85f);  
            default: return new Color(0.8f, 0.8f, 0.8f, 0.85f);
        }
    }

    public float GetTileWidth() { return tileWidth; }
    public float GetTileHeight() { return tileHeight; }
    // 그리드 크기 가져오기, 배치 오브젝트가 그리드 영역 밖으로 못벗어나게 막기용 
    public int GetGridWidth() { return gridWidth; }
    public int GetGridHeight() { return gridHeight; }
}