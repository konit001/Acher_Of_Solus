using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ช่องสกิล 1 ช่องบน HUD: ปุ่มที่กด + สกิลที่ใส่ไว้
[System.Serializable]
public class SkillSlot
{
    public Key key = Key.E;
    public ActiveSkillData skill;

    [HideInInspector] public float cooldownTimer;
}

// คูลดาวน์เก็บที่นี่ ไม่ใช่ใน ActiveSkillData เพราะ SO เป็น asset ที่แชร์กันทุกตัวละคร
public class SkillCaster : MonoBehaviour
{
    public static SkillCaster instance;

    // ยิงเมื่อช่อง Q/E ช่องไหนถูกใส่สกิลใหม่ — ให้ UI (แผง Upgrade + HUD) รีเฟรชตาม
    public event Action<Key, ActiveSkillData> OnSlotChanged;

    [Header("References")]
    public playerControl playerController;
    public PlayerAttack attackScript;

    [Header("Skill Slots")]
    [Tooltip("ช่องสกิลตามป้ายบน HUD ปกติคือ E แล้วก็ Q")]
    public List<SkillSlot> slots = new List<SkillSlot>();

    [Header("Skill Cast State")]
    [Tooltip("ลากตัวเดียวกับที่ playerController.SkillCastState ชี้อยู่")]
    public SkillCastState skillCastState;

    // true ระหว่างที่ล็อก state เข้า SkillCastState (เฉพาะสกิลที่มี castAnim เท่านั้น)
    public bool isCasting { get; private set; }

    private ActiveSkillData pendingSkill;
    private Vector2 pendingAimDirection;

    private PlayerAttack attacker;

    private void Awake()
    {
        instance = this;
        attacker = attackScript != null ? attackScript : GetComponent<PlayerAttack>();
    }

    // ใส่สกิลลงช่องว่างช่องแรก (ไม่มีช่องว่างก็ทับช่องแรก) ให้ UI เลือกสกิลไปใช้ได้โดยไม่ต้องเจาะจงช่อง
    public bool EquipSkill(ActiveSkillData skill)
    {
        SkillSlot target = slots.Find(s => s.skill == null) ?? (slots.Count > 0 ? slots[0] : null);
        if (target == null || skill == null) return false;

        target.skill = skill;
        target.cooldownTimer = 0f;
        OnSlotChanged?.Invoke(target.key, skill);
        return true;
    }

    public ActiveSkillData GetEquippedSkill(Key key) => slots.Find(s => s.key == key)?.skill;

    void Update()
    {
        TickCooldowns();
        ReadInput();
    }

    private void TickCooldowns()
    {
        foreach (SkillSlot slot in slots)
        {
            if (slot.cooldownTimer > 0f)
                slot.cooldownTimer -= Time.deltaTime;
        }
    }

    // ปุ่ม Q/E ในเกมเพลย์ไม่มีอยู่ใน Player.inputactions จึงอ่านจาก Keyboard ตรง ๆ
    private void ReadInput()
    {
        if (Keyboard.current == null) return;

        // ตอนเปิดเมนู Q/E คือปุ่ม Previous/Next ของ UI ห้ามให้ร่ายสกิลซ้อน
        if (UiPanelController.instance != null && UiPanelController.instance.IsPanelOpen) return;

        foreach (SkillSlot slot in slots)
        {
            if (Keyboard.current[slot.key].wasPressedThisFrame)
                TryCast(slot);
        }
    }

    public void TryCast(SkillSlot slot)
    {
        if (slot.skill == null)
        {
            Debug.Log($"[Skill] ช่อง {slot.key} ว่าง");
            return;
        }

        if (slot.cooldownTimer > 0f)
        {
            Debug.Log($"[Skill] {slot.skill.skillName} ยังติดคูลดาวน์อีก {slot.cooldownTimer:F1} วิ");
            return;
        }

        // เงื่อนไขเดียวกับที่ PlayerAttack.Update ใช้กันการโจมตีซ้อน บวกด่านสถานะ (Paralyzed ปิด CanAct)
        // + isCasting กันร่ายสกิลซ้อนสกิล (re-entrant cast)
        bool blockedByAttack = attackScript != null &&
            (attackScript.isAttacking || attackScript.isShooting || !attackScript.CanAct);
        if (blockedByAttack || isCasting) return;

        Vector2 aimDirection = playerController != null ? playerController.aimDirection : Vector2.right;

        PlayFX(slot.skill);

        if (slot.skill.castAnim == null)
        {
            // ไม่มีอนิเมชัน = ยิงดาเมจ/กระสุน/บัฟทันที ไม่ล็อก state machine
            ResolveSkillEffect(slot.skill, aimDirection);
        }
        else
        {
            // มีอนิเมชัน = ล็อกเข้า SkillCastState แล้วรอ Animation Event เรียก OnCastImpact()
            StartCoroutine(CastRoutine(slot.skill, aimDirection));
        }

        slot.cooldownTimer = slot.skill.cooldown;
        Debug.Log($"[Skill] ร่าย {slot.skill.skillName} (ปุ่ม {slot.key})");
    }

    private void ResolveSkillEffect(ActiveSkillData skill, Vector2 aimDirection)
    {
        switch (skill.skillType)
        {
            case ActiveSkillType.Melee:
                PerformMeleeAttack(skill, aimDirection);
                break;

            case ActiveSkillType.Ranged:
                PerformRangedAttack(skill, aimDirection);
                break;

            case ActiveSkillType.AOE:
                PerformAOEAttack(skill);
                break;

            case ActiveSkillType.Buff:
                PerformBuff(skill);
                break;
        }
    }

    // ล้อ PlayerAttack.AttackRoutine เป๊ะ ๆ: true -> เว้น 1 เฟรมให้ selectState() รับรู้ -> รอ IsComplete -> false
    private IEnumerator CastRoutine(ActiveSkillData skill, Vector2 aimDirection)
    {
        pendingSkill = skill;
        pendingAimDirection = aimDirection;
        if (skillCastState != null) skillCastState.SetClip(skill.castAnim);

        isCasting = true;
        yield return null; // ให้ playerControl.selectState() รับรู้แล้ว Enter() เข้า SkillCastState (reset IsComplete)

        // กันเหนียว: ถ้า Editor ต่อสายไม่ครบ (playerControl.skillCaster ว่าง ฯลฯ) state machine จะไม่เคย
        // Enter()/Do() ให้ IsComplete เป็น true เลย ถ้าไม่มี timeout ตรงนี้ isCasting จะค้าง true ถาวร
        float timeout = (skill.castAnim != null ? skill.castAnim.length : 0f) + 1f;
        float elapsed = 0f;
        while ((skillCastState == null || !skillCastState.IsComplete) && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isCasting = false;
        pendingSkill = null;
    }

    // เรียกจาก Animation Event เท่านั้น (ผ่าน AnimationRelay.CallSkillImpact)
    public void OnCastImpact()
    {
        if (pendingSkill == null) return;
        ResolveSkillEffect(pendingSkill, pendingAimDirection);
        pendingSkill = null; // กันเรียกซ้ำถ้าคลิปมี event ซ้ำโดยไม่ตั้งใจ
    }

    private void PlayFX(ActiveSkillData skill)
    {
        if (skill.castEffectPrefab != null)
        {
            Instantiate(skill.castEffectPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void PerformMeleeAttack(ActiveSkillData skill, Vector2 aimDirection)
    {
        if (attacker == null) return;

        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Vector2 center = (Vector2)transform.position + aimDirection.normalized * skill.meleeStats.meleeRange;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, skill.meleeStats.hitBoxSize, aimAngle, attacker.enemyLayers);
        DamageInside(skill, hits, aimDirection);
    }

    private void PerformAOEAttack(ActiveSkillData skill)
    {
        if (attacker == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, skill.effectRadius, attacker.enemyLayers);
        DamageInside(skill, hits, Vector2.zero);
    }

    // Melee กับ AOE ต่างกันแค่รูปทรงที่กวาด ส่วนที่เหลือใช้ร่วมกัน
    // knockbackDir = Vector2.zero แปลว่าไม่ผลัก (AOE ไม่มีทิศทาง)
    private void DamageInside(ActiveSkillData skill, Collider2D[] hits, Vector2 knockbackDir)
    {
        // ศัตรูตัวเดียวอาจมีหลาย collider — กันไม่ให้กินดาเมจซ้ำในครั้งเดียว
        HashSet<EnemyHealth> hitAlready = new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null || !hitAlready.Add(enemyHealth)) continue;

            attacker.DamageCalculation(enemyHealth, skill.damageMultiplier, attacker.GetEquippedWeapon(), skill.elementType, skill.baseDamage);
            ApplyKnockback(skill, hit, knockbackDir);
            SpawnHitEffect(skill, hit.transform.position);
        }
    }

    private void ApplyKnockback(ActiveSkillData skill, Collider2D target, Vector2 knockbackDir)
    {
        if (skill.meleeStats.knockbackForce <= 0f || knockbackDir == Vector2.zero) return;

        Rigidbody2D body = target.GetComponentInParent<Rigidbody2D>();
        if (body == null) return;

        body.AddForce(knockbackDir.normalized * skill.meleeStats.knockbackForce, ForceMode2D.Impulse);
    }

    // บัฟใช้ระบบสถานะตัวเดิมทั้งหมด — ระยะเวลา สแต็ก ไอคอน VFX มาจาก StatusEffectSO ไม่ทำซ้ำที่นี่
    private void PerformBuff(ActiveSkillData skill)
    {
        if (skill.buffEffect == null) return;

        IStatusEffectReceiver receiver = GetComponent<IStatusEffectReceiver>();
        receiver?.ApplyEffect(skill.buffEffect);
    }

    private void PerformRangedAttack(ActiveSkillData skill, Vector2 aimDirection)
    {
        if (skill.rangedStats.projectilePrefab == null) return;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // ต้องผ่าน pool เท่านั้น เพราะ Projectile.Despawn() แค่ SetActive(false)
        // ถ้า Instantiate ตรง ๆ กระสุนจะค้างใน Scene ตลอดเกมโดยไม่มีใครเก็บ
        GameObject bulletObj = ObjectPooling.Instance.SpawnFromPool(
            skill.rangedStats.projectilePrefab, transform.position, rotation);
        if (bulletObj == null) return;

        Projectile projectile = bulletObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Setup(skill.rangedStats.projectileSpeed, skill.rangedStats.projectileLifeTime, skill.rangedStats.targetLayers, (hitCollider) =>
            {
                OnProjectileHit(skill, hitCollider);
            });
        }
    }

    private void OnProjectileHit(ActiveSkillData skill, Collider2D hitCollider)
    {
        // เดิมใช้ GetComponent ทำให้พลาดเมื่อ collider เป็นลูกของตัวศัตรู — ใช้ InParent เหมือนฝั่ง Melee
        EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null && attacker != null)
        {
            attacker.DamageCalculation(enemyHealth, skill.damageMultiplier, attacker.GetEquippedWeapon(), skill.elementType, skill.baseDamage);
        }

        SpawnHitEffect(skill, hitCollider.transform.position);
    }

    private void SpawnHitEffect(ActiveSkillData skill, Vector3 position)
    {
        if (skill.hitEffectPrefab == null) return;

        GameObject fx = Instantiate(skill.hitEffectPrefab, position, Quaternion.identity);
        Destroy(fx, 0.5f); // ทำลาย Effect หลังเล่นจบ (ปรับเวลาได้ตามต้องการ)
    }
}
