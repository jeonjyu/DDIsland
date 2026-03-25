using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FireBase에서 쓰레드를 받아올 때 메인 쓰레드에 연결시켜주는 역할을 담당합니다
/// </summary>
public class ThreadDispatcher : Singleton<ThreadDispatcher>
{
    // 실행할 일을 담는 Queue
    private static readonly Queue<Action> _executionQueue = new();

    public void Update()
    {
        // 이 큐를 건드릴 동안 막아놓음
        lock (_executionQueue)
        {
            // 만약 큐에 데이터가 계속 있으면 
            while (_executionQueue.Count > 0)
            {
                // 큐에서 디큐하고 실행
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
    /// <summary>
    /// 코루틴을 메인 스레드에서 실행하도록 예약
    /// </summary>
    /// <param name="action"></param>
    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() =>
            {
                StartCoroutine(action);
            });
        }
    }
    /// <summary>
    /// 보이드 메서드를 메인 스레드에서 실행하도록 예약
    /// </summary>
    /// <param name="action"></param>
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }

    // 일반 함수를 코루틴 형태로 변환
    IEnumerator ActionWrapper(Action action)
    {
        action();
        yield return null;
    }
}
