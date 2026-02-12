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
