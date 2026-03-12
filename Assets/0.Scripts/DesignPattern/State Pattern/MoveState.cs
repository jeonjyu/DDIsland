using UnityEngine;

public class MoveState : IState
{
    private readonly PlayerController _player;
    Point _desPoint;
    public MoveState(PlayerController player, Point destination)
    {
        _player = player;
        _desPoint = destination;
    }
    private Transform GetTarget()
    {
        switch (_desPoint)
        {
            case Point.Fish:
                return _player.FishPoint;
            case Point.Kitchen:
                return _player.KitchenPoint;
            case Point.Acorn:
                return _player.AcornPoint;
            case Point.Table:
                return _player.TablePoint;
            case Point.Rest:
                return _player.RestAreaPoint;
            case Point.Sell:
                return _player.SellPoint;
        }

        return null;
    }


    public void Enter()
    {
        Debug.Log("무브진입");
        _player.Agent.stoppingDistance = 0.6f;
        _player.Animator.applyRootMotion = false;

        Transform target = GetTarget();
        if (target == null)
        {
            Debug.LogWarning($"MoveState target NULL / 목적지 = {_desPoint}");
            _player.SetState(new IdleState(_player));
            return;
        }
        float dist = Vector3.Distance(_player.transform.position, target.position);
        if (dist <= _player.Agent.stoppingDistance + 0.05f)  //이미도착한 거리면 바로 그상태 보내버리기
        {
            _player.Agent.isStopped = true;
            _player.Agent.ResetPath();
            _player.Agent.velocity = Vector3.zero;

            switch (_desPoint)
            {
                case Point.Fish: _player.SetState(new FishingState(_player)); break;
                case Point.Kitchen: _player.SetState(new CookState(_player)); break;
                case Point.Acorn: _player.SetState(new EatState(_player, _desPoint)); break;
                case Point.Table: _player.SetState(new EatState(_player, _desPoint)); break;
                case Point.Rest: _player.SetState(new SleepState(_player)); break;
            }
            return;
        }

        _player.Agent.isStopped = false;
        _player.Agent.SetDestination(target.position);
        _player.Animator.SetBool("isMove", true);
        _player.ApplyMoveSpeed();
    }

    public void Execute()
    {
        _player.ApplyMoveSpeed();
        Transform target = GetTarget();
        if (_desPoint == Point.Acorn && target == null)
        {
            Debug.LogWarning("도토리 타겟이 사라져서 Idle로 복귀");
            _player.SetState(new IdleState(_player));
            return;
        }

        if (!_player.Agent.pathPending &&
            _player.Agent.remainingDistance <= _player.Agent.stoppingDistance &&
            _player.Agent.velocity.sqrMagnitude < 0.05f)
        {
            ChangeToNextState();
        }
    }
    private void ChangeToNextState()
    {
        switch (_desPoint)
        {
            case Point.Fish:
                _player.SetState(new FishingState(_player));
                break;
            case Point.Kitchen:
                _player.SetState(new CookState(_player));
                break;
            case Point.Acorn:
                _player.SetState(new EatState(_player, _desPoint));
                break;
            case Point.Table:
                _player.SetState(new EatState(_player, _desPoint));
                break;
            case Point.Rest:
                _player.SetState(new SleepState(_player));
                break;
            case Point.Sell:
                _player.SetState(new SellState(_player));
                break;
        }
    }
    public void Exit()
    {
        _player.Animator.SetBool("isMove", false);
        _player.Animator.applyRootMotion = false;
    }

    public void FixedExecute()
    { 

    }

    
}
