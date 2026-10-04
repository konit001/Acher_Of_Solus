using System.Collections;
using System.Collections.Generic;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IActionGate
{
    [Header("References")]
    public Transform attackPoint;          // จุดกำเนิดกระสุนปืน
    public BoxCollider2D attackHitbox;     // กล่องโจมตีระยะประชิด ขนาด/ออฟเซ็ตคีย์ได้ในอนิเมชัน
    public LayerMask enemyLayers;
    public playerStatus playerStats;
    public playerControl playerController;
    public SkillCaster skillCaster; // กันโจมตีปกติเริ่มซ้อนตอนกำลังร่ายสกิลค้างอยู่

    [Header("UI Weapon Slots")]
    public weaponSlot slot1; // ลาก Weapon Slot 1 จาก UI มาใส่ใน Inspector
    public weaponSlot slot2; // ลาก Weapon Slot 2 จาก UI มาใส่ใน Inspector

    [Header("Runtime Status")]
    public bool canAttack { get; set; } = true;
    public bool isAttacking { get; private set; }
    public bool canShoot { get; set; } = true;
    public bool isShooting { get; private set; }
    public bool CanAct { get; set; } = true; // external gate (status effects), separate from the animation locks above

    // Internal Variables
    private weaponsData currentAttackWeapon;

    // สร้างครั้งเดียวแล้วใช้ซ้ำทุกครั้งที่ฟัน เพื่อไม่ให้เกิดขยะหน่วยความจำกลางคอมแบต
    private ContactFilter2D enemyFilter;
    private readonly List<Collider2D> overlapResults = new List<Collider2D>();
    private readonly HashSet<EnemyHealth> hitAlready = new HashSet<EnemyHealth>();

    #region Unity Methods

    private void Awake()
    {
        // ถ้าลืมลากใส่ ให้หาจาก attackPoint ให้เอง (collider อยู่บน GameObject เดียวกัน)
        if (attackHitbox == null && attackPoint != null)
        {
            attackHitbox = attackPoint.GetComponent<BoxCollider2D>();
        }

        enemyFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = enemyLayers,
            useTriggers = true   // ให้เหมือนพฤติกรรมเดิมของ OverlapBoxAll (queriesHitTriggers = true)
        };
    }

    private void Update()
    {
        // ไม่ต้องหมุน attackPoint เอง: กล่องโจมตีเป็นลูกของ visual อยู่แล้ว
        // จึงหมุนตามมุมเล็งและพลิกซ้ายขวาตามตัวละครโดยอัตโนมัติ

        // เดิมไม่มีความรู้เรื่องสกิลเลย ผู้เล่นเลยแทงหอก/ยิงปืนซ้อนตอนกำลังร่ายสกิลได้ — เติมด่านนี้กันไว้
        bool isCastingSkill = skillCaster != null && skillCaster.isCasting;
        if (!canAttack || isAttacking || !canShoot || isShooting || !CanAct || isCastingSkill) return;

        if (userInput.instance.attackLeftInput)
        {
            PerformAttack(slot1);
        }
        else if (userInput.instance.attackRightInput)
        {
            PerformAttack(slot2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackHitbox == null) return;

        Gizmos.color = Color.red;

        // localToWorldMatrix รวมการพลิกซ้ายขวาไว้ด้วย เส้นที่เห็นจึงตรงกับกล่องที่ Overlap() ใช้จริง
        Gizmos.matrix = attackHitbox.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(attackHitbox.offset, attackHitbox.size);

        // คืนค่า Matrix กลับเป็นปกติเพื่อไม่ให้กระทบ Gizmos อื่นๆ
        Gizmos.matrix = Matrix4x4.identity;
    }

    #endregion

    #region Attack Logic

    private void PerformAttack(weaponSlot currentSlot)
    {
        if (currentSlot.IsEmpty) return;

        weaponsData weaponInfo = currentSlot.GetCurrentItem() as weaponsData;
        if (weaponInfo == null)
        {
            Debug.LogWarning("ไอเทมในช่องนี้ไม่ใช่อาวุธ จึงโจมตีไม่ได้");
            return;
        }

        currentAttackWeapon = weaponInfo;   // TriggerSpearDamage ถูกเรียกจาก Animation Event จึงต้องจำอาวุธของไม้นี้ไว้

        playerController.FaceAimDirection();

        switch (weaponInfo.weaponType)
        {
            case WeaponsType.Spear:
                ExecuteSpearAttack();
                break;

            case WeaponsType.Gun:
                ExecuteGunAttack(weaponInfo);
                break;

            default:
                Debug.LogWarning($"ยังไม่ได้เขียนโค้ดโจมตีสำหรับอาวุธชนิดนี้: {weaponInfo.weaponType}");
                break;
        }
    }

    private void ExecuteSpearAttack()
    {
        print("Hoooit");
        StartCoroutine(AttackRoutine(playerController.SpearAttackState));
    }

private void ExecuteGunAttack(weaponsData weaponInfo)
    {
        RangedWeaponData gunData = weaponInfo as RangedWeaponData;
        if (gunData == null || gunData.bulletPrefab == null) return; 

        Vector2 aimDir = playerController.aimDirection;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0f, 0f, aimAngle);

        // ✅ โยน gunData.bulletPrefab เข้าไปตรงๆ เลย! ระบบจะหากระสุนของปืนกระบอกนี้ให้อัตโนมัติ
        GameObject bulletObj = ObjectPooling.Instance.SpawnFromPool(gunData.bulletPrefab, attackPoint.position, bulletRotation);
        
        if (bulletObj != null)
        {
            Projectile bulletScript = bulletObj.GetComponent<Projectile>();
            if (bulletScript != null)
            {
                bulletScript.Setup(gunData.bulletSpeed, gunData.bulletlifetime, enemyLayers, (hitCollider) =>
                {
                    EnemyHealth enemy = hitCollider.GetComponentInParent<EnemyHealth>();
                    if (enemy != null)
                    {
                        DamageCalculation(enemy, 0.1f, weaponInfo);
                    }
                });
            }
        }
        
        StartCoroutine(GunCooldownRoutine(gunData.fireRate));
    }

    #endregion

    #region Damage Calculation

    public void TriggerSpearDamage()
    {
        if (attackHitbox == null) return;

        // อนิเมชันขยับ Transform ในรอบ Update ส่วนรูปทรงฟิสิกส์จะอัปเดตรอบ FixedUpdate
        // จึงต้อง sync ก่อน ไม่งั้น Overlap() จะอ่านตำแหน่งของเฟรมก่อนหน้า
        Physics2D.SyncTransforms();

        hitAlready.Clear();
        overlapResults.Clear();

        // ให้ Unity คิดขนาดจริง, offset, มุม และการพลิกซ้ายขวาให้ครบจาก matrix ของ collider
        int hitCount = attackHitbox.Overlap(enemyFilter, overlapResults);

        for (int i = 0; i < hitCount; i++)
        {
            EnemyHealth hitEnemyHealth = overlapResults[i].GetComponentInParent<EnemyHealth>();

            if (hitEnemyHealth != null && hitAlready.Add(hitEnemyHealth))
            {
                DamageCalculation(hitEnemyHealth, 0.25f, currentAttackWeapon);
            }
        }
    }

    // overrideElement = ธาตุของสกิล (None = ใช้ธาตุของอาวุธตามเดิม)
    // bonusDamage     = ดาเมจของตัวสกิลเอง บวกเข้ากับ baseAttack ของผู้เล่น
    public void DamageCalculation(EnemyHealth targetEnemy, float multiple, weaponsData attackWeapon,
                                  ElementType overrideElement = ElementType.None, float bonusDamage = 0f)
    {
        ElementType attackElement = overrideElement != ElementType.None
            ? overrideElement
            : (attackWeapon != null ? attackWeapon.elementType : ElementType.None);

        // --- Player + Weapon Damage Calculation ---
        // ค่าสถานะของอาวุธจะแรงขึ้นตามเลเวลของอาวุธเล่มนั้น
        // อัพเกรดอาวุธก็บวกพลังโจมตีของตัวละครหลัก (MC) เพิ่มไปด้วย ไม่ใช่แค่ค่าโจมตีของอาวุธ
        float weaponMultiplier = GetWeaponMultiplier(attackWeapon);

        float totalAttack = playerStats.baseAttack * weaponMultiplier + GetWeaponBaseAttack(attackWeapon) * weaponMultiplier + bonusDamage;
        float totalElementalBonus = playerStats.ElementalBonus;
        float totalCritRate = playerStats.CritRate;
        float totalCritDamage = playerStats.CritDamage;

        // โบนัสจาก Passive Skill ที่ปลดล็อกแล้ว (ถ้ามี SkillTreeManager อยู่ในฉาก)
        if (SkillTreeManager.Instance != null)
        {
            totalElementalBonus += SkillTreeManager.Instance.GetTotalPassiveBonus(StatusType.ElementalBonus);
            totalCritRate += SkillTreeManager.Instance.GetTotalPassiveBonus(StatusType.CritRate);
            totalCritDamage += SkillTreeManager.Instance.GetTotalPassiveBonus(StatusType.CritDamage);
        }

        if (attackWeapon != null)
        {
            totalElementalBonus += attackWeapon.ElementalBonus * weaponMultiplier;
            totalCritRate += attackWeapon.CritRate * weaponMultiplier;
            totalCritDamage += attackWeapon.CritDamage * weaponMultiplier;
        }

        float damage = totalAttack * (1 + totalElementalBonus / 100f);
        bool isCrit = Random.Range(0f, 100f) <= Mathf.Clamp(totalCritRate, 0f, 100f);
        
        if (isCrit)
        {
            float cirtMult = (1 + totalCritDamage / 100f) * 1.25f;
            damage *= cirtMult;
        }

        // --- Enemy Defense Calculation ---
        float enemyDefend = targetEnemy.enemy.Defend * (targetEnemy.enemy.ResistanceDamage / 100f);
        float elementalMult = 1f - targetEnemy.enemy.ElementalResistance / 100f;
        float critResist = isCrit ? (1f - targetEnemy.enemy.CritDamageResistance / 100f) : 1f;

        // --- Total Damage ---
        float totalDamage = (damage * critResist - enemyDefend) * elementalMult;
        totalDamage = Mathf.Max(totalDamage, 0f);

        Debug.Log($"Total Damage dealt: {totalDamage}");

        // สี Damage Pop-up อิงตามธาตุของอาวุธที่โจมตี ผ่าน ElementalManager
        Color popupColor = ElementalManager.GetElementColor(attackElement);
        targetEnemy.TakeDamage(totalDamage * multiple, popupColor, !isCrit);

        // ติดสถานะได้เฉพาะเป้าหมายที่ยังไม่ตายจากหมัดนี้ — โชคของผู้เล่นบวกโชคของอาวุธ
        if (targetEnemy.IsAlive)
        {
            float luck = Mathf.Clamp(playerStats.Luck + (attackWeapon != null ? attackWeapon.Luck : 0f), 0f, 100f);
            Debug.Log($"[OnHit] PlayerAttack → manager {(StatusEffectManager.Instance != null ? "พร้อม" : "= null!")} | อาวุธ = {(attackWeapon != null ? attackWeapon.itemName : "null")} | playerLuck {playerStats.Luck} + weaponLuck {(attackWeapon != null ? attackWeapon.Luck : 0f)} = {luck}");
            StatusEffectManager.Instance?.TryApplyOnHit(targetEnemy.gameObject, attackElement, luck);
        }
        else
        {
            Debug.Log("[OnHit] PlayerAttack → ศัตรูตายจากหมัดนี้แล้ว ข้ามการติดสถานะ");
        }
    }

    // ค่าโจมตีพื้นฐานของอาวุธ (ยังไม่คิดเลเวล)
    private float GetWeaponBaseAttack(weaponsData weapon)
    {
        MeleeWeaponData melee = weapon as MeleeWeaponData;
        if (melee != null) return melee.MeleeAttack;

        RangedWeaponData ranged = weapon as RangedWeaponData;
        if (ranged != null) return ranged.rangedAttack;

        return 0f;
    }

    // ตัวคูณค่าสถานะตามเลเวลของอาวุธ — ไม่มีอาวุธหรือยังไม่มีตัวจัดการก็คือ 1 เท่า
    private float GetWeaponMultiplier(weaponsData weapon)
    {
        if (weapon == null || WeaponProgressManager.instance == null) return 1f;

        return WeaponProgressManager.instance.GetStatMultiplier(weapon);
    }

    // สกิลไม่ได้มาจากการกดโจมตี จึงไม่มี currentAttackWeapon ของตัวเอง
    // ยืมอาวุธที่สวมอยู่มาคิดดาเมจฐาน/คริต — ใช้ร่วมกับ DamageCalculation ตรง ๆ จาก SkillCaster
    public weaponsData GetEquippedWeapon()
    {
        if (slot1 != null && !slot1.IsEmpty) return slot1.GetCurrentItem() as weaponsData;
        if (slot2 != null && !slot2.IsEmpty) return slot2.GetCurrentItem() as weaponsData;
        return null;
    }

    #endregion

    #region Coroutines

    private IEnumerator AttackRoutine(State attackState)
    {
        canAttack = false;
        isAttacking = true;
        
        yield return null;
        yield return new WaitUntil(() => attackState.IsComplete);
        
        isAttacking = false;
        canAttack = true;
    }

    private IEnumerator GunCooldownRoutine(float fireRate)
    {
        canShoot = false;
        isShooting = true;

        float cooldownTime = 1f / fireRate;
        yield return new WaitForSeconds(cooldownTime);

        isShooting = false;
        canShoot = true;
    }

    #endregion
}