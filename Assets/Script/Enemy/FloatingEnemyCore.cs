using UnityEngine;

public class FloatingEnemyCore : Enemy
{
    [Header("Patrol Settings")]
    public float patrolDistanceLeft;
    public float patrolDistanceRight;

    protected override void Awake()
    {
        base.Awake();
        rb.gravityScale = 0f;
    }

    protected override void OnDrawGizmosSelected()
    {
        // patrol length
        Gizmos.color = Color.cyan;
        float originPoint = Application.isPlaying ? startPoint : transform.position.x;
        Vector3 leftPos = new Vector3(originPoint - patrolDistanceLeft, transform.position.y + 1, transform.position.z);
        Vector3 rightPos = new Vector3(originPoint + patrolDistanceRight, transform.position.y + 1, transform.position.z);
        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawLine(leftPos + Vector3.up * 0.5f, leftPos + Vector3.down * 0.5f);
        Gizmos.DrawLine(rightPos + Vector3.up * 0.5f, rightPos + Vector3.down * 0.5f);
    }
}
