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
        if (!_player.CanCook) { _player.SetState(new IdleState(_player)); return; }  //혹시 모르니까 탈출기능
        _player.Agent.isStopped = true;
        _player.Agent.ResetPath();
        _player.Agent.velocity = Vector3.zero;

        _player.transform.SetPositionAndRotation(_player.KitchenPoint.position, _player.KitchenPoint.rotation);

        Debug.Log("요리진입");
        _player.TryCooking();

    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }

    public void FixedExecute()
    {
  
    }
}
