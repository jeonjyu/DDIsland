using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ThemeResource
{
    public FishType type;       // 테마 종류 (Lake, Sea, River 등)
    public Sprite background;   // 배경 이미지
    public string buttonText;   // 버튼에 표시될 텍스트
}

/// <summary>
/// 배경 물고기들을 관리하는 스크립트
/// 물고기를 생성하거나 초기화 하는 역할을 담당합니다
/// </summary>
public class AquariumMgr : MonoBehaviour
{
    private Queue<BackGroundFish> _fishQueue = new();
    public List<BackGroundFish> _activeFish = new();

    [Header("테마 설정")]
    [SerializeField] private FishType _currentType = FishType.Lake;
    [SerializeField] private ThemeResource[] _themes; // 테마 데이터 배열
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TMP_Text _themeButtonText;
    private Dictionary<FishType, ThemeResource> _themeDict = new();

    [Header("물고기 관련 설정")]
    [SerializeField] private BackGroundFish _fishPrefab;
    [SerializeField] private RectTransform _spawnArea;
    [SerializeField] private float _spawnPadding = 200f;
    private IMovement _boidsFish;
    private IMovement _normalFish;

    [Header("군집 관련 설정")]
    [SerializeField] int _maxFishCount;
    [SerializeField] int _minFlockSize;      // 한 무리의 최소 마리 수
    [SerializeField] int _maxFlockSize;     // 한 무리의 최대 마리 수
    private Dictionary<int, int> _flockMemberCount = new();
    private int _currentThemeIndex = 0;

    [Header("개체수")]
    [SerializeField] private float _spawnInterval;
    [SerializeField] private int _maxTotalGroups = 10; // 최대 10그룹
    private int _currentActiveGroups = 0;

    private Vector2 _lastScreenSize;
    public float ScreenLimit { get; private set; }
    public float HighLimit => (_spawnArea.rect.height / 2f);

    public RectTransform SpawnArea => _spawnArea;

    private void Awake()
    {
        _themeDict.Clear();
        foreach (var res in _themes)
        {
            if (!_themeDict.ContainsKey(res.type))
            {
                _themeDict.Add(res.type, res);
            }
        }
        InitThemeUI();
        FishMake();

        _boidsFish = new FishBoids();
        _normalFish = new FishPattern();
    }
    private void Start()
    {
        StartCoroutine(LateStart());
        AquariumBounds();


    }
    private void Update()
    {
        if (_lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
        {
            AquariumBounds();
        }
    }

    IEnumerator LateStart()
    {
        yield return null;

        StartCoroutine(RepeatSpawnFish());
    }
    public void AquariumBounds()
    {
        _lastScreenSize = new Vector2(Screen.width, Screen.height);

        float fishWidth = _fishPrefab.GetComponent<RectTransform>().rect.width;

        // 물고기가 화면 밖으로 완전히 나가는 지점 갱신
        ScreenLimit = (Screen.width / 2f) + fishWidth + _spawnPadding;
    }
    #region 호수,바다 관련
    private void InitThemeUI()
    {
        if (_themeDict.TryGetValue(_currentType, out ThemeResource res))
        {
            if (_backgroundImage != null) _backgroundImage.sprite = res.background;
            if (_themeButtonText != null) _themeButtonText.text = res.buttonText;

            // 인덱스 동기화 (Toggle 기능을 위해)
            for (int i = 0; i < _themes.Length; i++)
            {
                if (_themes[i].type == _currentType)
                {
                    _currentThemeIndex = i;
                    break;
                }
            }
        }
    }

    public void ToggleTheme()
    {
        _currentThemeIndex = (_currentThemeIndex + 1) % _themes.Length;

        FishType nextType = _themes[_currentThemeIndex].type;

        ChangeTheme(nextType);
    }

    private void ChangeTheme(FishType newType)
    {
        StopCoroutine(RepeatSpawnFish());

        // Todo: 현재 테마 변경 시 물고기들 사라지는 연출 필요

        if (_themeDict.TryGetValue(newType, out ThemeResource res))
        {
            if (_backgroundImage != null) _backgroundImage.sprite = res.background;
            if (_themeButtonText != null) _themeButtonText.text = res.buttonText;

            _currentType = newType; 
            Debug.Log($"테마 변경 완료: {res.type}");
        }

        StartCoroutine(RepeatSpawnFish());
    }
    #endregion

    #region 물고기 소환 관련
    private void FishMake()
    {
        for (int i = 0; i < _maxFishCount; i++)
        {
            BackGroundFish newFish = Instantiate(_fishPrefab, _spawnArea);
            newFish.InitManager(this);
            _fishQueue.Enqueue(newFish);
            newFish.gameObject.SetActive(false);
        }
    }
  
    public IEnumerator SpawnFlock()
    {
        if (_fishQueue.Count < _minFlockSize) yield break;

        var flockCandidates = FishManager.Instance.GetFishData(_currentType, isFlocking: true);

        if (flockCandidates == null || flockCandidates.Count == 0)
        {
            Debug.LogError($"{_currentType}에 해당하는 군집 물고기 데이터가 없습니다");
            yield break;
        }

        Debug.Log($" 후보 물고기 수: {flockCandidates.Count}");

        foreach (var f in flockCandidates)
        {
            Debug.Log($" 후보 명단: {f.FishName_String}");
        }

        FishDataSO selectedFishData = flockCandidates[Random.Range(0, flockCandidates.Count)];

        Debug.Log($"<color=green>[군집 소환]  현재 계절: {FishManager.Instance.CurrentSeason} / 뽑힌 물고기: {selectedFishData.FishName_String}</color>");

        int flockID = Random.Range(0, 1000);
        bool isRight = (Random.value > 0.5f);
        int count = Random.Range(_minFlockSize, _maxFlockSize);

        _flockMemberCount[flockID] = count;

        // 화면 내에서 랜덤 Y 위치
        float flockY = Random.Range(-_spawnArea.rect.height / 2 + 100f, _spawnArea.rect.height / 2 - 100f); 

        for (int i = 0; i < count; i++)
        {
            float SpawnX = ScreenLimit;
            float spawnX = isRight ? -SpawnX : SpawnX; // 얘들은 해상도에 따라 설정
            spawnX += Random.Range(-50f, 50f); // 약간의 랜덤 위치 보정
            float spawnY = flockY + Random.Range(-50f, 50f); // 무리 내에서 약간의 Y 위치 보정

            Vector2 spawnPos = new(spawnX, spawnY);

            SpawnFish(spawnPos, selectedFishData ,fishID: flockID, isRight: isRight, movement: _boidsFish);

            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator SpawnSingle()
    {
        var singleCandidates = FishManager.Instance.GetFishData(_currentType, isFlocking: false);

        if (singleCandidates == null || singleCandidates.Count == 0)
        {
            Debug.LogError($"{_currentType}에 해당하는 일반 물고기 데이터가 없습니다");
            yield break;
        }

        FishDataSO selectedData = singleCandidates[Random.Range(0, singleCandidates.Count)];

        Debug.Log($"<color=yellow>[단일 소환] 현재 계절: {FishManager.Instance.CurrentSeason} / 뽑힌 물고기: {selectedData.FishName_String}</color>");

        int fishID = Random.Range(10000, 99999);
        _flockMemberCount[fishID] = 1;

        bool isRight = (Random.value > 0.5f);

        float spawnX = isRight ? -ScreenLimit : ScreenLimit;
        float spawnY = Random.Range(-_spawnArea.rect.height / 2 + 100f, _spawnArea.rect.height / 2 - 100f);

        SpawnFish(new Vector2(spawnX, spawnY), selectedData, fishID: fishID, isRight: isRight, movement: _normalFish);

        yield return null;
    }
    public void SpawnFish(Vector2 spawnPosition, FishDataSO data, int fishID = -1, bool isRight = true, IMovement movement = null)
    {
        if (_fishQueue.Count > 0)
        {
            BackGroundFish fish = _fishQueue.Dequeue();

            fish.FishTransform.anchoredPosition = spawnPosition;
            fish.gameObject.SetActive(true);
            fish.SetSpawn(data, fishID, isRight, movement);
            _activeFish.Add(fish);
        }
    }
    private IEnumerator RepeatSpawnFish()
    {
        while (true)
        {
            // 무조건 화면 밖에서 생성
            if (_currentActiveGroups < _maxTotalGroups)
            {
                // 50% 확률로 군집 또는 일반 물고기 결정
                if (Random.value > 0.5f)
                {
                    StartCoroutine(SpawnFlock()); // 군집 소환
                }
                else
                {
                    StartCoroutine(SpawnSingle()); // 일반 물고기 소환
                }
                _currentActiveGroups++; // 그룹 카운트 증가
            }
            // 다음 무리 생성 대기시간
            yield return new WaitForSeconds(_spawnInterval);
        }
    }
    public void ReturnFish(BackGroundFish fish)
    {
        // 물고기가 비활성화 되면 리스트에서 제거하고 큐에 다시 넣음
        if (_activeFish.Contains(fish))
        {
            int myID = fish._floackID;
            _activeFish.Remove(fish);
            _fishQueue.Enqueue(fish);

            if (_flockMemberCount.ContainsKey(myID))
            {
                _flockMemberCount[myID]--;

                // 팀원이 0이 되면 슬롯 반납
                if (_flockMemberCount[myID] <= 0)
                {
                    _flockMemberCount.Remove(myID);
                    _currentActiveGroups--;
                }
            }
        }
    }
    #endregion


    // 물고기 숨기기 
    public void HideFish() 
    {

         StopAllCoroutines();
        // 투명도 0 보이지 않게 
        var cg = _spawnArea.GetComponent<CanvasGroup>();
        if (cg == null) cg = _spawnArea.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
    }
    // 물고기 보이기 
    public void ShowFish() 
    {
        // 물고기 투명도 1 (보이는 상태로)
        var cg = _spawnArea.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 1f;
        StartCoroutine(RepeatSpawnFish());
    }
}
