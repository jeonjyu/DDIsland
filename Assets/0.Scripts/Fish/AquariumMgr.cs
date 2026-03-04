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

    public RectTransform SpawnArea => _spawnArea;

    private void Awake()
    {
        FishMake();
    }
    private void Start()
    {
        StartCoroutine(RepeatSpawnFish());
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

        int flockID = Random.Range(0,4);
        bool isRight = (flockID % 2 == 0);

        int count = Random.Range(_minFlockSize, _maxFlockSize);

        // 화면 내에서 랜덤 Y 위치
        float flockY = Random.Range(-_spawnArea.rect.height / 2 + 100f, _spawnArea.rect.height / 2 - 100f); 

        for (int i = 0; i < count; i++)
        {
            float spawnX = isRight ? -1100f : 1100f; // 오른쪽으로 갈거면 왼쪽(-1100)에서 생성. 얘들은 해상도에 따라 임시로 설정
            spawnX += Random.Range(-50f, 50f); // 약간의 랜덤 위치 보정
            float spawnY = flockY + Random.Range(-50f, 50f); // 무리 내에서 약간의 Y 위치 보정

            Vector2 spawnPos = new(spawnX, spawnY);

            SpawnFish(spawnPos, fishID: flockID, isRight: isRight);

            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SpawnFish(Vector2 spawnPosition, int fishID = -1, bool? isRight = null)
    {
        if (_fishQueue.Count > 0)
        {
            BackGroundFish fish = _fishQueue.Dequeue();

            fish._fishID = (fishID == -1) ? Random.Range(0, 4) : fishID;
            fish._isGoingRight = isRight ?? (fish._fishID % 2 == 0);

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

        // 물고기 비활성화
        //var temp = new List<BackGroundFish>(_activeFish); // 복사 
        //foreach (var fish in temp)
        //    fish.gameObject.SetActive(false);
    }
    // 물고기 보이기 
    public void ShowFish() 
    {
        // 물고기 투명도 1 (보이는 상태로)
        var cg = _spawnArea.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 1f;
        StartCoroutine(RepeatSpawnFish());

        // 물고기 다시 활성화 (초기 위치에서 스폰됨) 
        //foreach (var fish in _activeFish)
        //    fish.gameObject.SetActive(true);

    }
}
