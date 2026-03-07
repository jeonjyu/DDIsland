using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("물고기 관련")]
    public float _speed = 15f;
    private float _rotationSpeed = 8f;
    private float _height;

    [Header("물고기 군집 관련")]
    private float _neighborDistance = 200f; // 근처 물고기와의 거리
    private float _separationDistance = 100f; // 충돌 방지 범위
    public int _floackID; // 물고기 군집 구분용 ID

    [Header("수조 경계 제한")]
    private float _highLimit;
    private float _screenLimit;

    [Header("물고기 시각화 관련")]
    private Image _fishImage;
    private Vector2 _moveDir;
    public bool _isGoingRight = true; // true면 우행, false면 좌행
    public float _fishScale = 0.5f;

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
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _fishImage = GetComponent<Image>();
    }
    
    public void InitManager(AquariumMgr mgr)
    {
        Manager = mgr;
    }

    private void Update()
    {
        float dt = Mathf.Min(Time.deltaTime, 0.05f);

        _velocity = Vector2.Lerp(_velocity, _targetVelocity, dt * 1.5f);
        _rectTransform.anchoredPosition += _velocity * dt;

        //Atan2 방식에서 벡터 방식으로 변경
        if (_velocity.sqrMagnitude > 0.01f)
        {
            float xHoldScale = (_velocity.x < 0) ? -_fishScale : _fishScale;
            _rectTransform.localScale = new Vector3(xHoldScale, _fishScale, 1);

            Vector2 moveDir = _velocity.normalized;

            Vector2 lookTarget = (xHoldScale < 0) ? -moveDir : moveDir;

            _rectTransform.right = Vector2.Lerp(_rectTransform.right, lookTarget, dt * _rotationSpeed);
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

    public void SetSpawn(FishDataSO data, int flockID, bool isRight)
    {
        _floackID = flockID;
        _isGoingRight = isRight;

        if (data != null)
        {
            _fishImage.sprite = data.FishImgPath_Sprite;
            _fishImage.SetNativeSize();
        }

        _moveDir = _isGoingRight ? Vector2.right : Vector2.left;
        _velocity = _moveDir * _speed;
        _targetVelocity = _velocity;
        _rectTransform.right = _moveDir;
    }


    // 업데이트에서 하면 안되는거 같아서 코루틴으로 주변 물고기 찾는거 구현
    IEnumerator FindNeighborFish()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        while (true)
        {
            if (_manager != null)
            {
                Bodis();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void Bodis()
    {
        Vector2 boundForce = Vector2.zero; // 경계에서 멀어질 때 중앙으로 돌아오게 하는 힘을 저장
        Vector2 separationForce = Vector2.zero; // 멀어지는 힘을 저장
        Vector2 alignmentForce = Vector2.zero;  // 쫒아가는 힘을 저장
        Vector2 cohesionForce = Vector2.zero;   // 모여드는 힘을 저장

        Vector2 centerPosition = Vector2.zero; // 전체 물고기의 좌표 합을 저장
        Vector2 desiredVelocity = Vector2.zero;
        int neighborCount = 0;

        float sqrNeighborDist = Mathf.Pow(_neighborDistance,2); //연산 최적화를 위해 제곱값 사용
        float sqrSeparationDist = Mathf.Pow(_separationDistance, 2); //연산 최적화를 위해 제곱값 사용

        float yRatio = Mathf.Abs(_rectTransform.anchoredPosition.y) / Manager.HighLimit;

        // 인스펙터에 노출시켜 직접 제어할 수 있게 해두기
        if (yRatio > 0.8f)
        {
            float forceStrength = (yRatio - 0.8f) * 2.0f;
            boundForce = (_rectTransform.anchoredPosition.y > 0) ? Vector2.down * forceStrength : Vector2.up * forceStrength;
        }

        foreach (var other in _manager._activeFish)
        {
            if (other == this || other._isGoingRight != _isGoingRight || other._floackID != _floackID) continue;

            Vector2 diff = _rectTransform.anchoredPosition - (other._rectTransform.anchoredPosition);
            float sqrDist = diff.sqrMagnitude;

            // 너무 가깝다면
            if (sqrDist < sqrNeighborDist && sqrDist > 0)
            {
                // 물고기의 속도를 더해서 평균 속도 계산에 사용 (쫒아 갈 때 사용)
                alignmentForce += other._velocity;
                // 물고기의 좌표를 더해서 평균 좌표 계산에 사용 (중심으로 모여서 함께 이동할 때 사용)
                centerPosition += other._rectTransform.anchoredPosition;

                neighborCount++;

                if (sqrDist < sqrSeparationDist)
                {
                    // 가까울수록 더 강하게 밀어냄
                    separationForce += diff.normalized / Mathf.Max(0.1f, Mathf.Sqrt(sqrDist));
                }
            }
        }
        //내 주변에 한명이라도 존재하면
        if (neighborCount > 0)
        {
            // 주변 물고기들이 어디로 가는지 방향 계산 (쫒아가기)
            alignmentForce /= neighborCount;

            // 주변 물고기들의 평균 좌표 계산 (중심 잡기)
            centerPosition /= neighborCount;

            // 내 위치에서 중심으로 항햐는 벡터 계산
            cohesionForce = (centerPosition - _rectTransform.anchoredPosition);

            Vector2 attendantsDir = _moveDir * 1f;

            // 가중치 조절
            desiredVelocity =
                (separationForce.normalized * 0.7f) + // 겹치지 않게
                (alignmentForce.normalized * 1.0f) +  // 같은 방향으로
                (cohesionForce.normalized * 0.5f) +   // 뭉치도록
                (attendantsDir * 1.5f) +              // 오른쪽 또는 왼쪽으로 이동
                (boundForce * 1.5f);                // 경계에서 멀어지도록

        }
        // 만약 주변에 아무도 없을수 도 있으니까...
        else
        {
            // [수정 부분] recoveryDir를 desiredVelocity에 할당
            desiredVelocity = (_moveDir + (boundForce * 1.5f));
        }

        if (desiredVelocity.sqrMagnitude > 0.001f)
        {
            _targetVelocity = desiredVelocity.normalized * _speed;
        }
    }
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

        if(_manager != null)
        {
            _manager.ReturnFish(this);
        }
    }
}
