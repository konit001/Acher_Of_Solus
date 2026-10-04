using UnityEngine;

public class ShootState : State
{
    public AnimationClip shootAnim; // ใส่แอนิเมชันยิง (ถ้ามี)

    [Header("Burst Fire")]
    public int shotsPerBurst = 3;
    public float cooldownDuration = 2f; // เวลารอหลังยิงครบ ก่อนยิงชุดใหม่ได้ (จังหวะระหว่างนัดใช้ enemyCombat.fireInterval เดิม)

    private Enemy enemyCore => core as Enemy; // เข้าถึงตัวแปรใน Enemy
    private int shotsFired;

    public override bool CanEnter()
        => enemyCore != null && enemyCore.combat != null
           && enemyCore.combat.DetectPlayer()
           && Time.time >= enemyCore.combat.cooldownUntil; // ยังไม่หมดคูลดาวน์ = เข้าไม่ได้

    public override void Enter()
    {
        shotsFired = 0;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (shootAnim != null && animator != null)
        {
            animator.Play(shootAnim.name);
        }
    }

    public override void Do()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (shotsFired >= shotsPerBurst)
        {
            enemyCore.combat.cooldownUntil = Time.time + cooldownDuration; // เริ่มนับคูลดาวน์
            return; // selectState() รอบหน้าจะเห็น CanEnter()=false แล้วสลับไป CooldownState แทน
        }

        if (enemyCore != null && enemyCore.combat != null && enemyCore.combat.ExecuteShoot())
        {
            shotsFired++; // นับเฉพาะเฟรมที่ ExecuteShoot() ยิงจริง (ตาม fireInterval เดิม)
        }
    }

    public override void Exit(){}


//    void Update()
//     {
//         if (DetectPlayer())
//         {
//             AttackState();
//         }
//     }

//     public override void AttackState()
//     {
//         fireTimer += Time.deltaTime;

//         if (fireTimer >= fireInterval)
//         {
//             fireTimer = 0f;
//             ShootAtPlayer();
//         }
//     }

//     void ShootAtPlayer()
//     {
//         if (enemyBulletPrefab == null || firePoint == null) return;

//         GameObject player = GameObject.FindGameObjectWithTag("Player");
//         if (player != null)
//         {
//             Vector2 direction = (player.transform.position - firePoint.position).normalized;
//             float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
//             Quaternion rot = Quaternion.Euler(0f, 0f, angle);

//             GameObject bulletObj = Instantiate(enemyBulletPrefab, firePoint.position, rot);

//             EnemyBullet bulletScript = bulletObj.GetComponent<EnemyBullet>();
//             if (bulletScript != null)
//             {
//                 // ส่งค่า this (enemyCombat ตัวนี้) เข้าไปให้กระสุนรู้ว่าจะเรียกใช้สูตรคำนวณดาเมจของใคร
//                 bulletScript.Setup(this, 10f, 3f);
//             }
//         }
//     }
}