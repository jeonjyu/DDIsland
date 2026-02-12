using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.Rendering.DebugUI;

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
    private bool _canCook = false;
    private bool _shouldSell = false;
    private bool _isFishing = false;
    private bool _isResting = false;
    private int _fishingCount = 5;  //낚시횟수

    private Coroutine _fishingRoutine;  //낚시 코루틴

    public PlayerData PlayerData { get; private set; }

    Point _currentPoint = Point.Fish;  //플레이어 목적지
    [SerializeField] private Transform _fishPoint;  //각 지점 위치
    [SerializeField] private Transform _storePoint;
    [SerializeField] private Transform _kitchenPoint;
    [SerializeField] private Transform _restAreaPoint;

    public Rigidbody2D Rigid => _rigid;
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;
    public MeshRenderer MeshRenderer => _meshRenderer;
    public bool IsHungery => _ishungery;
    public bool _IsResting => _isResting;

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
    }

    private void Update()
    {
        if (_currentState == null)
            SetState(new IdleState(this));

        _currentState?.Execute();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedExecute();
    }

    public void TryConsumeFood()
    {
        // 음식 섭취 로직
    }

    public void AddFishToStorage()
    {
        // 물고기 보관 로직
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
        _fishingCount--;
        _isFishing = false;
        PlayerData.SetHunger(PlayerData.Hunger - 4);
        PlayerData.SetStamina(PlayerData.Stamina - 5);
        //물고기 얻기, 보관함추가등등      
    }
    public void ResetFishingCount()
    {
        _fishingCount = 5;
    }

    public void FullyRecovered()  //휴식이 다찰때까지 어떤 상태로도 전환 불가
    {
        if (_isResting) return;
        _isResting = true;

        InvokeRepeating("WaitRecovered", 0f, 1f);
    }
    private void WaitRecovered()  
    {
        PlayerData.SetStamina(PlayerData.Stamina + 2);
        if (PlayerData.Stamina >= 100)
        {
            CancelInvoke("WaitRecovered");
            _isResting = false;
            SetState(new IdleState(this));
        }
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

        if (_canCook)
        {
            _currentPoint = Point.Kitchen;
            return new MoveState(this, _currentPoint);
        }

        if (_shouldSell)
        {
            _currentPoint = Point.Store;
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
