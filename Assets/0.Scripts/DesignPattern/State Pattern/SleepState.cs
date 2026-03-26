using UnityEngine;

public class SleepState : IState
{
    private readonly PlayerController _player;
    public SleepState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("수면진입");
        _player.Agent.ResetPath();
        _player.Agent.velocity = Vector3.zero;
        _player.Agent.updateRotation = false;


        _player.transform.SetPositionAndRotation(_player.RestAreaPoint.position, _player.RestAreaPoint.rotation);
        _player.Animator.SetBool("isSleep", true);
        _player.StartRecover();
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _player.StopRecover();
        _player.Animator.SetBool("isSleep", false);
        _player.Agent.updateRotation = true;
        _player.Agent.isStopped = false;
    }

    public void FixedExecute()
    {
       
    }

}
