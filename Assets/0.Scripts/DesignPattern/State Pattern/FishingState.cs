using UnityEngine;

public class FishingState : IState
{
    private readonly PlayerController _player;
    private Quaternion _fixedRot;

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

        Vector3 dir = (_player.FishPoint.position - _player.transform.position);  //낚시할때 바라볼 방향 고정
        dir.y = 0f;
        if (dir.magnitude > 0.0001f)
        {
            _fixedRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }
        else
        {
            _fixedRot = _player.transform.rotation;
        }
        _player.Agent.updateRotation = false;
        _player.transform.rotation = _fixedRot;
    }

    public void Execute()
    {
        _player.transform.rotation = _fixedRot;
        _player.TryFishing();
    }

    public void Exit()
    {
        _player.HandOffFishingRod();
        _player.StopFishing();
        _player.Animator.SetBool("isFish", false);
        _player.Agent.updatePosition = true;
    }

    public void FixedExecute()
    {
        
    }
}
