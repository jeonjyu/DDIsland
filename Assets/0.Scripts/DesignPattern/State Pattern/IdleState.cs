using UnityEngine;

public class IdleState : IState
{
    private readonly PlayerController _player;

    private float _idleTimer;
    private float _idleDuration;

    private float _yawnTimer;
    private float _nextYawnTime;

    public IdleState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("아이들진입");
        _player.Agent.isStopped = true;
        _player.Agent.velocity = Vector3.zero;

        _player.Animator.SetBool("isIdle", true);

        _idleTimer = 0f;
        _idleDuration = Random.Range(2f, 5.0f);

   
        _yawnTimer = 0f;
        _nextYawnTime = Random.Range(3.0f, 8.0f);
    }

    public void Execute()
    {
        if (!_player.IsYawning)
        {
            _yawnTimer += Time.deltaTime;

            if (_yawnTimer >= _nextYawnTime)
            {
                _player.HasYawned();
                _player.Animator.SetTrigger("Yawn");

                _yawnTimer = 0f;
                _nextYawnTime = Random.Range(3f, 8f);
            }
        }


        // 잠깐 쉬었다가
        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _idleDuration && !_player.IsYawning) _player.RequestReplan();  //Idle에서 벗어나 수치에따른 다음 상태 결정
    }

    public void FixedExecute()
    {

    }

    public void Exit()
    {
        _player.Animator.SetBool("isIdle", false);
    }
}
