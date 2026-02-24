using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.Controls;
using static UnityEditor.Experimental.GraphView.GraphView;


public enum Point
{
    Fish,Store,Kitchen,Rest
}
public class PlayerController : MonoBehaviour
{
    private IState _currentState;

    private Animator _animator;
    private Rigidbody2D _rigid;
    private NavMeshAgent _agent;
    private MeshRenderer _meshRenderer;

    private bool _ishungery = false;
    private bool _canCook = true;  //테스트
    private bool _shouldSell = false;
    private bool _isFishing = false;
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

    [SerializeField] private SkinnedMeshRenderer _targetSMR;
    [SerializeField] private SkinnedMeshRenderer _slimSource;
    [SerializeField] private SkinnedMeshRenderer _normalSource;
    [SerializeField] private SkinnedMeshRenderer _roundSource;
    [SerializeField] private SkinnedMeshRenderer _chubbySource;

    private float _hungerTickTimer;
    private float _baseMoveSpeed;  
    private bool _slowApplied;

    public Rigidbody2D Rigid => _rigid;
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;
    public MeshRenderer MeshRenderer => _meshRenderer;
    public bool IsHungery => _ishungery;
    public bool IsResting => _isResting;
    public bool HasYawn => _hasYawned;
    public bool IsYawning => _isYawning;
    public Point Point => _currentPoint;
    public Transform FishPoint => _fishPoint;
    public Transform StorePoint => _storePoint;
    public Transform KitchenPoint => _kitchenPoint;
    public Transform RestAreaPoint => _restAreaPoint;
    public int FishingCount => _fishingCount;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
        if (PlayerData == null) PlayerData = new PlayerData();
        //테스트용
        PlayerData.SetHunger(80);
        PlayerData.SetStamina(5);
        PlayerData.SetMoveSpeed(1);
        PlayerData.SetDoongDoongStat(1000);
    }
    private void Start()
    {
        _baseMoveSpeed = PlayerData.MoveSpeed;
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

    public void SupplyAcorns()
    {
        if (!_canCook && !_shouldSell) return;
        //보급용도토리 로직
    }

    public void TryConsumeFood()
    {
        // 음식 섭취 로직
    }

    public void AddFishToStorage()
    {
        // 물고기 보관 로직
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
        if (PlayerData.Hunger <= 20)
            return new EatState(this);

        if (PlayerData.Stamina <= 10)
        {
            _currentPoint = Point.Rest;
            return new MoveState(this, _currentPoint);
        }

       // if (_canCook)
       // {
       //     _currentPoint = Point.Kitchen;
       //     return new MoveState(this, _currentPoint);
       // }

       // if (_shouldSell)
       // {
       //     _currentPoint = Point.Store;
       //     return new MoveState(this, _currentPoint);
       // }
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
