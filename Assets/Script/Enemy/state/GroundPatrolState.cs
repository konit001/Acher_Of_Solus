using UnityEngine;

public class GroundPatrolState : patrolState
{
    private GroundEnemyCore groundCore => core as GroundEnemyCore;

    public override void Do()
    {
        if (groundCore.IsMovementDisabled || groundCore.IsActionDisabled) return;

        if (groundCore.IsWall() || (!groundCore.IsGrounded()))
        {
            groundCore.flip();
            return;
        }

        rb.linearVelocity = new Vector2(groundCore.enemy.basespeed * groundCore.patrolDir, rb.linearVelocity.y);
    }
}
