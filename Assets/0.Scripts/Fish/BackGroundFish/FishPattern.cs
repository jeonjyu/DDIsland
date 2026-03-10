using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
public enum SubState
{
    BaseMove, ShortMove, Wait, Interval, Enter,
}

public class FishPattern : IMovement
{
    private const float INSIDE_THE_SCREEN = 0.8f;
    private const float BASE_SPEED = 50f;
    private const float PLUS_SPEED = 20f;
    private const float RADIUS = 100f;
    private const float WAIT_TIME = 3f;
    private const float INTERVAL_TIME = 1f;

    public Vector2 GetTargetVelocity(BackGroundFish fish, float dt)
    {
        var data = fish._patternData;

        Vector2 fishVelocity = Vector2.zero;
        Vector2 forwardDir = fish._isGoingRight ? Vector2.right : Vector2.left;

        if (data._currentState == SubState.Enter)
        {
            float posX = fish.FishTransform.anchoredPosition.x;
            float entry = fish.Manager.ScreenLimit * INSIDE_THE_SCREEN;

            // 화면 안으로 충분히 들어왔다면 패턴 시작
            if (Mathf.Abs(posX) < entry)
            {
                SelectNewPattern(data);
            }
            fishVelocity = forwardDir * BASE_SPEED;
        }
        else
        {
            data._stateTimer -= dt;

            switch (data._currentState)
            {
                case SubState.BaseMove:
                    fishVelocity = forwardDir * BASE_SPEED;
                    break;

                case SubState.ShortMove:
                    if (!data._isSubTargetSet) SetNewShortTarget(fish, data);

                    Vector2 toTarget = (data._subTargetPos - fish.FishTransform.anchoredPosition);

                    float dir = toTarget.magnitude;

                    if (dir < 30f)
                    {
                        data._isSubTargetSet = false;
                        data._shortMoveCount++;

                        if (data._shortMoveCount >= 3) SelectNewPattern(data);
                        else
                        {
                            data._currentState = SubState.Interval;
                            data._stateTimer = INTERVAL_TIME;
                        }
                        fishVelocity = forwardDir * BASE_SPEED;
                    }
                    else
                    {
                        if (Vector2.Dot(forwardDir, toTarget.normalized) < 0.1f)
                        {
                            fishVelocity = forwardDir * (BASE_SPEED + PLUS_SPEED);
                            data._isSubTargetSet = false;
                        }
                        else
                        {
                            fishVelocity = toTarget.normalized * (BASE_SPEED + PLUS_SPEED);
                        }
                    }
                    break;

                case SubState.Interval:
                    if (data._stateTimer <= 0)
                    {
                        data._currentState = SubState.ShortMove;
                    }
                    return Vector2.zero;

                case SubState.Wait:

                    if (data._stateTimer <= 0) SelectNewPattern(data);
                    return Vector2.zero;
            }
        }

        float smoothness = 0.1f;
        data._currentVelocity = Vector2.Lerp(data._currentVelocity, fishVelocity, smoothness);

        return data._currentVelocity;
    }

    private void SelectNewPattern(FishStateData data)
    {
        float rand = Random.value;
        if (rand < 0.35f) data._currentState = SubState.BaseMove;
        else if (rand < 0.90f) { data._currentState = SubState.ShortMove; data._shortMoveCount = 0; }
        else { data._currentState = SubState.Wait; data._stateTimer = WAIT_TIME; }

        data._isSubTargetSet = false;
    }

    private void SetNewShortTarget(BackGroundFish fish, FishStateData data)
    {
        float angle = Random.Range(-30f, 30f); // 전방 반원 각도
        float radian = angle * Mathf.Deg2Rad;

        Vector2 forward = fish._isGoingRight ? Vector2.right : Vector2.left;
        float baseAngle = Mathf.Atan2(forward.y, forward.x);

        float finalAngle = baseAngle + radian;
        Vector2 offset = new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle)) * RADIUS;

        data._subTargetPos = fish.FishTransform.anchoredPosition + offset;

        float limitY = fish.Manager.HighLimit - 50f;
        data._subTargetPos.y = Mathf.Clamp(data._subTargetPos.y, -limitY, limitY);

        data._isSubTargetSet = true;
    }
}


