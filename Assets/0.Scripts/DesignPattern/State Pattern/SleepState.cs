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
        _player.Animator.SetBool("isSleep", true);
    }

    public void Execute()
    {
        _player.FullyRecovered();
    }

    public void Exit()
    {
        _player.CancelInvoke("WaitRecovered");
        _player.Animator.SetBool("isSleep", false);
    }

    public void FixedExecute()
    {
       
    }

}
