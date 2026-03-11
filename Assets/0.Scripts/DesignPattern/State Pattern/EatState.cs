using UnityEngine;

public class EatState : IState
{
    private readonly PlayerController _player;
    Point _desPoint;
    private Quaternion _fixedRot;
    public EatState(PlayerController player, Point destination)
    {
        _player = player;
        _desPoint = destination;
    }
    public void Enter()
    {
        _player.Agent.isStopped = true;
        _player.Agent.velocity = Vector3.zero;

        if (_desPoint == Point.Acorn)
        {
            _player.EatCurrentAcorn();
        }
        else if (_desPoint == Point.Table)
        {
            _player.Animator.SetTrigger("isEat");
        }

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
    }

    public void Exit()
    {
        _player.Agent.updateRotation = true;
        _player.Agent.updatePosition = true;
    }

    public void FixedExecute()
    {
        
    }
}
