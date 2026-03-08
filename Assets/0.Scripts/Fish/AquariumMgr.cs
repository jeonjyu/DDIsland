using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배경 물고기들을 관리하는 스크립트
/// 물고기를 생성하거나 초기화 하는 역할을 담당합니다
/// </summary>
public class AquariumMgr : MonoBehaviour
{
    private Queue<BackGroundFish> _fishQueue = new();
    public List<BackGroundFish> _activeFish = new();

    [SerializeField] private BackGroundFish _fishPrefab;
    [SerializeField] private RectTransform _spawnArea;
    [SerializeField] int _maxFishCount = 100;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] int _minFlockSize = 15;      // 한 무리의 최소 마리 수
    [SerializeField] int _maxFlockSize = 30;     // 한 무리의 최대 마리 수
    [SerializeField] private float _spawnPadding = 200f;


    private Vector2 _lastScreenSize;
    public float ScreenLimit { get; private set; }
    public float HighLimit { get; private set; }

    public RectTransform SpawnArea => _spawnArea;

    private void Awake()
    {
        FishMake();
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

        // 수조 높이에 따른 이동 제한 구역 갱신
        HighLimit = (_spawnArea.rect.height / 2f) * 0.9f;
    }

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
  
    public IEnumerator SpawnEdge()
    {
        if (_fishQueue.Count < _minFlockSize) yield break;

        var flockCandidates = FishManager.Instance.GetFishData(FishType.Lake, isFlocking: true);

        Debug.Log($"[AquariumMgr] 후보 물고기 수: {flockCandidates.Count}");

        foreach (var f in flockCandidates)
        {
            Debug.Log($"[AquariumMgr] 후보 명단: {f.FishName_String}");
        }

        FishDataSO selectedFishData = flockCandidates[Random.Range(0, flockCandidates.Count)];

        Debug.Log($"새 무리 생성: {selectedFishData.FishName_String}");

        int flockID = Random.Range(0,10);
        bool isRight = (flockID % 2 == 0);

        int count = Random.Range(_minFlockSize, _maxFlockSize);

        // 화면 내에서 랜덤 Y 위치
        float flockY = Random.Range(-_spawnArea.rect.height / 2 + 100f, _spawnArea.rect.height / 2 - 100f); 

        for (int i = 0; i < count; i++)
        {
            float SpawnX = ScreenLimit;
            // Todo: 해상도를 변수로 받아서 +- 20~30정도
            float spawnX = isRight ? -SpawnX : SpawnX; // 얘들은 해상도에 따라 설정
            spawnX += Random.Range(-50f, 50f); // 약간의 랜덤 위치 보정
            float spawnY = flockY + Random.Range(-50f, 50f); // 무리 내에서 약간의 Y 위치 보정

            Vector2 spawnPos = new(spawnX, spawnY);

            SpawnFish(spawnPos, selectedFishData ,fishID: flockID, isRight: isRight);

            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SpawnFish(Vector2 spawnPosition, FishDataSO data, int fishID = -1, bool? isRight = null)
    {
        if (_fishQueue.Count > 0)
        {
            BackGroundFish fish = _fishQueue.Dequeue();

            int _floackID = (fishID == -1) ? Random.Range(0, 4) : fishID;
            bool _isGoingRight = isRight ?? (fish._floackID % 2 == 0);

            fish.SetSpawn(data, _floackID, _isGoingRight);

            fish.FishTransform.anchoredPosition = spawnPosition;
            fish.gameObject.SetActive(true);

            _activeFish.Add(fish);
        }
    }
    private IEnumerator RepeatSpawnFish()
    {
        while (true)
        {
            // 무조건 화면 밖에서 생성
            yield return StartCoroutine(SpawnEdge());

            // 다음 무리 생성 대기시간
            yield return new WaitForSeconds(_spawnInterval);
        }
    }
    public void ReturnFish(BackGroundFish fish)
    {
        // 물고기가 비활성화 되면 리스트에서 제거하고 큐에 다시 넣음
        if (_activeFish.Contains(fish))
        {
            _activeFish.Remove(fish);
            _fishQueue.Enqueue(fish);
        }
    }
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
