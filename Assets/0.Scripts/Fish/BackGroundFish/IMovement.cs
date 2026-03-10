using UnityEngine;

public interface IMovement
{
    public Vector2 GetTargetVelocity(BackGroundFish fish, float dt = 0);
}
    

