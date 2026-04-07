using UnityEngine;
using System.Collections;
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
        _player.HandOnFork();
        if (_desPoint == Point.Acorn)
        {
            _player.StartCoroutine(CoEatAcorn());
        }
        else if (_desPoint == Point.Table)
        {
            _player.transform.SetPositionAndRotation(_player.TablePoint.position, _player.TablePoint.rotation);
            if (!_player.PrepareEatFood())
            {
                _player.SetState(new IdleState(_player));
                return;
            }
            _player.Animator.SetTrigger("isEat");
        }
        

    }
    private IEnumerator CoEatAcorn()
    {
        yield return null;
        _player.EatCurrentAcorn();
    }
    public void Execute()
    {

    }

    public void Exit()
    {
        _player.HandOffFork();
        _player.HideFood3DModel();
    }

    public void FixedExecute()
    {
        
    }
}
