using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FixMaker : MonoBehaviour
{
    private static readonly List<FixMaker> _allMarkers = new();

    // 외부에서 절대 수정못하게 IReadOnly 인터페이스 사용
    public static IReadOnlyList<FixMaker> AllMarkers => _allMarkers;

    [Header("인테리어 ID")]
    public int _interiorId;

    private void OnEnable()
    {
        // 마커들을 스스로 등록시켜 여기에 'ID를 받아서 소환해라!' 라는 로직을 작성할 거임
        if (!_allMarkers.Contains(this))
            _allMarkers.Add(this);
    }
    private void OnDisable()
    {
        _allMarkers.Remove(this);
    }
    private void OnDestroy()
    {
        // 할 일을 다한 애들은 쉬게 해주기
        _allMarkers.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.5f, 1.0f, 0.4f);
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 1f);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 1f);
    }
}
