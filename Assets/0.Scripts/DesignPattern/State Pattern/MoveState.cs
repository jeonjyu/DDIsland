using UnityEngine;

public class MoveState : IState
{
    private readonly PlayerController _player;
    Point _desPoint;
    Transform target;
    public MoveState(PlayerController player, Point destination)
    {
        _player = player;
        _desPoint = destination;
        switch (_desPoint)  //받아온 Point에 따른 타겟설정
        {
            case Point.Fish:
                target = _player.FishPoint;
                break;
                //  case Point.Store:
                //      target = _player.StorePoint;
                //      break;
                // case Point.Kitchen:
                //     target = _player.KitchenPoint;
                //      break;
                 case Point.Acorn:
                    target = _player.AcornPoint;
                break;
            case Point.Rest:
                target = _player.RestAreaPoint;
                break;
        }
    }

    public void Enter()
    {
        Debug.Log("무브진입");
        _player.Agent.stoppingDistance = 0.6f;
        _player.Agent.isStopped = false;
        _player.Agent.SetDestination(target.position);
        _player.Animator.SetBool("isMove", true);
        _player.ApplyMoveSpeed();
    }

    public void Execute()
    {
        _player.ApplyMoveSpeed();
        if (!_player.Agent.pathPending && _player.Agent.remainingDistance <= _player.Agent.stoppingDistance && _player.Agent.velocity.sqrMagnitude < 0.05)  //받아온 Point에 따른 상태전환
        {
            switch (_desPoint)
            {
                case Point.Fish:
                    _player.SetState(new FishingState(_player));
                    break;
                // case Point.Store:
                //     _player.SetState(new SalesState(_player));
                //     break;
                // case Point.Kitchen:
                //     //_player.SetState(new CookState(_player));  
                //    break;
                case Point.Acorn:
                    _player.SetState(new EatState(_player));  
                   break;
                case Point.Rest:
                    _player.SetState(new SleepState(_player));
                    break;
            }
        }
    }

    public void Exit()
    {
        _player.Animator.SetBool("isMove", false);

    }

    public void FixedExecute()
    { 

    }

    
}
