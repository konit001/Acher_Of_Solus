using UnityEngine;

public class FloatingPatrolState : patrolState
{
    private FloatingEnemyCore floatCore => core as FloatingEnemyCore;

    public override void Do()
    {
        float leftPos = floatCore.startPoint - floatCore.patrolDistanceLeft;
        float rightPos = floatCore.startPoint + floatCore.patrolDistanceRight;

        bool hitLeft = floatCore.transform.position.x <= leftPos && floatCore.patrolDir < 0f;
        bool hitRight = floatCore.transform.position.x >= rightPos && floatCore.patrolDir > 0f;

        if (hitLeft || hitRight)
        {
            floatCore.flip();
            return;
        }

        rb.linearVelocity = new Vector2(floatCore.enemy.basespeed * floatCore.patrolDir * floatCore.moveSpeedMultiplier, 0f);
    }
}
