using UnityEngine;

public class EatState : IState
{
    private readonly PlayerController _player;
    Point _desPoint;
    public EatState(PlayerController player, Point destination)
    {
        _player = player;
        _desPoint = destination;
    }
    public void Enter()
    {
        _player.Agent.isStopped = true;
        _player.Agent.velocity = Vector3.zero;

        _player.transform.SetPositionAndRotation(_player.TablePoint.position, _player.TablePoint.rotation);

        if (_desPoint == Point.Acorn)
        {
            _player.EatCurrentAcorn();
        }
        else if (_desPoint == Point.Table)
        {
            _player.Animator.SetTrigger("isEat");
        }
        

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
