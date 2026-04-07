using UnityEngine;

public class CookState : IState
{
    private readonly PlayerController _player;
    public CookState(PlayerController player)
    {
        _player = player;
    }
    public void Enter()
    {
        if (!_player.CanCook) { Debug.LogWarning("요리 불가라 Idle 복귀"); _player.SetState(new IdleState(_player)); return; }  //혹시 모르니까 탈출기능
        _player.Agent.isStopped = true;
        _player.Agent.ResetPath();
        _player.Agent.velocity = Vector3.zero;
        _player.HandOnPan();

        _player.transform.SetPositionAndRotation(_player.KitchenPoint.position, _player.KitchenPoint.rotation);

        Debug.Log("요리진입");
        bool started = _player.TryCooking();
        if (!started)
        {
            Debug.LogWarning("요리 시작 실패 => Idle 복귀");
            _player.SetState(new IdleState(_player));
            return;
        }

    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _player.HandOffPan();
    }

    public void FixedExecute()
    {
  
    }
}
