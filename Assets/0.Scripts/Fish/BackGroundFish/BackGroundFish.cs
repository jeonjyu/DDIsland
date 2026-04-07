using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 일반 물고기의 패턴을 위해 각 물고기들의 패턴을 보관
/// </summary>
public class FishStateData
{
    public SubState _currentState = SubState.Enter;
    public float _stateTimer;
    public int _shortMoveCount;
    public Vector2 _subTargetPos;
    public Vector2 _currentVelocity;
    public bool _isSubTargetSet;


    public void Reset()
    {
        _currentState = SubState.Enter;
        _stateTimer = 0f;
        _shortMoveCount = 0;
        _isSubTargetSet = false;
        _currentVelocity = Vector2.zero;
    }
}

/// <summary>
/// 물고기 군집 행동을 관리하는 스크립트
/// 물고기들이 서로를 인식하고 함께 움직이도록 하는 역할
/// </summary>
public class BackGroundFish : MonoBehaviour
{
    private AquariumMgr _manager;
    private RectTransform _rectTransform;
    private Vector2 _velocity;
    private Vector2 _targetVelocity;
    private IMovement _movementPattern;
    private Animator _animator;
    public FishStateData _patternData = new();

    [Header("물고기 관련")]
    public float _speed = 15f;
    private float _rotationSpeed = 4f;
    [SerializeField] private float _spawnTime = 0.2f;

    [Header("물고기 군집 관련")]
    private float _neighborDistance = 250f; // 근처 물고기와의 거리
    private float _separationDistance = 100f; // 충돌 방지 범위
    public int _floackID; // 물고기 군집 구분용 ID

    [Header("수조 경계 제한")]
    [Range(0, 1)]
    [SerializeField] float _upperFishLimited;
    [Range(0, 1)]
    [SerializeField] float _lowerFishLimited;

    [Header("물고기 시각화 관련")]
    private Image _fishImage;
    private Vector2 _moveDir;
    public bool _isGoingRight = true; // true면 우행, false면 좌행
    public float _fishScale = 0.15f;

    #region 프로퍼티
    public Vector2 CurrentVelocity => _velocity;
    public Vector2 MoveDir => _moveDir;
    public float NeighborDistance => _neighborDistance;
    public float SeparationDistance => _separationDistance;
    public float UpsserFish => _upperFishLimited;
    public float LowerFish => _lowerFishLimited;
    public AquariumMgr Manager
    {
        get { return _manager; }
        private set { _manager = value; }
    }
    public RectTransform FishTransform
    {
        get { return _rectTransform; }
        set { _rectTransform = value; }
    }
    #endregion
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _fishImage = GetComponent<Image>();
        _animator = GetComponent<Animator>();
    }

    public void InitManager(AquariumMgr mgr)
    {
        Manager = mgr;
    }

    private void Update()
    {
        float dt = Mathf.Min(Time.deltaTime, 0.05f);

        _velocity = Vector2.Lerp(_velocity, _targetVelocity, dt * 1.5f);

        //float waveY = Mathf.Sin(Time.time * 1.5f + _floackID) * 15f;
        //Vector2 waveOffset = Vector2.up * waveY;

        _rectTransform.anchoredPosition += (_velocity) * dt;

        // 다시 Atan2 사용(짐벌락 방지)
        float sqrMag = _velocity.sqrMagnitude;
        if (sqrMag > 0.01f)
        {
            float targetAngle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
            float xRotation = (_velocity.x < 0) ? 180f : 0f;
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

            if (Quaternion.Angle(_rectTransform.localRotation, targetRot) > 0.5f)
            {
                _rectTransform.localRotation = Quaternion.Slerp(
                    _rectTransform.localRotation,
                    targetRot,
                    dt * _rotationSpeed);
            }

            float expectedYScale = (_velocity.x < 0) ? -_fishScale : _fishScale;

            Vector3 targetScale = new (_fishScale, expectedYScale, 1f);

            if (_rectTransform.localScale != targetScale)
            {
                _rectTransform.localScale = targetScale;
            }
        }

        if (_isGoingRight && _rectTransform.anchoredPosition.x > Manager.ScreenLimit)
        {
            gameObject.SetActive(false);
        }
        else if (!_isGoingRight && _rectTransform.anchoredPosition.x < -Manager.ScreenLimit)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetSpawn(FishDataSO data, int flockID, bool isRight, IMovement movement)
    {
        _patternData.Reset(); // 소환할 때 기록 초기화

        _floackID = flockID;
        _isGoingRight = isRight;
        _movementPattern = movement;

        float baseSpeed = 50f;
        float randomSpeedFactor = Random.Range(0.9f, 1.1f);
        _speed = baseSpeed * randomSpeedFactor;

        if (data != null)
        {
            _fishImage.sprite = data.FishImgPath_Sprite;

            if (data.FishAnimPath_AnimatorOverrideController != null)
            {
                _animator.runtimeAnimatorController = data.FishAnimPath_AnimatorOverrideController;
                _animator.enabled = true;
                _animator.Rebind();

                _animator.Update(0f);
            }
            else
            {
                _animator.enabled = false;
            }


            _fishImage.SetNativeSize();
            _rectTransform.localScale = new Vector3(_fishScale, _fishScale, 1f);
        }

        _moveDir = _isGoingRight ? Vector2.right : Vector2.left;
        _velocity = _moveDir * _speed;
        _targetVelocity = _velocity;
        _rectTransform.right = _moveDir;

        StopAllCoroutines();
        StartCoroutine(FindNeighborFish());
    }


    #region 물고기 군집 관련
    // 업데이트에서 하면 안되는거 같아서 코루틴으로 주변 물고기 찾는거 구현
    IEnumerator FindNeighborFish()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        while (true)
        {
            if (_manager != null)
            {
                _targetVelocity = _movementPattern.GetTargetVelocity(this, _spawnTime);
            }
            yield return new WaitForSeconds(_spawnTime + Random.Range(-0.02f,0.02f));
        }
    }
    #endregion

    private void OnEnable()
    {
        if (_manager != null)
        {
            StartCoroutine(FindNeighborFish());
        }
    }
    private void OnDisable()
    {
        //코루틴 멈추기 ( 메모리 아끼기)
        StopAllCoroutines();

        if (_manager != null)
        {
            _manager.ReturnFish(this);
        }
    }
}
