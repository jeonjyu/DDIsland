using UnityEngine;

public class FishMovementPattern : MonoBehaviour
{
    private enum Pattern
    {
        CrossLake,   // 기본 이동(화면 끝으로 가서 디스폰)
        ShortMove,   // 반경 랜덤 이동 + 대기, 3회 반복
        IdleWait     // 제자리 대기
    }
    private Pattern _currentPattern;

    [Header("Refs")]
    [SerializeField] private RectTransform container;   //부모
    private RectTransform rect;

    public float _moveSpeed = 100f;
    public float _plusMoveSpeed = 60f;

    [Range(0, 100)] public int _crossLakeChance = 35;
    [Range(0, 100)] public int _shortMoveChance = 55;
    [Range(0, 100)] public int _idleWaitChance = 10;
    public float _crossLakeTargetViewportX = -0.2f; // 화면 밖까지 나가게
    private Vector3 _targetPos;
    private int _shortMoveDoneCount;
    private float _waitTimer;

    [Header("Short Move Settings")]
    public float _radius = 100f;     
    public float _waitTime = 1.0f;   // 목표 도달 후 대기
    public int _shortMoveRepeat = 3;

    [Header("Idle Wait Settings")]
    public float _delayTime = 3.0f;

    [Header("Target Y Lanes")]
    public bool _useYLanes = false;
    public float[] _yLanes; // 만약 위아래중간 정렬해서 움직이게할고시프면

    public bool _useAcceleration = false;

    private void Awake()
    {
        rect = (RectTransform)transform;


    }
    private void OnEnable()
    {
        PickNextPattern();
    }
    private void Update()
    {
        switch (_currentPattern)
        {
            case Pattern.CrossLake:
                UpdateCrossLake();
                break;
            case Pattern.ShortMove:
                UpdateShortMove();
                break;
            case Pattern.IdleWait:
                UpdateIdleWait();
                break;
        }
       
       
        if (_currentPattern == Pattern.CrossLake && IsOutsideContainer())
         {
            gameObject.SetActive(false);
        }
       
    }
    private void PickNextPattern()
    {
        _currentPattern = WeightedPick();
        _shortMoveDoneCount = 0;
        _waitTimer = 0f;
        if (_currentPattern == Pattern.CrossLake)
        {
            _targetPos = MakeCrossLakeTarget();
        }
        else if (_currentPattern == Pattern.ShortMove)
        {
            _targetPos = MakeRandomTargetInRadius();
        }
        else //IdleWait
        {
            // 그냥 대기 시작
        }
    }
    private Pattern WeightedPick()
    {
        int total = _crossLakeChance + _shortMoveChance + _idleWaitChance;
        if (total <= 0) return Pattern.ShortMove;

        int r = Random.Range(0, total);
        if (r < _crossLakeChance) return Pattern.CrossLake;
        if (r < _crossLakeChance + _shortMoveChance) return Pattern.ShortMove;
        return Pattern.IdleWait;
    }
    private Vector3 MakeCrossLakeTarget()  // 화면 반대편 밖으로 목표 지점 설정
    {
        // 왼=>오 또는 오=>왼 결정
        float halfW = container.rect.width * 0.5f;
        float halfH = container.rect.height * 0.5f;

        Vector2 pos = rect.anchoredPosition;
        float outX = (pos.x < 0f) ? (halfW + 80f) : (-halfW - 80f);

        float y = _useYLanes ? PickLaneY() : pos.y;
        y = Mathf.Clamp(y, -halfH + 20f, halfH - 20f);

        return new Vector2(outX, y);
    }
    private Vector3 MakeRandomTargetInRadius()  // 현재 위치기준반경 내 랜덤지점생성, y는 레인 쓰면 레인, 아니면 현재 y 유지
    {
        float halfW = container.rect.width * 0.5f;
        float halfH = container.rect.height * 0.5f;

        Vector2 rnd = Random.insideUnitCircle * _radius;
        Vector2 target = rect.anchoredPosition + rnd;

        if (_useYLanes) target.y = PickLaneY();

        // 패널 안쪽으로
        target.x = Mathf.Clamp(target.x, -halfW + 20f, halfW - 20f);
        target.y = Mathf.Clamp(target.y, -halfH + 20f, halfH - 20f);
        return target;
    }
    private float PickLaneY()
    {
        if (_yLanes == null || _yLanes.Length == 0) return transform.position.y;
        int index = Random.Range(0, _yLanes.Length);
        return _yLanes[index];
    }
    private void MoveTo(Vector2 target, float speed)
    {
        rect.anchoredPosition = Vector2.MoveTowards(
            rect.anchoredPosition,
            target,
            speed * Time.deltaTime
        );
    }

    private void UpdateShortMove()
    {
        //1 이동중
        if (!IsArrived(_targetPos))
        {
            MoveTo(_targetPos, _moveSpeed + _plusMoveSpeed);
            return;
        }

        //2 도착후 1초대기
        _waitTimer += Time.deltaTime;
        if (_waitTimer < _waitTime) return;

        //3 다음목표 생정
        _waitTimer = 0f;
        _shortMoveDoneCount++;

        if (_shortMoveDoneCount >= _shortMoveRepeat)
        {
            //3번 반복 후 새패턴 
            PickNextPattern();
            return;
        }
        _targetPos = MakeRandomTargetInRadius();
    }
    private void UpdateIdleWait()
    {
        _waitTimer += Time.deltaTime;
        if (_waitTimer >= _delayTime)
        {
            PickNextPattern();
        }
    }
    private void UpdateCrossLake()
    {
        MoveTo(_targetPos, _moveSpeed);
    }
    private bool IsOutsideContainer()  // 화면 밖으로 나갔는지 체크
    {
        float halfW = container.rect.width * 0.5f;
        float halfH = container.rect.height * 0.5f;

        Vector2 p = rect.anchoredPosition;
        return (p.x < -halfW - 50f || p.x > halfW + 50f || p.y < -halfH - 50f || p.y > halfH + 50f);
    }
    private bool IsArrived(Vector2 target)  
    {
        return (rect.anchoredPosition - target).sqrMagnitude < 4f;
    }
}
