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
        _player.Animator.SetBool("isSleep", true);


        _player.transform.SetPositionAndRotation(_player.RestAreaPoint.position, _player.RestAreaPoint.rotation);

        _player.StartRecover();
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _player.StopRecover();
        _player.Animator.SetBool("isSleep", false);
    }

    public void FixedExecute()
    {
       
    }

}
