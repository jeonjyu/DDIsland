using UnityEngine;

// 부모가 움직여도 제자리 유지
public class FixedPos : MonoBehaviour
{
    Vector3 fixedPos;

    void Start()
    {
        fixedPos = transform.position; // 시작 위치 저장
    }

    void LateUpdate()
    {
        transform.position = fixedPos; // 매 프레임 원래 위치로
    }
}
