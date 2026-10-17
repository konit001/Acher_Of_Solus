using UnityEngine;

public class GunAttackState : State
{
    public PlayerAttack input;

    [Header("Renderers")]
    public SpriteRenderer fullBody;
    public SpriteRenderer gunLegs;
    public SpriteRenderer gunBody;

    [Header("Clips")]
    public AnimationClip bodyShootAnim;   // Layer 1
    public AnimationClip legsIdleAnim;    // Layer 0
    public AnimationClip legsMoveAnim;    // Layer 0
    public int bodyLayerIndex = 1;

    [Header("Linger")]
    public float legsLingerTime = 0.3f;   // ขาค้างอยู่หลังท่อนบนจบ

    private bool legsMoving;
    private float bodyStartTime;
    private bool bodyPlaying;

    public override bool CanEnter() => input.isShooting;

    public override void Enter()
    {
        fullBody.enabled = false;
        gunLegs.enabled = true;
        gunBody.enabled = true;

        legsMoving = IsMoving();
        PlayLegs();
        StartBody();
    }

    public override void Do()
    {
        // ขาสลับ idle/move ตามความเร็วตลอด ไม่เกี่ยวกับท่อนบน
        bool moving = IsMoving();
        if (moving != legsMoving)
        {
            legsMoving = moving;
            PlayLegs();
        }

        // กดยิงอีกรอบ: รีสตาร์ทเฉพาะท่อนบน ขาไม่ถูกแตะ
        if (input.isShooting && (!bodyPlaying || Time.time - bodyStartTime >= bodyShootAnim.length))
        {
            StartBody();
        }

        // ท่อนบนจบแล้ว: ซ่อน body และเริ่มนับ linger
        if (bodyPlaying && Time.time - bodyStartTime >= bodyShootAnim.length)
        {
            bodyPlaying = false;
            animator.SetLayerWeight(bodyLayerIndex, 0f);
            gunBody.enabled = false;
        }

        // จบ state เมื่อท่อนบนจบ + พ้นช่วง linger
        if (!bodyPlaying && Time.time - (bodyStartTime + bodyShootAnim.length) >= legsLingerTime)
            IsComplete = true;
    }

    public override void Exit()
    {
        gunLegs.enabled = false;
        gunBody.enabled = false;
        fullBody.enabled = true;
        animator.SetLayerWeight(bodyLayerIndex, 0f);
    }

    private void StartBody()
    {
        bodyStartTime = Time.time;
        bodyPlaying = true;
        gunBody.enabled = true;
        animator.SetLayerWeight(bodyLayerIndex, 1f);
        animator.Play(bodyShootAnim.name, bodyLayerIndex, 0f);
    }

    private bool IsMoving() => Mathf.Abs(rb.linearVelocity.x) > 0.1f;

    private void PlayLegs()
    {
        var clip = legsMoving ? legsMoveAnim : legsIdleAnim;
        animator.Play(clip.name, 0, 0f);
    }
}