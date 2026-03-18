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
        _player.HandOnFishingRod();
        _player.ResetFishingCount();
        _player.Agent.isStopped = true;
        _player.Agent.ResetPath();
        _player.Agent.velocity = Vector3.zero;
            
        _player.transform.SetPositionAndRotation(_player.FishPoint.position, _player.FishPoint.rotation);

        _player.EnterFishingState();
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _player.ExitFishingState();
        _player.HandOffFishingRod();
        _player.Animator.SetBool("isFish", false);
    }

    public void FixedExecute()
    {
        
    }
}
