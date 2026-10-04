using UnityEngine;

public class MonsterAggro : MonoBehaviour
{
    [Header("Detection")]
    public GameObject detectCheck;
    public float detectRadius = 8f;
    [Tooltip("ระยะที่จะหลุดเป้า ต้องมากกว่า detectRadius เพื่อกันมอนสั่นเข้าๆ ออกๆ ที่ขอบวง")]
    public float loseSightRadius = 12f;
    public LayerMask playerLayer;

    [Header("Line of Sight")]
    public bool requireLineOfSight;
    public LayerMask sightBlockerLayer;

    [Header("Memory")]
    [Tooltip("มองไม่เห็นแล้วยังตามต่ออีกกี่วินาที")]
    public float memoryDuration = 3f;
    [Tooltip("โดนตีแล้วโกรธ/กลัวนานกี่วินาที (0 = ไม่มีวันลืม)")]
    public float provokeDuration = 8f;

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;
    public bool CanSeeNow { get; private set; }
    public bool IsProvoked => provokeTimer > 0f || provokedForever;
    public Vector2 LastKnownPosition { get; private set; }

    public Vector2 Origin => detectCheck != null ? (Vector2)detectCheck.transform.position : (Vector2)transform.position;

    public float DistanceToTarget =>
        HasTarget ? Vector2.Distance(Origin, Target.position) : float.MaxValue;

    private float memoryTimer;
    private float provokeTimer;
    private bool provokedForever;

    private void Update()
    {
        if (memoryTimer > 0f) memoryTimer -= Time.deltaTime;
        if (provokeTimer > 0f) provokeTimer -= Time.deltaTime;

        UpdateTarget();
    }

    private void UpdateTarget()
    {
        // เห็นอยู่แล้วให้ใช้วงกว้าง (loseSight) ยังไม่เห็นให้ใช้วงแคบ (detect) — hysteresis
        float radius = HasTarget ? loseSightRadius : detectRadius;
        Collider2D seen = Physics2D.OverlapCircle(Origin, radius, playerLayer);

        CanSeeNow = seen != null && HasLineOfSight(seen.transform);

        if (CanSeeNow)
        {
            Target = seen.transform;
            LastKnownPosition = Target.position;
            memoryTimer = memoryDuration;
            return;
        }

        // มองไม่เห็นแล้ว แต่ยังจำตำแหน่งสุดท้ายได้จนกว่าความจำจะหมด
        if (memoryTimer <= 0f) Target = null;
    }

    private bool HasLineOfSight(Transform candidate)
    {
        if (!requireLineOfSight) return true;

        Vector2 toTarget = (Vector2)candidate.position - Origin;
        RaycastHit2D blocker = Physics2D.Raycast(Origin, toTarget.normalized, toTarget.magnitude, sightBlockerLayer);
        return blocker.collider == null;
    }

    // เรียกตอนมอนโดนตี — Neutral จะโกรธ, Friendly จะกลัว, Aggressive ดุอยู่แล้วไม่เปลี่ยนอะไร
    public void Provoke()
    {
        if (provokeDuration <= 0f) provokedForever = true;
        else provokeTimer = Mathf.Max(provokeTimer, provokeDuration);

        if (HasTarget) return;

        // โดนยิงจากนอกระยะสายตา ก็ต้องรู้ว่าใครยิง
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Target = player.transform;
        LastKnownPosition = Target.position;
        memoryTimer = memoryDuration;
    }

    // ตำแหน่งที่มอน "เชื่อว่า" เป้าอยู่ — เห็นอยู่ใช้ของจริง ไม่เห็นใช้ที่จำไว้
    public Vector2 BelievedPosition => CanSeeNow && HasTarget ? (Vector2)Target.position : LastKnownPosition;

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = detectCheck != null ? detectCheck.transform.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, detectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, loseSightRadius);
    }
}
