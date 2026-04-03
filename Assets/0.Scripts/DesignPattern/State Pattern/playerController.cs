
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Point
{
    Fish,Kitchen,Rest,Acorn,Table,Sell
}
public class PlayerController : MonoBehaviour
{
    private IState _currentState;

    private Animator _animator;
    private Rigidbody2D _rigid;
    private NavMeshAgent _agent;
    private MeshRenderer _meshRenderer;

    private bool _ishungery = false;
    private bool _canCook = false;
    private bool _isCooking = false;
    private bool _isFishing = false;
    private bool _isAcornFalling = false;
    private bool _isResting = false;
    private bool _isYawning = false;
    private bool _hasYawned = false;
    private int _fishingCount = 5;  //낚시횟수

    private Coroutine _fishingRoutine;  //낚시 코루틴
    private bool _isFishingState;
    private bool _cycleRunning;

    private Coroutine _yawnRoutine;  //하품 코루틴

    private Coroutine _sellRoutine;

    public CharacterDataSO PlayerDataSO;
    //public PlayerContext playerData;  //일단은 남겨둠 혹시모르니까
    public PlayerData PlayerDataOld; //업그레이드매니저를위한거
    private FoodDataSO PendingFood;

    Point _currentPoint = Point.Fish;  //플레이어 목적지
    [SerializeField] private Transform _fishPoint;  //각 지점 위치
    [SerializeField] private Transform _kitchenPoint;
    [SerializeField] private Transform _restAreaPoint;
    [SerializeField] private Transform _tablePoint;
    [SerializeField] private Transform _SellPoint;
    private Transform _acornPoint;

    [SerializeField] private SkinnedMeshRenderer _targetSMR;
    private SkinnedMeshRenderer _slimSource;
    private SkinnedMeshRenderer _normalSource;
    private SkinnedMeshRenderer _chubbySource;
    private SkinnedMeshRenderer _roundSource;

    [SerializeField] GameObject _slimPrefab;
    [SerializeField] GameObject _normalPrefab;
    [SerializeField] GameObject _chubbyPrefab;
    [SerializeField] GameObject _roundPrefab;

    [SerializeField] FishingHookEffect _fishingHookEffect;

    [SerializeField] private GameObject _fishingRod;
    [SerializeField] private GameObject _fork;
    [SerializeField] private GameObject _pan;

    [SerializeField] private GameObject[] _hats;
    [SerializeField] private GameObject[] _bodys;
    [SerializeField] private GameObject[] _fishingRods;
    private Dictionary<int, GameObject> _hatById;
    private Dictionary<int, GameObject> _bodyById;
    private Dictionary<int, GameObject> _fishingRodsById;

    private int _equippedBaitId;  //저장용들임
    private int _equippedBobberId;
    private int _equippedHatId;
    private int _equippedbodyId;
    private int _equippedfishingRodsId;

    private float _bobberLengthBonus;
    private int _baitFishPool;

    private EnvironmentModel _environment;
    private DayilyCycle _currentSeason;
    private bool _isSummerRain;

    private float _hungerTickTimer;
    private float _baseMoveSpeed;
    private bool _slowApplied;
    private SkinnedMeshRenderer _currentSkinSource; //현재 상태를 저장하기 위한 변수입니다 +추가됨

    [SerializeField] private GameObject _acornPrefab;
    private readonly List<GameObject> _acorns = new();
    private int _acornIndex = 0; //도토리 프리팹 인덱스

    private Coroutine _recoverRoutine;

    Dictionary<int, float> _fishRodSpeed;
    private int _currentFishingRodsId;
    private float _baseFishingSpeed;

    [SerializeField] private AudioClip _fishingSfx;
    [SerializeField] private AudioClip _sellSfx;
    [SerializeField] private AudioClip _hungerSfx;
    [SerializeField] private AudioClip _eatSfx;
    [SerializeField] private AudioClip _cookSfx;
    private bool _wasHungry;

    public Rigidbody2D Rigid => _rigid;
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;
    public MeshRenderer MeshRenderer => _meshRenderer;
    public bool IsHungery => _ishungery;
    public bool IsResting => _isResting;
    public bool HasYawn => _hasYawned;
    public bool IsYawning => _isYawning;
    public bool IsCookuing => _isCooking;
    public bool CanCook => _canCook;
    public Point Point => _currentPoint;
    public Transform FishPoint => _fishPoint;
    public Transform KitchenPoint => _kitchenPoint;
    public Transform RestAreaPoint => _restAreaPoint;
    public Transform AcornPoint => _acornPoint;
    public Transform TablePoint => _tablePoint;
    public Transform SellPoint => _SellPoint;
    public int FishingCount => _fishingCount;
    public float BobberLengthBonus => _bobberLengthBonus;
    public int BaitFishPool => _baitFishPool;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
        _slimSource = _slimPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        _normalSource = _normalPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        _chubbySource = _chubbyPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        _roundSource = _roundPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        PlayerDataOld = new PlayerData();
        _hatById = new Dictionary<int, GameObject>();
        _bodyById = new Dictionary<int, GameObject>();
        _fishingRodsById = new Dictionary<int, GameObject>();
        _fishRodSpeed = new Dictionary<int, float>();
    }
    private void Start()
    {
        Debug.Log($"[PlayerController] Start 호출됨, PlayerDataSO null = {PlayerDataSO == null}");
        if (PlayerDataSO == null) return;
        ApplyPlayerStats(PlayerDataSO);

        // 데이터 불러오기 추가
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                PlayerDataOld.SyncCharacterDataLoad();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += PlayerDataOld.SyncCharacterDataLoad;
            }
        }

        _baseMoveSpeed = PlayerDataOld.MoveSpeed;
        _baseFishingSpeed = PlayerDataOld.FishingSpeed;
        _fishingRod.gameObject.SetActive(false);
        _fork.gameObject.SetActive(false);
        _pan.gameObject.SetActive(false);
        BuildCostumeMap();

        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                PlayerDataOld.SyncCharacterDataLoad();
                SyncEquipDataLoad();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += PlayerDataOld.SyncCharacterDataLoad;
                DataManager.Instance.Hub.OnDataLoaded += SyncEquipDataLoad;
            }
        }

        RefreshCanCook();
    }
    private void Update()
    {
        if (_currentState == null)
            SetState(new IdleState(this));
        UpdateConditionFlags(); //상태 업데이트

        HungerMovement();  //배고픔에 따른 이동속도 감소 
        ExhaustionMovement(); //피로도에 따른 이동속도 감소 
        UpdateMoveType();     //위 두개 상태에 따른 이동타입 업데이트, 애니메이션에 적용


        ApplyTier();  //둥둥수치에 따른 외형변화
        _currentState?.Execute();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedExecute();
    }
    private void OnEnable()
    {
        if (FishStorageManager.Instance != null)
            FishStorageManager.Instance.OnSlotChanged += OnFishStorageChanged;

        if (FoodStorageManager.Instance != null)
            FoodStorageManager.Instance.OnSlotChanged += OnFoodStorageChanged;

        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += PlayerDataOld.SyncCharacterDataSave;
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnEquipChanged += ChangeCostume;
        if (EmojiController.Instance != null)
        {
            PlayerDataOld.OnHungerChanged += (_) => UpdateEmoji();
            PlayerDataOld.OnStaminaChanged += (_) => UpdateEmoji();
        }

        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnRequestSave += PlayerDataOld.SyncCharacterDataSave;
            DataManager.Instance.Hub.OnRequestSave += SyncEquipDataSave;
        }
    }

    private void OnDisable()
    {
        if (FishStorageManager.Instance != null)
            FishStorageManager.Instance.OnSlotChanged -= OnFishStorageChanged;

        if (FoodStorageManager.Instance != null)
            FoodStorageManager.Instance.OnSlotChanged -= OnFoodStorageChanged;

        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (!DataManager.Instance.Hub.IsQuite)
            {
                DataManager.Instance.Hub.OnRequestSave -= PlayerDataOld.SyncCharacterDataSave;
            }
        }
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnEquipChanged -= ChangeCostume;
        if (EmojiController.Instance != null)
        {
            PlayerDataOld.OnHungerChanged -= (_) => UpdateEmoji();
            PlayerDataOld.OnStaminaChanged -= (_) => UpdateEmoji();
        }

        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (!DataManager.Instance.Hub.IsQuite)
            {
                DataManager.Instance.Hub.OnRequestSave -= PlayerDataOld.SyncCharacterDataSave;
                DataManager.Instance.Hub.OnRequestSave -= SyncEquipDataSave;
            }
        }
    }

    public void ApplyPlayerStats(CharacterDataSO SO)
    {
        PlayerDataOld.Initialize(SO);
    }

    public void ChangeCostume(Enum type, int objectId)  //코스튬 타입에 따른 코스튬쓰기
    {
        string typeName = type.ToString();

        if (typeName == "Head")
        {
            SetHat(objectId);
        }
        else if (typeName == "Body")
        {
            SetBody(objectId);
        }
         else if (typeName == "Pole")
         {
             SetTool(objectId);
         }
        else if (typeName == "Bait")
        {
            SetBait(objectId);
        }
        else if (typeName == "Bobber")
        {
            SetBobber(objectId);
        }

        DataManager.Instance.Hub.SaveAllData();
    }
    private void BuildCostumeMap()  //모자,옷을 ID로 빠르게 찾을 수 있게 딕셔너리
    {
        _hatById.Clear();
        _bodyById.Clear();
        _fishingRodsById.Clear();

        var data = DataManager.Instance.DecorationDatabase.CostumeData.datas;
        var fishingItemData = DataManager.Instance.FishingDatabase.FishingItemData.datas;

        if (data != null)
        {
            int hatIndex = 0;
            int bodyIndex = 0;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == null) continue;

                if (data[i].costumeType == CostumeType.Head)
                {
                    if (hatIndex >= _hats.Length) continue;
                    if (_hats[hatIndex] == null)
                    {
                        hatIndex++;
                        continue;
                    }

                    _hatById[data[i].CostumeID] = _hats[hatIndex];
                    _hats[hatIndex].SetActive(false);
                    hatIndex++;
                }
                else if (data[i].costumeType == CostumeType.Body)
                {
                    if (bodyIndex >= _bodys.Length) continue;
                    if (_bodys[bodyIndex] == null)
                    {
                        bodyIndex++;
                        continue;
                    }

                    _bodyById[data[i].CostumeID] = _bodys[bodyIndex];
                    _bodys[bodyIndex].SetActive(false);
                    bodyIndex++;
                }
            }
        }

        if (fishingItemData != null)
        {
            int toolIndex = 0;

            for (int i = 0; i < fishingItemData.Count; i++)
            {
                if (fishingItemData[i] == null) continue;
                if (fishingItemData[i].fishingitemType != FishingItemType.Pole) continue;
                if (toolIndex >= _fishingRods.Length) continue;
                if (_fishingRods[toolIndex] == null)
                {
                    toolIndex++;
                    continue;
                }

                _fishingRodsById[fishingItemData[i].ID] = _fishingRods[toolIndex];
                _fishRodSpeed[fishingItemData[i].ID] = fishingItemData[i].FishingSpeed;
                _fishingRods[toolIndex].SetActive(false);
                toolIndex++;
            }
        }
        Debug.Log($"Hat:{_hatById.Count}, Body:{_bodyById.Count}");
    }
    public void SetHat(int id)
    {
        foreach (var pair in _hatById)
        {
            pair.Value.SetActive(false);
        }

        if (id == 0) return;

        if (_hatById.TryGetValue(id, out var hatObj))
        {
            hatObj.SetActive(true);
            _equippedHatId = id;
            Debug.Log($"모자 장착 성공: {id}");
        }
        else Debug.LogWarning($"모자 ID 없음: {id}");
    }
    public void SetBody(int id)
    {
        foreach (var pair in _bodyById)
        {
            pair.Value.SetActive(false);
        }

        if (id == 0) return;

        if (_bodyById.TryGetValue(id, out var bodyObj))
        {
            bodyObj.SetActive(true);
            _equippedbodyId = id;
            Debug.Log($"의상 장착 성공: {id}");
        }
        else Debug.LogWarning($"의상 ID 없음: {id}");
    }
     public void SetTool(int id)  //pole
     {
         foreach (var pair in _fishingRodsById)
         {
             pair.Value.SetActive(false);
         }

        if (id == 0)
        {
            _currentFishingRodsId = 0;
            PlayerDataOld.SetFishingSpeed(_baseFishingSpeed);
            if (_isFishingState) HandOnFishingRod();
            return;
        }

        if (_fishingRodsById.TryGetValue(id, out var toolObj))
         {
             PlayerDataOld.SetFishingSpeed(_baseFishingSpeed - _fishRodSpeed[id]);
            _currentFishingRodsId = id;
            _equippedfishingRodsId = id;
            if (_isFishingState) HandOnFishingRod();

            Debug.Log($"의상 장착 성공: {id}");
         }
         else Debug.LogWarning($"의상 ID 없음: {id}");
     }
    public void SetBait(int id)
    {
        if (id == 0) id = 40001; // 하급 미끼 기본값
        _equippedBaitId = id;

        var data = DataManager.Instance.FishingDatabase.FishingItemData[id];
        _baitFishPool = data != null ? data.FishPool : 0;

        Debug.Log($"미끼 장착 성공: {id}");
    }
    public void SetBobber(int id)
    {
        if (id == 0) id = 40004; // 기본 낚시찌 기본값
        _equippedBobberId = id;

        var data = DataManager.Instance.FishingDatabase.FishingItemData[id];
        _bobberLengthBonus = data != null ? data.FishLength : 0f;
        Debug.Log($"낚시찌 장착 성공: {id}");
    }
    public void StartAcornSupply(Vector3 center)  //도토리 떨어지는 함수
    {
        _isAcornFalling = true;

        foreach (var acorn in _acorns)
        {
            if (acorn != null)
                Destroy(acorn);
        }

        _acorns.Clear();
        _acornPoint = null;
        _acornIndex = 0;

        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPos = center + new Vector3(UnityEngine.Random.Range(-4f, 4f), 0, UnityEngine.Random.Range(-4f, 4f));

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                GameObject acorn = Instantiate(_acornPrefab, hit.position + Vector3.up * 2f, Quaternion.identity);
                _acorns.Add(acorn);
            }
        }

        _acorns.RemoveAll(a => a == null);

        if (_acorns.Count > 0)
            _acornPoint = _acorns[0].transform;
        else
            _isAcornFalling = false;
    }
    private void SetNextAcornTargetAndMove()  //다음 도토리 위치로 이동하거나, 도토리가 없으면 도토리 떨어지는 상태 종료
    {
        _acorns.RemoveAll(a => a == null);

        if (_acorns.Count == 0)
        {
            _isAcornFalling = false;
            _acornPoint = null;
            SetState(new IdleState(this));
            return;
        }

        if (_acornIndex >= _acorns.Count)
            _acornIndex = 0;

        _acornPoint = _acorns[_acornIndex].transform;
        SetState(new MoveState(this, Point.Acorn));
    }
    public void InterruptToAcorn(Vector3 center)
    {
        if (_currentState is SleepState) return;

        if (_isAcornFalling && _acornPoint != null)
        {
            SetState(new MoveState(this, Point.Acorn), true);
            return;
        }

        if (!(_currentState is IdleState)) StopCurrentAction();

        StartAcornSupply(center);

        if (_isAcornFalling && _acornPoint != null)
            SetState(new MoveState(this, Point.Acorn), true);
        else
            SetState(new IdleState(this), true);
    }
    public void EatCurrentAcorn()  //도토리 먹는 함수, 도토리 없으면 도토리 떨어지는 상태 종료
    {
        if (!_isAcornFalling) return;

        _acorns.RemoveAll(a => a == null);

        if (_acorns.Count == 0)
        {
            _acornPoint = null;
            _isAcornFalling = false;
            SetState(new IdleState(this));
            return;
        }

        if (_acornIndex >= _acorns.Count)
            _acornIndex = 0;

        var acorn = _acorns[_acornIndex];
        if (acorn == null)
        {
            _acornPoint = null;
            SetNextAcornTargetAndMove();
            return;
        }

        if (_acornPoint == acorn.transform)
            _acornPoint = null;
        SoundManager.Instance.PlaySFX(_eatSfx);
        PlayerDataOld.SetHunger(PlayerDataOld.Hunger + 10);

        _acorns.RemoveAt(_acornIndex);
        Destroy(acorn);

        if (_acorns.Count == 0)
        {
            _isAcornFalling = false;
            _acornPoint = null;
            SetState(new IdleState(this));
            return;
        }

        SetNextAcornTargetAndMove();

    }
    public void StopCurrentAction()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;

        PullUpFishingHookEffect();

        ExitFishingState();
        _isFishing = false;
        _animator.SetBool("isFish", false);
        _animator.ResetTrigger("FishingHit");
        HandOffFishingRod();

        _isCooking = false;
        PendingFood = null;
        _animator.SetBool("isCook", false);

        _animator.ResetTrigger("isEat");
        _animator.ResetTrigger("Yawn");

        if (_yawnRoutine != null)
        {
            StopCoroutine(_yawnRoutine);
            _yawnRoutine = null;
        }

        _isYawning = false;
        _hasYawned = false;

        _animator.SetBool("isMove", false);
        _animator.SetBool("isIdle", false);

        if (!(_currentState is SleepState))
        {
            StopRecover();
        }
    }
    public void AnimEvent_TryConsumeFood()  //먹기 애니에 넣기
    {
        if (!(_currentState is EatState))
            return;

        SoundManager.Instance.PlaySFX(_eatSfx);

        int index = FoodStorageManager.Instance.TakeOutRandomFood();
        if (index == -1) return;
        var slot = FoodStorageManager.Instance.GetFoodSlot(index);
        if (!slot.HasValue) return;
        int foodId = slot.Value.FoodId;
        var food = DataManager.Instance.FoodDatabase.FoodInfoData[foodId];
        if (food == null) return;
        if (food.foodeffectType == FoodEffectType.None) return;
        float afterHunger = PlayerDataOld.Hunger + food.HungerBuffRate;
        if (afterHunger > PlayerDataOld.MaxHunger)
        {

            PlayerDataOld.SetDoongDoongStat(PlayerDataOld.DoongDoongStat + Mathf.FloorToInt(afterHunger - PlayerDataOld.MaxHunger));
        }

        PlayerDataOld.SetHunger(PlayerDataOld.Hunger + food.HungerBuffRate);
        

        PlayerDataOld.SetDoongDoongStat(PlayerDataOld.DoongDoongStat + food.DoongDoongBuffRate);

        FoodStorageManager.Instance.TryFoodRemoveAt(index);
        //만약 추후 음식버프 효과가 주가되면 버프는 중첩안되게 하기
    }

    public void AnimEvent_EatEnd()  //애니재생 끝나는 타이밍을 맞추기 위한거
    {
        if (!(_currentState is EatState))
            return;
        SetState(new IdleState(this));
    }

    public bool TryCooking()
    {
        RefreshCanCook();

        if (!_canCook) return false;
        if (_isCooking) return false;
        if (PlayerDataOld.Stamina < 10)
        {
            _canCook = false;
            return false;
        }

        SoundManager.Instance.PlaySFX(_cookSfx);

        var candidates = CookingManager.Instance.BuildCookCandidates(new CookingContext());
        var pickedFood = CookingManager.Instance.PickRecipe(candidates, new CookingContext());

        if (pickedFood == null)
        {
            _canCook = false;
            return false;
        }

        PendingFood = pickedFood;
        _isCooking = true;
        ConsumeStamina(10);
        _animator.SetBool("isCook", true);
        CookingManager.Instance.FoodIngredientsRemove(pickedFood);

        return true;
    }
    public void AnimEvent_CookingEnd()  //이함수를 Cook_FryingPan_Mix@loop 애니의 끝부분에 넣기
    {
        if (!(_currentState is CookState))
            return;

        if (PendingFood != null)
        {
            CookingManager.Instance.CreateCookedFood(PendingFood);
            PendingFood = null;
        }

        _isCooking = false;
        _animator.SetBool("isCook", false);
        RefreshCanCook();

        SetState(new IdleState(this));
    }
    public void RefreshCanCook()  //요리 조건 확인
    {
        var candidates = CookingManager.Instance.BuildCookCandidates(new CookingContext());

        if (candidates == null || candidates.Count == 0)
        {
            Debug.Log("요리 가능한 음식 없음");
            _canCook = false;
            return;
        }
        var pickedFood = CookingManager.Instance.PickRecipe(candidates, new CookingContext());
        if (pickedFood == null)
        {
            Debug.Log("레시피 선택 실패");
            _canCook = false;
            return;
        }

        if (!FoodStorageManager.Instance.HasSpaceForFood(pickedFood.ID))
        {
            Debug.Log("요리 보관함이 가득 참");
            _canCook = false;
            return;
        }
        _canCook = true;
    }
    private void OnFishStorageChanged(int index)
    {
        RefreshCanCook();
    }

    private void OnFoodStorageChanged(int index)
    {
        RefreshCanCook();
    }
    public void ExhaustionMovement()  //피로도에 따른 이동속도 감소 및 애니메이션 변화
    {
        //로그창 폭발로 주석처리
        if (PlayerDataOld.Stamina > 30) return;
        if (PlayerDataOld.Stamina <= 10) return; //Debug.Log("탈진 상태. 완전히 피곤해 찌들은 애니메이션");
        else if (PlayerDataOld.Stamina <= 14) return;//Debug.Log("꾸벅거림이 심해진다.");
        else if (PlayerDataOld.Stamina <= 19) return;//Debug.Log("꽤나 자주 꾸벅거린다.");
        else if (PlayerDataOld.Stamina <= 24) return; //Debug.Log("조금 더 자주 꾸벅거린다.");
        else return; //Debug.Log("가끔 꾸벅거린다"); 
    }
    public void HungerMovement()  //배고픔에 따른 이동속도 감소 및 애니메이션 변화
    {
        if (!_ishungery || PlayerDataOld.Hunger > 25)
        {
            if (_slowApplied)
            {
                //playerData.MoveSpeed = _baseMoveSpeed;
                PlayerDataOld.SetMoveSpeed(_baseMoveSpeed);
                _slowApplied = false;
            }
            _hungerTickTimer = 0f;
            return;
        }
        ApplyHungerTier();
        // _animator.SetFloat("MoveType", 2);
        // Debug.Log("배고픔 힘 빠진 이동");
    }
    public void ApplyHungerTier()  //배고픔 단계에 따른 둥둥 감소 간격 조절
    {
        float interval = 0f;

        if (PlayerDataOld.Hunger <= 9) interval = 1f;
        else if (PlayerDataOld.Hunger <= 14) interval = 3f;
        else if (PlayerDataOld.Hunger <= 19) interval = 5f;
        else if (PlayerDataOld.Hunger <= 25) interval = 10f;

        if (_isSummerRain)  interval /= 1.2f;

        ApplyHunger(interval);
    }
    private void HandleWeatherChanged(Season season, bool isPlaying)
    {
        _isSummerRain = (season == Season.Summer && isPlaying);
    }
    public void ApplyHunger(float inter)  //배고픔 단계에 따른 둥둥 감소 간격 조절
    {
        if (inter <= 0f) return;
        _hungerTickTimer += Time.deltaTime;
        while (_hungerTickTimer >= inter)
        {
            _hungerTickTimer -= inter;

            int next = Mathf.Max(0, PlayerDataOld.DoongDoongStat - 1);
            //playerData.DoongDoongStat = next;
            PlayerDataOld.SetDoongDoongStat(next);
            if (next == 0) break;
        }
    }
    private void UpdateConditionFlags()  //상태 업데이트
    {
        bool wasHungry = _ishungery;
        _ishungery = PlayerDataOld.Hunger <= 25;

        if (!wasHungry && _ishungery)
        {
            SoundManager.Instance.PlaySFX(_hungerSfx);
        }


        if (_isAcornFalling) return;
        bool noCookedFood = FoodStorageManager.Instance.FoodEmptyCheck();
        bool hasCookableRecipe = HasCookableRecipe();
        if (PlayerDataOld.Hunger <= 20 && noCookedFood && !hasCookableRecipe)
        {
            InterruptToAcorn(transform.position);
        }
    }
    private bool HasCookableRecipe()  //_canCook하나로 통일하고싶었는데 만들다보니 이변수 하나가 담당하는게 너무많아짐 그래서 따로 함수만듬 
    {
        var candidates = CookingManager.Instance.BuildCookCandidates(new CookingContext());
        return candidates != null && candidates.Count > 0;
    }
    private void UpdateMoveType()  //이동타입 업데이트, 애니메이션에 적용
    {
        // 0 기본 / 1 탈진 / 2 배고픔
        float moveType;
        if (_ishungery) moveType = 2;
        else if (PlayerDataOld.Stamina <= 30) moveType = 1;
        else moveType = 0;
        _animator.SetFloat("MoveType", moveType);
    }

    public void ApplyTier()  //둥둥수치에 따른 외형변화
    {
        SkinnedMeshRenderer src;
        if (PlayerDataOld.DoongDoongStat >= 1000) src = _roundSource;
        else if (PlayerDataOld.DoongDoongStat >= 500) src = _chubbySource;
        else if (PlayerDataOld.DoongDoongStat >= 300) src = _normalSource;
        else src = _slimSource;

        // 추가된 부분
        if (_currentSkinSource != src)
        {
            _currentSkinSource = src; // 현재 체형 갱신
            ApplySkin(src);           // 실제 모델링 교체

            DataManager.Instance.Hub.SaveAllData();
        }
    }
    private void ApplySkin(SkinnedMeshRenderer src)
    {
        if (src == null || _targetSMR == null) return;

        _targetSMR.sharedMesh = src.sharedMesh;
        _targetSMR.sharedMaterials = src.sharedMaterials;

    }

    public void EnterFishingState()
    {
        _isFishingState = true;
        _cycleRunning = false;
        SoundManager.Instance.PlaySFX(_fishingSfx);
        if (_fishingRoutine != null)
            StopCoroutine(_fishingRoutine);

        _fishingRoutine = StartCoroutine(FishingLoop());
    }
    public void ExitFishingState()
    {
        _isFishingState = false;
        _cycleRunning = false;
        PullUpFishingHookEffect();
        if (_fishingRoutine != null)
        {
            StopCoroutine(_fishingRoutine);
            _fishingRoutine = null;
        }
    }
    private IEnumerator FishingLoop()
    {
        while (_isFishingState)
        {
            // 한 사이클 시작
            _cycleRunning = true;
            float rodSpeed = 0f;
            _fishRodSpeed.TryGetValue(_currentFishingRodsId, out rodSpeed);
            float minTime = Mathf.Max(1f, 5f - rodSpeed);
            float maxTime = Mathf.Max(minTime, PlayerDataOld.FishingSpeed);
            float waitTime = UnityEngine.Random.Range(minTime, maxTime);
            Debug.Log($"낚시속도: {waitTime}");
            yield return new WaitForSeconds(waitTime);

            Animator.SetTrigger("FishingHit");

            // 사이클이 끝났다는 이벤트가 올 때까지 기다림
            yield return new WaitUntil(() => _cycleRunning == false);
            PullUpFishingHookEffect();
        }
    }

    public void AnimEvent_GiveFishOnce()  //낚시 성공 애니에 넣을 함수
    {
        FishManager.Instance.PickRandomSeasonFish();
    }
    public void AnimEvent_FishingCycleEnd() //낚시 성공 애니에 넣을 함수
    {
        _fishingCount--;
        Debug.Log("fishingCount: " + _fishingCount);
        PlayerDataOld.SetHunger(PlayerDataOld.Hunger - 4);
        ConsumeStamina(5);
        _isFishing = false;
        if (_fishingCount <= 0 || PlayerDataOld.Hunger <= 0 || PlayerDataOld.Stamina <= 0)
        {
            ExitFishingState();
            Animator.SetBool("isFish", false);
            SetState(new IdleState(this));
            return;
        }
        _cycleRunning = false;  //다음 사이클로 넘어갈 수 있게 코루틴 대기 해제
    }

    public void AnimEvent_StartFishingHookEffect() //낚시cast에 넣을 함수
    {
        if (_fishingHookEffect != null)
            _fishingHookEffect.PlayerFishingHookEffect();
    }

    public void PullUpFishingHookEffect()
    {
        if (_fishingHookEffect != null)
            _fishingHookEffect.PullUpHook();
    }

    public void ResetFishingCount()
    {
        _fishingCount = 5;
    }
    public void HandOnFishingRod()
    {
        _fishingRod.SetActive(false);

        // 장착 낚싯대가 있으면 그걸 보여줌
        if (_currentFishingRodsId != 0 &&
            _fishingRodsById.TryGetValue(_currentFishingRodsId, out var rodObj))
        {
            rodObj.SetActive(true);
        }
        else
        {
            // 장착 안 했으면 기본 낚싯대 사용
            _fishingRod.SetActive(true);
        }
    }
    public void HandOffFishingRod()
    {
        _fishingRod.SetActive(false);

        foreach (var pair in _fishingRodsById)
        {
            pair.Value.SetActive(false);
        }
    }
    public void HandOnFork()
    {
        _fork.SetActive(true);
    }
    public void HandOffFork()
    {
        _fork.SetActive(false);
    }
    public void HandOnPan()
    {
        _pan.SetActive(true);
    }
    public void HandOffPan()
    {
        _pan.SetActive(false);
    }
    public void HasYawned()
    {
        if (_isYawning) return;

        _isYawning = true;

        if (_yawnRoutine != null)
            StopCoroutine(_yawnRoutine);

        _yawnRoutine = StartCoroutine(YawnWait());
    }

    IEnumerator YawnWait()
    {
        Agent.isStopped = true;          // 하품 중 이동 금지
        Agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(8f);
        _hasYawned = false;
        _isYawning = false;
        _yawnRoutine = null;
    }


    public void SellFishs()
    {
       _sellRoutine = StartCoroutine(SellWait());
    }

    IEnumerator SellWait()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(2f);
        FishStorageManager.Instance.SellAllFish();
        SoundManager.Instance.PlaySFX(_sellSfx);
        PlayerDataOld.SetHunger(PlayerDataOld.Hunger - 4);
        ConsumeStamina(5);
        SetState(new IdleState(this));
    }

    public void StartRecover()
    {
        if (_recoverRoutine != null) return;
        _isResting = true;
        _recoverRoutine = StartCoroutine(RecoverRoutine());
    }
    public void StopRecover()
    {
        if (_recoverRoutine != null)
        {
            StopCoroutine(_recoverRoutine);
            _recoverRoutine = null;
        }
        _isResting = false;
    }
    private IEnumerator RecoverRoutine()
    {
        while (true)
        {
            float next = PlayerDataOld.Stamina + (100f * 0.02f);
            //playerData.Stamina = next;
            PlayerDataOld.SetStamina(next);
            if (PlayerDataOld.Stamina >= 100f)
            {
                //playerData.Stamina = 100f;
                PlayerDataOld.SetStamina(100f);
                _recoverRoutine = null;
                _isResting = false;
                SetState(new IdleState(this));
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }


    public void ApplyMoveSpeed()
    {
        float baseSpeed = PlayerDataOld.MoveSpeed;
        float finalSpeed = IsHungery ? baseSpeed * 0.5f : baseSpeed;  //배고프면 속도 절반

        _agent.speed = finalSpeed;
    }
   
    private void HandleDailyChanged(DayilyCycle newSeason)
    {
        _currentSeason = newSeason;
    }
    void ConsumeStamina(float baseCost)
    {
        float cost = baseCost;
        if (IsDailyNight()) cost *= 1.5f;
        PlayerDataOld.SetStamina(PlayerDataOld.Stamina - cost);
    }
    public bool IsDailyNight()
    {
        if (_currentSeason == DayilyCycle.Sunset)
        {
            return true;
        }
        return false;
    }
    void UpdateEmoji()
    {
        EmojiController.Instance.RefreshStateEmoji(PlayerDataOld, IsResting);
    }
    public void SetEnvironment(EnvironmentModel time)
    {
        if (_environment != null)
        {
            _environment.OnDailyChanged -= HandleDailyChanged;
            _environment.OnWeatherChanged -= HandleWeatherChanged;
        }

        _environment = time;

        if (_environment != null)
        {
            _environment.OnDailyChanged += HandleDailyChanged;
            _environment.OnWeatherChanged += HandleWeatherChanged;

            HandleDailyChanged(_environment.CurrentDay);
            HandleWeatherChanged(_environment.CurrentSeason, _environment.IsWeatherActive);
        }
    }
    public void RequestReplan()  //idle에서 자율적으로 다음행동 결정
    {
        RefreshCanCook();
        var nextState = DecideNextState();
        if (nextState is not SleepState && _isResting == true) return;
        if (nextState != null)
            SetState(nextState);
    }

    private IState DecideNextState()    //상태에 따른 도착지 결정, Move상태가 도착지의 따른 행동결정
    {
        if (PlayerDataOld.Hunger <= 20)  //20이하가 되어도 음식체크 => 물고기(레시피)체크 => 도토리 순서
        {
            if (!FoodStorageManager.Instance.FoodEmptyCheck())
            {
                _currentPoint = Point.Table;
                return new MoveState(this, _currentPoint);
            }

            if (_canCook)
            {
                _currentPoint = Point.Kitchen;
                return new MoveState(this, _currentPoint);
            }

            if (_isAcornFalling && _acornPoint != null)
            {
                _currentPoint = Point.Acorn;
                return new MoveState(this, _currentPoint);
            }

            return new IdleState(this);
        }

        if (PlayerDataOld.Stamina <= 10)
        {
            _currentPoint = Point.Rest;
            return new MoveState(this, _currentPoint);
        }
        if (_canCook)
        {
            _currentPoint = Point.Kitchen;
            return new MoveState(this, _currentPoint);
        }
        if (FishStorageManager.Instance.ShouldSellFish())
        {
            _currentPoint = Point.Sell;
            return new MoveState(this, _currentPoint);
        }
        if (_isAcornFalling && _acornPoint != null) 
        {
            _currentPoint = Point.Acorn;
            return new MoveState(this, _currentPoint);  
        }
        _currentPoint = Point.Fish;
        return new MoveState(this, _currentPoint);
    }

    public void SetState(IState newState, bool force = false)
    {
        if (!force && newState is not SleepState && _isResting)
            return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private void SyncEquipDataSave()
    {
        // 목적지가 Store가 아니라 Character로 변경됨
        var charBox = DataManager.Instance.Hub._allUserData.Character;

        charBox._equippedHatId = _equippedHatId;
        charBox._equippedBodyId = _equippedbodyId;
        charBox._equippedFishingRodId = _equippedfishingRodsId;
        charBox._equippedBaitId = _equippedBaitId;
        charBox._equippedBobberId = _equippedBobberId;
    }

    private void SyncEquipDataLoad()
    {
        var charBox = DataManager.Instance.Hub._allUserData.Character;

        // 파이어베이스에서 가져온 ID를 바탕으로 각각 장착 함수 실행
        SetHat(charBox._equippedHatId);
        SetBody(charBox._equippedBodyId);
        SetTool(charBox._equippedFishingRodId);
        SetBait(charBox._equippedBaitId);
        SetBobber(charBox._equippedBobberId);
    }
}
