
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public enum Point
{
    Fish,Store,Kitchen,Rest,Acorn
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
    private bool _shouldSell = false;
    private bool _isFishing = false;
    private bool _isAcornFalling = false;
    private bool _isResting = false;
    private bool _isYawning = false;
    private bool _hasYawned = false;
    private int _fishingCount = 5;  //낚시횟수

    private Coroutine _fishingRoutine;  //낚시 코루틴
    private Coroutine _yawnRoutine;  //하품 코루틴
    public PlayerData PlayerData { get; private set; }

    Point _currentPoint = Point.Fish;  //플레이어 목적지
    [SerializeField] private Transform _fishPoint;  //각 지점 위치
    [SerializeField] private Transform _storePoint;
    [SerializeField] private Transform _kitchenPoint;
    [SerializeField] private Transform _restAreaPoint;
     private Transform _acornPoint;

    [SerializeField] private SkinnedMeshRenderer _targetSMR;
    [SerializeField] private SkinnedMeshRenderer _slimSource;
    [SerializeField] private SkinnedMeshRenderer _normalSource;
    [SerializeField] private SkinnedMeshRenderer _roundSource;
    [SerializeField] private SkinnedMeshRenderer _chubbySource;

    [SerializeField] private GameObject _fishingRod;

    private float _hungerTickTimer;
    private float _baseMoveSpeed;  
    private bool _slowApplied;

    [SerializeField] private GameObject _acornPrefab;
    private readonly List<GameObject> _acorns = new();
    private int _acornIndex = 0; //도토리 프리팹 인덱스

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
    public Transform StorePoint => _storePoint;
    public Transform KitchenPoint => _kitchenPoint;
    public Transform RestAreaPoint => _restAreaPoint;
    public Transform AcornPoint => _acornPoint;
    public int FishingCount => _fishingCount;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
        if (PlayerData == null) PlayerData = new PlayerData();
        //테스트용
        PlayerData.SetHunger(100);
        PlayerData.SetStamina(100);
        PlayerData.SetMoveSpeed(1);
        PlayerData.SetDoongDoongStat(1000);
    }
    private void Start()
    {
        _baseMoveSpeed = PlayerData.MoveSpeed;
        _fishingRod.gameObject.SetActive(false);
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

   public void StartAcornSupply(Vector3 center)  //도토리 떨어지는 함수
    {
        if (_isAcornFalling) return;
        _isAcornFalling = true;
        _acorns.Clear();
        _acornIndex = 0;

        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPos = center + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                GameObject acorn = Instantiate(_acornPrefab, hit.position + Vector3.up * 2f, Quaternion.identity);
                _acorns.Add(acorn);
            }
        }

        SetNextAcornTargetAndMove();
    }
    private void SetNextAcornTargetAndMove()  //다음 도토리 위치로 이동하거나, 도토리가 없으면 도토리 떨어지는 상태 종료
    {
        _acorns.RemoveAll(a => a == null);  //파괴된 도토리 리스트에서 제거

        if (_acorns.Count == 0)  
        {
            _isAcornFalling = false;
            SetState(new IdleState(this));
            return;
        }

        if (_acornIndex >= _acorns.Count) _acornIndex = 0;  
        _acornPoint = _acorns[_acornIndex].transform;    
        SetState(new MoveState(this, Point.Acorn));
    }
    public void EatCurrentAcorn()  //도토리 먹는 함수, 도토리 없으면 도토리 떨어지는 상태 종료
    {
        if (!_isAcornFalling) return;
        if (_acorns.Count == 0)  
        {
            SetNextAcornTargetAndMove();
            return;
        }
        var acorn = _acorns[_acornIndex];  

        PlayerData.SetHunger(PlayerData.Hunger + 10);
        Destroy(acorn);
        _acorns.RemoveAt(_acornIndex);
        if (_acorns.Count == 0)
        {
            _isAcornFalling = false;
            SetState(new IdleState(this));
            return;
        }
        else SetNextAcornTargetAndMove();

    }
    public void TryConsumeFood()
    {
        // 음식 섭취 로직
    }

    public void TryCooking()
    {
        if (!_canCook) return;
        if (_isCooking) return;
        if (PlayerData.Stamina < 10)
        {
            _canCook = false;
            return;
        }
        _isCooking = true;
        PlayerData.SetStamina(PlayerData.Stamina - 10);
        _animator.SetBool("isCook", true);
    }
    public void AnimEvent_CookingEnd()  //이함수를 Cook_FryingPan_Mix@loop 애니의 끝부분에 넣기
    {
        //요리 결과 보관
        //레시피대로 재료 소비
        //재료 남았나 체크해서 _canCook 갱신
        _isCooking = false;
        _animator.SetBool("isCook", false);
        if (_canCook && _currentState is CookState)
        {
            TryCooking();
            return;
        }
         SetState(new IdleState(this));  //재료없으면
    }  
    public void ExhaustionMovement()  //피로도에 따른 이동속도 감소 및 애니메이션 변화
    {
        //로그창 폭발로 주석처리
        if (PlayerData.Stamina > 30) return;
        if (PlayerData.Stamina <= 10) return; //Debug.Log("탈진 상태. 완전히 피곤해 찌들은 애니메이션");
        else if (PlayerData.Stamina <= 14) return;//Debug.Log("꾸벅거림이 심해진다.");
        else if (PlayerData.Stamina <= 19) return;//Debug.Log("꽤나 자주 꾸벅거린다.");
        else if (PlayerData.Stamina <= 24) return; //Debug.Log("조금 더 자주 꾸벅거린다.");
        else return; //Debug.Log("가끔 꾸벅거린다"); 
    }
    public void HungerMovement()  //배고픔에 따른 이동속도 감소 및 애니메이션 변화
    {
        if (!_ishungery || PlayerData.Hunger > 25)
        {
            if (_slowApplied)
            {
                PlayerData.SetMoveSpeed(_baseMoveSpeed);
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
        if (PlayerData.Hunger <= 9) interval = 1f;
        else if (PlayerData.Hunger <= 14) interval = 3f;
        else if (PlayerData.Hunger <= 19) interval = 5f;
        else if (PlayerData.Hunger <= 25) interval = 10f;
        ApplyHunger(interval);
    }
    public void ApplyHunger(float inter)  //배고픔 단계에 따른 둥둥 감소 간격 조절
    {
        if (inter <= 0f) return;
        _hungerTickTimer += Time.deltaTime;
        while (_hungerTickTimer >= inter)
        {
            _hungerTickTimer -= inter;

            int next = Mathf.Max(0, PlayerData.DoongDoongStat - 1);
            PlayerData.SetDoongDoongStat(next);
            if (next == 0) break;
        }
    }
    private void UpdateConditionFlags()  //상태 업데이트
    {
        _ishungery = PlayerData.Hunger <= 25;
        
        if (_isAcornFalling) return;
        if (!_canCook && !_shouldSell && PlayerData.Hunger <= 20)
        {
            StartAcornSupply(transform.position); 
        }
    }

    private void UpdateMoveType()  //이동타입 업데이트, 애니메이션에 적용
    {
        // 0 기본 / 1 탈진 / 2 배고픔
        float moveType;
        if (_ishungery) moveType = 2;
        else if (PlayerData.Stamina <= 30) moveType = 1;
        else moveType = 0;
        _animator.SetFloat("MoveType", moveType);
    }

    public void ApplyTier()  //둥둥수치에 따른 외형변화
    {
        SkinnedMeshRenderer src;
        if (PlayerData.DoongDoongStat >= 1000) src = _roundSource;
        else if (PlayerData.DoongDoongStat >= 500) src = _chubbySource;
        else if (PlayerData.DoongDoongStat >= 300) src = _normalSource;
        else src = _slimSource;
        ApplySkin(src);
    }
    private void ApplySkin(SkinnedMeshRenderer src)
    {
        if (src == null || _targetSMR == null) return;

        _targetSMR.sharedMesh = src.sharedMesh;
        _targetSMR.sharedMaterials = src.sharedMaterials;

    }

    public void TryFishing()
    {
        if (_isFishing) return; 
        _isFishing = true;
        _fishingRoutine = StartCoroutine(FishWait());
    }
    public void StopFishing()
    {
        if (_fishingRoutine != null)
        {
            StopCoroutine(FishWait());
            _isFishing = false;
        }
        _isFishing = false;
    }
    IEnumerator FishWait()  //5~8초 텀으로 낚시하기
    {
        var waitTime = Random.Range(5f, 8f);
        yield return new WaitForSeconds(waitTime);
        _animator.SetTrigger("FishingHit");
        //물고기 얻기, 보관함추가등등      
        FishManager.Instance.PickRandomSeasonFish();
    }
    public void AnimEvent_FishingCycleEnd() 
    {
        _fishingCount--; 
        Debug.Log("fishingCount: " + _fishingCount);
        PlayerData.SetHunger(PlayerData.Hunger - 4);
        PlayerData.SetStamina(PlayerData.Stamina - 5);
        _isFishing = false; _fishingRoutine = null;
        if (_fishingCount <= 0 || PlayerData.Hunger <= 0 || PlayerData.Stamina <= 0)
        {
            Animator.SetBool("isFish", false);
            SetState(new IdleState(this)); 
        }
    }
    public void ResetFishingCount()
    {
        _fishingCount = 5;
    }
    public void HandOnFishingRod()
    {
        _fishingRod.SetActive(true);
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
    public void FullyRecovered()  //휴식이 다찰때까지 어떤 상태로도 전환 불가
    {
        if (_isResting) return;
        _isResting = true;

        InvokeRepeating("WaitRecovered", 0f, 1f);
    }
    private void WaitRecovered()
    {
        float increase = PlayerData.Stamina * 0.02f;
        PlayerData.SetStamina(PlayerData.Stamina + increase);
        if (PlayerData.Stamina >= 100)
        {
            CancelInvoke("WaitRecovered");
            _isResting = false;
            SetState(new IdleState(this));
        }
    }
    public void ApplyMoveSpeed()
    {
        float baseSpeed = PlayerData.MoveSpeed;
        float finalSpeed = IsHungery ? baseSpeed * 0.5f : baseSpeed;  //배고프면 속도 절반

        _agent.speed = finalSpeed;
    }
    public void PlayAnim(string anim)
    {
        _animator.Play(anim);
    }

    public void MoveTo(Transform target)
    {
        _agent.SetDestination(target.position);
    }

    public void RequestReplan()  //idle에서 자율적으로 다음행동 결정
    {
        var nextState = DecideNextState();
        if (nextState is not SleepState && _isResting == true) return;
        if (nextState != null)
            SetState(nextState);
    }

    private IState DecideNextState()    //상태에 따른 도착지 결정, Move상태가 도착지의 따른 행동결정
    {
        //if (PlayerData.Hunger <= 20)
        //    return new EatState(this);

        if (PlayerData.Stamina <= 10)
        {
            _currentPoint = Point.Rest;
            return new MoveState(this, _currentPoint);
        }

        if (_canCook)
        {
            _currentPoint = Point.Kitchen;
            return new MoveState(this, _currentPoint);
        }

       // if (_shouldSell)
       // {
       //     _currentPoint = Point.Store;
       //     return new MoveState(this, _currentPoint);
       // }
       if (_isAcornFalling) 
        {
            _currentPoint = Point.Acorn;
            return new MoveState(this, _currentPoint);  
        }
        _currentPoint = Point.Fish;
        return new MoveState(this, _currentPoint);
    }

    public void SetState(IState newState)
    {
        if (newState is not SleepState && _isResting == true) return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
}
