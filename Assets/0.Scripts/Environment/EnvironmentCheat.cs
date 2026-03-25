#if TESTMODE
using UnityEngine;
using UnityEngine.InputSystem;

public class EnvironmentCheat : MonoBehaviour
{
    [SerializeField] private EnvironmentPresenter _presenter;

    [Header("Actions")]
    [SerializeField] private InputActionReference _toggleCheat;
    [SerializeField] private InputActionReference _changeSeason;
    [SerializeField] private InputActionReference _changeTime;
    [SerializeField] private InputActionReference _toggleParticle;

    private void OnEnable()
    {
        // 액션맵 활성화
        _toggleCheat.action.Enable();
        _changeSeason.action.Enable();
        _changeTime.action.Enable();
        _toggleParticle.action.Enable();

        _toggleCheat.action.performed += OnToggleCheat;
        _changeSeason.action.performed += OnChangeSeason;
        _changeTime.action.performed += OnChangeTime;
        _toggleParticle.action.performed += OnToggleWeather;

    }

    private void OnDisable()
    {
        _toggleCheat.action.Disable();
        _changeSeason.action.Disable();
        _changeTime.action.Disable();
        _toggleParticle.action.Disable();

        _toggleCheat.action.performed -= OnToggleCheat;
        _changeSeason.action.performed -= OnChangeSeason;
        _changeTime.action.performed -= OnChangeTime;
        _toggleParticle.action.performed -= OnToggleWeather;
    }
    private void OnToggleCheat(InputAction.CallbackContext context)
    {
        var model = _presenter.Model;
        model.IsCheatMode = !model.IsCheatMode;
    }
    private void OnChangeSeason(InputAction.CallbackContext context)
    {
        if (!_presenter.Model.IsCheatMode) return;

        // 1, 2, 3, 4 순서로 봄, 여름, 가을, 겨울
        string keyName = context.control.name;
        int seasonIndex = keyName switch
        {
            "1" => 0,
            "2" => 1,
            "3" => 2,
            "4" => 3,
            _ => -1
        };

        if (seasonIndex != -1) _presenter.Model.ForceSetSeason((Season)seasonIndex);
    }
    private void OnChangeTime(InputAction.CallbackContext context)
    {
        if (!_presenter.Model.IsCheatMode) return;

        // q = 낮, w = 저녁(노을), e = 밤
        string keyName = context.control.name.ToLower();
        int timeIndex = keyName switch
        {
            "q" => 0,
            "w" => 1,
            "e" => 2,
            _ => -1
        };

        if (timeIndex != -1) _presenter.Model.ForceSetDaily((DayilyCycle)timeIndex);
    }

    private void OnToggleWeather(InputAction.CallbackContext context)
    {
        if (!_presenter.Model.IsCheatMode) return;

        bool currentStatus = _presenter.Model.IsWeatherActive;
        _presenter.Model.ForceSetWeather(!currentStatus);

    }
}
#endif