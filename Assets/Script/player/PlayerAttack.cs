using System.Collections;
using System.Collections.Generic;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IActionGate
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public playerStatus playerStats;
    public playerControl playerController;

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
    private ElementType currentMeleeElement;

    #region Unity Methods

private void Update()
    {
        // ให้ Attack Point หมุนตามการเล็งตลอดเวลา
        RotateAttackPoint();

        if (!canAttack || isAttacking || !canShoot || isShooting || !CanAct) return;

        if (userInput.instance.attackLeftInput)
        {
            PerformAttack(slot1);
        }
        else if (userInput.instance.attackRightInput)
        {
            PerformAttack(slot2);
        }
    }

    private void RotateAttackPoint()
    {
        if (attackPoint == null || playerController == null) return;

        Vector2 aimDir = playerController.aimDirection;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        
        // กำหนดองศาแกน Z ให้ attackPoint
        attackPoint.rotation = Quaternion.Euler(0f, 0f, aimAngle);
    }

private void OnDrawGizmos()
    {
        if (attackPoint == null) return; 

        Gizmos.color = Color.red;

        // นำ Position, Rotation, และ Scale ของ attackPoint มาสร้าง Matrix 
        // เพื่อให้เส้น Gizmos เอียงตามแกนหมุนของวัตถุ
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, attackPoint.localScale);
        Gizmos.matrix = rotationMatrix;
        
        // วาดที่ตำแหน่ง 0,0 และขนาด 1 เพราะตำแหน่งและสเกลถูกจัดการใน Matrix แล้ว
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one); 
        
        // คืนค่า Matrix กลับเป็นปกติเพื่อไม่ให้กระทบ Gizmos อื่นๆ
        Gizmos.matrix = Matrix4x4.identity; 
    }

    #endregion

    #region Attack Logic

    private void PerformAttack(weaponSlot currentSlot)
    {
        if (currentSlot.IsEmpty) return;

        BaseItemData currentItem = currentSlot.GetCurrentItem();
        WeaponsType currentWeaponType = currentItem.weaponType;
        weaponsData weaponInfo = currentItem as weaponsData;

        playerController.FaceAimDirection();

        switch (currentWeaponType)
        {
            case WeaponsType.Spear:
                if (weaponInfo != null)
                {
                    currentMeleeElement = weaponInfo.elementType;
                    Debug.Log($"[PlayerAttack] ธาตุจากอาวุธ '{weaponInfo.itemName}' = {currentMeleeElement}");
                }
                ExecuteSpearAttack();
                break;

            case WeaponsType.Gun:
                ExecuteGunAttack(currentItem, weaponInfo);
                break;

            default:
                Debug.LogWarning($"ยังไม่ได้เขียนโค้ดโจมตีสำหรับอาวุธชนิดนี้: {currentWeaponType}");
                break;
        }
    }

    private void ExecuteSpearAttack()
    {
        print("Hoooit");
        StartCoroutine(AttackRoutine(playerController.SpearAttackState));
    }

private void ExecuteGunAttack(BaseItemData currentItem, weaponsData weaponInfo)
    {
        RangedWeaponData gunData = currentItem as RangedWeaponData;
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
                    EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
                    if (enemy != null)
                    {
                        DamageCalculation(enemy, 0.1f, weaponInfo.elementType);
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
        // เปลี่ยนจาก 0f เป็น attackPoint.eulerAngles.z เพื่อให้กล่องโจมตีเอียงตามทิศการเล็ง
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPoint.position, 
            attackPoint.localScale, 
            attackPoint.eulerAngles.z, 
            enemyLayers
        );
        
        HashSet<EnemyHealth> hitAlready = new HashSet<EnemyHealth>();

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth hitEnemyHealth = enemy.GetComponent<EnemyHealth>();

            if (hitEnemyHealth != null && hitAlready.Add(hitEnemyHealth))
            {
                DamageCalculation(hitEnemyHealth, 0.25f, currentMeleeElement);
            }
        }
    }

    public void DamageCalculation(EnemyHealth targetEnemy, float multiple, ElementType attackElement)
    {
        // --- Player Damage Calculation ---
        float damage = playerStats.baseAttack * (1 + playerStats.ElementalBonus / 100f);
        bool isCrit = Random.Range(0f, 100f) <= playerStats.CritRate;
        
        if (isCrit)
        {
            float cirtMult = (1 + playerStats.CritDamage / 100f) * 1.25f;
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