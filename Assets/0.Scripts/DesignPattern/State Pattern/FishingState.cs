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
        _player.Animator.SetBool("isFish", true);
        _player.ResetFishingCount();
    }

    public void Execute()
    { 
        _player.TryFishing();

        if (_player.FishingCount <= 0 || _player.PlayerData.Hunger <= 0 || _player.PlayerData.Stamina <= 0)
        {
            _player.Animator.SetBool("isFish", false);
            _player.SetState(new IdleState(_player));
        }
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
