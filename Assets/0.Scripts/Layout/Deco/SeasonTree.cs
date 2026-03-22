using System.Collections;
using UnityEngine;

public class SeasonTree : MonoBehaviour
{
    [SerializeField] private GameObject[] _seasonModels = new GameObject[4];
    [SerializeField] private EnvironmentPresenter _envPresenter;

    private int _ditherFactorId = Shader.PropertyToID("_DitherFactor"); // 쉐이더 프로퍼티 이름 맞출 것
    private MaterialPropertyBlock _mpb;
    private float _fadeDuration = 1.0f;

    private Renderer[][] _allRenderers = new Renderer[4][];

    private Renderer[] _previousRenderers;
    private Renderer[] _currentRenderers;
    private Coroutine _fadeCoroutine;

    private int _currentSeasonIndex = -1;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();

        for (int i = 0; i < _seasonModels.Length; i++)
        {
            if (_seasonModels[i] != null)
            {
                _allRenderers[i] = _seasonModels[i].GetComponentsInChildren<Renderer>(true);
                _seasonModels[i].SetActive(false);
            }
        }
    }

    private void Start()
    {
        if (_envPresenter != null && _envPresenter.Model != null)
        {
            _envPresenter.Model.OnSeasonChanged += ChangeSeason;

            ChangeSeason(_envPresenter.Model.CurrentSeason);
        }
    }
    private void OnDisable()
    {
        if (_envPresenter != null && _envPresenter.Model != null)
        {
            _envPresenter.Model.OnSeasonChanged -= ChangeSeason;
        }

    }

    public void ChangeSeason(Season newSeasonIndex)
    {
        int newindex = (int)newSeasonIndex;
        if (newindex == _currentSeasonIndex || _fadeCoroutine != null) return;

        if (_currentSeasonIndex == -1)
        {
            _currentSeasonIndex = newindex;
            Debug.Log($"<color=cyan>[SeasonTree] 1. 초기화 실행: {newSeasonIndex} (시간: {Time.timeSinceLevelLoad})</color>");
            _seasonModels[_currentSeasonIndex].SetActive(true);

            _currentRenderers = _allRenderers[_currentSeasonIndex];

            _mpb.SetFloat(_ditherFactorId, 1f);
            SetPropertyBlockToRenderers(_currentRenderers);
            return;
        }

        if (_seasonModels[_currentSeasonIndex] == _seasonModels[newindex])
        {
            _currentSeasonIndex = newindex;
            Debug.Log($"<color=white>[SeasonTree] 2. 모델 공유 중 (변경 안함): {newSeasonIndex}</color>");
            return;
        }

        if (Time.timeSinceLevelLoad < 0.5f)
        {
            Debug.Log($"<color=yellow>[SeasonTree] 3. 데이터 로드 감지 (즉시 교체): {newSeasonIndex} (시간: {Time.timeSinceLevelLoad})</color>");
            _seasonModels[_currentSeasonIndex].SetActive(false);
            _currentSeasonIndex = newindex;
            _seasonModels[_currentSeasonIndex].SetActive(true);
            _currentRenderers = _allRenderers[_currentSeasonIndex];
            _mpb.SetFloat(_ditherFactorId, 1f);
            SetPropertyBlockToRenderers(_currentRenderers);
            return;
        }

        Debug.Log($"<color=lime>[SeasonTree] 4. 정상 페이드 시작: {_currentSeasonIndex} -> {newSeasonIndex}</color>");
        _previousRenderers = _currentRenderers;

        _seasonModels[newindex].SetActive(true);
        _currentRenderers = _allRenderers[newindex];

        _fadeCoroutine = StartCoroutine(CrossfadeRoutine(newindex));
    }

    IEnumerator CrossfadeRoutine(int newSeasonIndex)
    {
        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / _fadeDuration;

            _mpb.SetFloat(_ditherFactorId, 1f - normalizedTime);
            SetPropertyBlockToRenderers(_previousRenderers);

            _mpb.SetFloat(_ditherFactorId, normalizedTime);
            SetPropertyBlockToRenderers(_currentRenderers);

            yield return null;
        }

        _mpb.SetFloat(_ditherFactorId, 0f);
        SetPropertyBlockToRenderers(_previousRenderers);
        _seasonModels[_currentSeasonIndex].SetActive(false);

        _mpb.SetFloat(_ditherFactorId, 1f);
        SetPropertyBlockToRenderers(_currentRenderers);

        _currentSeasonIndex = newSeasonIndex;
        _fadeCoroutine = null;
    }

    private void SetPropertyBlockToRenderers(Renderer[] renderers)
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].SetPropertyBlock(_mpb);
        }
    }
}
