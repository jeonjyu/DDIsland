using UnityEngine;

public class FishingState : IState
{
    private readonly PlayerController _player;

    public FishingState(PlayerController player)
    {
        _player = player;
    }
    public void Enter()
    {
        Debug.Log("낚시진입");
        _player.Animator.SetBool("isFish", true);
        _player.ResetFishingCount();
        _player.Agent.isStopped = true;
        _player.Agent.ResetPath();
        _player.Agent.velocity = Vector3.zero;
    }

    public void Execute()
    { 
        _player.TryFishing();
    }

    public void Exit()
    {
        _player.StopFishing();
        _player.Animator.SetBool("isFish", false);
    }

    public void FixedExecute()
    {
        
    }
}
