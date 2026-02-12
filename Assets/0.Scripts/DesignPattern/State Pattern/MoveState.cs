using UnityEngine;

public class MoveState : IState
{
    private readonly PlayerController _player;
    Point _desPoint;
    float _moveSpeed;
    Transform target;
    public MoveState(PlayerController player, Point destination)
    {
        _player = player;
        _moveSpeed = player.PlayerData.MoveSpeed;
        _desPoint = destination;
        switch (_desPoint)  //받아온 Point에 따른 타겟설정
        {
            case Point.Fish:
                target = _player.FishPoint;
                break;
            case Point.Store:
                target = _player.StorePoint;
                break;
            case Point.Kitchen:
                target = _player.KitchenPoint;
                break;
            case Point.Rest:
                target = _player.RestAreaPoint;
                break;
        }
    }

    public void Enter()
    {
        _player.Agent.SetDestination(target.position);
        _player.Animator.SetBool("isMove", true);
        if (_player.IsHungery)
        {
            _player.PlayerData.SetMoveSpeed(_moveSpeed / 2);
            _player.Agent.speed = _moveSpeed / 2;
        }
        else
        {
            _player.PlayerData.SetMoveSpeed(_moveSpeed);
            _player.Agent.speed = _moveSpeed;
        }

   
    }

    public void Execute()
    {
        if (!_player.Agent.pathPending && _player.Agent.remainingDistance <= _player.Agent.stoppingDistance)  //받아온 Point에 따른 상태전환
        {
            switch (_desPoint)
            {
                case Point.Fish:
                    _player.SetState(new FishingState(_player));
                    break;
                case Point.Store:
                    _player.SetState(new SalesState(_player));
                    break;
                case Point.Kitchen:
                    //_player.SetState(new CookState(_player));  //먹는상태 있어야할거같은
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
