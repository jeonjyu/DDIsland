using UnityEngine;

public class SellState : IState
{
    private readonly PlayerController _player;
    public SellState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("판매진입");
        _player.SellFishs();
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
