using System.Collections;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// 물고기 군집 행동을 관리하는 스크립트
/// 물고기들이 서로를 인식하고 함께 움직이도록 하는 역할
/// </summary>
public class BackGroundFish : MonoBehaviour
{
    private AquariumMgr _manager;
    private RectTransform _rectTransform;
    private Vector2 _velocity;

    [Header("물고기 관련")]
    public float _speed = 15f;
    private float _rotationSpeed = 5f;
    private float _height;

    [Header("물고기 군집 관련")]
    private float _neighborDistance = 200f; // 근처 물고기와의 거리
    private float _separationDistance = 100f; // 충돌 방지 범위
    public int _fishID; // 물고기 군집 구분용 ID

    [Header("수조 경계 제한")]
    private float _yLimit = 400f;

    [Header("물고기 화면 관련")]
    private float _screenLimit = 1100f;
    private Vector2 _moveDir;
    public bool _isGoingRight = true; // true면 우행, false면 좌행

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
        _height = _rectTransform.rect.height;
    }
    
    public void InitManager(AquariumMgr mgr)
    {
        Manager = mgr;
        _yLimit = (Manager.SpawnArea.rect.height / 2f) - _height;
    }

    private void Update()
    {
        _rectTransform.anchoredPosition += _velocity * Time.deltaTime;

        //float targetAngle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
        //Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        //_rectTransform.rotation = Quaternion.Lerp(_rectTransform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        //Atan2 방식에서 벡터 방식으로 변경
        if (_velocity.sqrMagnitude > 0.01f)
        {
            if (_velocity.x < 0)
            {
                _rectTransform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                _rectTransform.localScale = new Vector3(1, 1, 1);
            }

            // 현재 방향
            Vector2 currentDir = _rectTransform.right;
            // 목표 방향
            Vector2 targetDir = (_velocity.x < 0) ? -_velocity.normalized : _velocity.normalized;
            // 다음 방향
            Vector2 nextDir = Vector2.Lerp(currentDir, targetDir, Time.deltaTime * _rotationSpeed);

            _rectTransform.right = nextDir;
        }

        if (_isGoingRight && _rectTransform.anchoredPosition.x > _screenLimit)
        {
            gameObject.SetActive(false); // 오른쪽 끝으로 나감
        }
        else if (!_isGoingRight && _rectTransform.anchoredPosition.x < -_screenLimit)
        {
            gameObject.SetActive(false); // 왼쪽 끝으로 나감
        }

    }
    // 업데이트에서 하면 안되는거 같아서 코루틴으로 주변 물고기 찾는거 구현
    IEnumerator FindNeighboorFish()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.2f));

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
        int neighborCount = 0;

        float sqrNeighborDist = Mathf.Pow(_neighborDistance,2); //연산 최적화를 위해 제곱값 사용
        float sqrSeparationDist = Mathf.Pow(_separationDistance, 2); //연산 최적화를 위해 제곱값 사용

        if (_rectTransform.anchoredPosition.y > _yLimit)
        {
            boundForce = Vector2.down * 2.0f; // 위로 너무 가면 아래로 힘을 줌
        }
        else if (_rectTransform.anchoredPosition.y < -_yLimit)
        {
            boundForce = Vector2.up * 2.0f; // 아래로 너무 가면 위로 힘을 줌
        }

        foreach (var other in _manager._activeFish)
        {
            if (other == this || other._isGoingRight != _isGoingRight) continue;

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

            Vector2 attendantsDir = _moveDir * 0.5f;

            // 가중치 조절
            Vector2 desiredVelocity =
                (separationForce.normalized * 1.5f) + // 겹치지 않게
                (alignmentForce.normalized * 1.0f) +  // 같은 방향으로
                (cohesionForce.normalized * 1.3f) +   // 뭉치도록
                (attendantsDir * 0.6f) +              // 오른쪽 또는 왼쪽으로 이동
                (boundForce * 2.0f);                // 경계에서 멀어지도록

            // 최종 속도에 반영
            _velocity = Vector2.Lerp(_velocity, desiredVelocity.normalized * _speed, Time.deltaTime * 3f);
        }
        // 만약 주변에 아무도 없을수 도 있으니까...
        else
        {
            Vector2 elseDir = Vector2.Lerp(_moveDir, boundForce, boundForce.magnitude).normalized;
            _velocity = Vector2.Lerp(_velocity, elseDir * _speed, Time.deltaTime * 3f);
        }
    }
    private void OnEnable()
    {
        // 얘는 매니저에서 지정해준 ID값에 따라 가는 방향 지정
        _moveDir = _isGoingRight ? Vector2.right : Vector2.left;

        // 속도 초기화
        _velocity = _moveDir * _speed;

        if (_manager != null)
        {
            StartCoroutine(FindNeighboorFish());
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
