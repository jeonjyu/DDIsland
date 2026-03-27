using UnityEngine;

public class FishBoids : IMovement
{
    // 기존에 있던 BackGroundFish의 군집 패턴을 여기로 이동
    public Vector2 GetTargetVelocity(BackGroundFish fish, float no =0) //여긴 float값 안씀
    {
        Vector2 boundForce = Vector2.zero; // 경계에서 멀어질 때 중앙으로 돌아오게 하는 힘을 저장
        Vector2 separationForce = Vector2.zero; // 멀어지는 힘을 저장
        Vector2 alignmentForce = Vector2.zero;  // 쫒아가는 힘을 저장
        Vector2 cohesionForce = Vector2.zero;   // 모여드는 힘을 저장

        Vector2 centerPosition = Vector2.zero; // 전체 물고기의 좌표 합을 저장
        Vector2 desiredVelocity = Vector2.zero;
        int neighborCount = 0;

        float currentY = fish.FishTransform.anchoredPosition.y;

        float sqrNeighborDist = Mathf.Pow(fish.NeighborDistance, 2); //연산 최적화를 위해 제곱값 사용
        float sqrSeparationDist = Mathf.Pow(fish.SeparationDistance, 2); //연산 최적화를 위해 제곱값 사용

        float yRatio = Mathf.Abs(fish.FishTransform.anchoredPosition.y) / fish.Manager.HighLimit;
        // 인스펙터에 노출시켜 직접 제어할 수 있게 해두기
        if (currentY > 0 && yRatio > fish.UpsserFish) // 위쪽 경계 감지
        {
            float forceStrength = (yRatio - fish.UpsserFish) * 10f;
            boundForce = Vector2.down * forceStrength;
        }
        else if (currentY < 0 && yRatio > fish.LowerFish) // 아래쪽 경계 감지
        {
            float forceStrength = (yRatio - fish.LowerFish) * 10f;
            boundForce = Vector2.up * forceStrength;
        }

        float wanderNoise = Mathf.PerlinNoise(Time.time * 0.5f, fish.gameObject.GetInstanceID());
        float wanderAngle = wanderNoise * Mathf.PI * 4f;
        Vector2 wanderForce = new (Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle));

        foreach (var other in fish.Manager._activeFish)
        {
            if (other == fish || other._isGoingRight != fish._isGoingRight || other._floackID != fish._floackID) continue;

            Vector2 diff = fish.FishTransform.anchoredPosition - (other.FishTransform.anchoredPosition);
            float sqrDist = diff.sqrMagnitude;

            // 너무 가깝다면
            if (sqrDist < sqrNeighborDist && sqrDist > 0)
            {
                // 물고기의 속도를 더해서 평균 속도 계산에 사용 (쫒아 갈 때 사용)
                alignmentForce += other.CurrentVelocity;
                // 물고기의 좌표를 더해서 평균 좌표 계산에 사용 (중심으로 모여서 함께 이동할 때 사용)
                centerPosition += other.FishTransform.anchoredPosition;

                neighborCount++;

                if (sqrDist < sqrSeparationDist)
                {
                    // 가까울수록 더 강하게 밀어냄
                    separationForce += diff.normalized / Mathf.Max(0.1f, Mathf.Sqrt(sqrDist));
                }
            }
        }
        //내 주변에 한명이라도 존재하면
        if (neighborCount > 0)
        {
            // 주변 물고기들이 어디로 가는지 방향 계산 (쫒아가기)
            alignmentForce /= neighborCount;

            // 주변 물고기들의 평균 좌표 계산 (중심 잡기)
            centerPosition /= neighborCount;

            // 내 위치에서 중심으로 항햐는 벡터 계산
            cohesionForce = (centerPosition - fish.FishTransform.anchoredPosition);

            Vector2 attendantsDir = fish.MoveDir * 1f;

            // 가중치 조절
            desiredVelocity =
                (separationForce.normalized * 1.5f) + // 겹치지 않게
                (alignmentForce.normalized * 1.0f) +  // 같은 방향으로
                (cohesionForce.normalized * 0.8f) +   // 뭉치도록
                (attendantsDir * 1.5f) +              // 오른쪽 또는 왼쪽으로 이동
                (wanderForce*0.2f);                // 경계에서 멀어지도록
            desiredVelocity += boundForce * 1.5f;
        }
        // 만약 주변에 아무도 없을수 도 있으니까...
        else
        {
            desiredVelocity = fish.MoveDir + boundForce;
        }

        return desiredVelocity.normalized * fish._speed;

    }
}
