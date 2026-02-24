using UnityEngine;

public class EatState : IState
{
    private readonly PlayerController _player;
    public EatState(PlayerController player)
    {
        _player = player;
    }
    public void Enter()
    {
        _player.Agent.isStopped = true;
        _player.Agent.velocity = Vector3.zero;
        //_player.Animator.SetTrigger("Eat");
        _player.EatCurrentAcorn();  
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
