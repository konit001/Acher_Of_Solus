using UnityEngine;

public class enemyCombat : MonoBehaviour, IActionGate
{
    public enemyStatus enemy;
    public GameObject enemyHpbar;
    public bool isAttacking;
    public bool isAttacked;
    public bool canAct = true;

    bool IActionGate.CanAct
    {
        get => canAct;
        set => canAct = value;
    }

    [Header("detect")]
    public GameObject detectCheck;
    public bool detectPlayer;
    public float detectRadius;
    public LayerMask playerLayer;

    [Header("Combat & Shooting")]
    public Transform firePoint;
    public GameObject enemyBulletPrefab;
    public float fireInterval = 1.5f;
    protected float fireTimer;

    [Header("Burst Cooldown")]
    public float cooldownUntil; // Time.time ที่จะยิงชุดใหม่ได้ (0 = ยิงได้ทันที)

    public bool DetectPlayer()
    {
        return detectPlayer = Physics2D.OverlapCircle(detectCheck.transform.position, detectRadius, playerLayer);
    }

    // คืนค่า true เฉพาะเฟรมที่ยิงจริง (fireTimer สะสมครบ fireInterval) เพื่อให้ผู้เรียก (เช่น ShootState) นับจำนวนนัดที่ยิงได้จริง
    public bool ExecuteShoot()
    {
        if (!canAct) return false;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            ShootAtPlayer();
            return true;
        }

        return false;
    }

    void ShootAtPlayer()
    {
        if (enemyBulletPrefab == null || firePoint == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            // ✅ โยน enemyBulletPrefab เข้าไปตรงๆ!
            GameObject bulletObj = ObjectPooling.Instance.SpawnFromPool(enemyBulletPrefab, firePoint.position, rot);

            if (bulletObj != null)
            {
                Projectile bulletScript = bulletObj.GetComponent<Projectile>();
                if (bulletScript != null)
                {
                    bulletScript.Setup(10f, 3f, playerLayer, (hitCollider) =>
                    {
                        PlayerHealth playerHealth = hitCollider.GetComponentInParent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            DamageCalculation(playerHealth);
                        }
                    });
                }
            }
        }
    }
    public void DamageCalculation(PlayerHealth targetEnemy)
    {

        //enemy
        float damage = enemy.baseAttack * (1 + enemy.ElementalBonus / 100f);
        bool isCrit = Random.Range(0f, 100f) <= enemy.CritRate;
        if (isCrit)
        {
            float cirtMult = (1 + enemy.CritDamage / 100f) * 1.25f;
            damage *= cirtMult;
        }

        //player
        float enemyDefend = targetEnemy.playerStats.Defend * (targetEnemy.playerStats.ResistanceDamage / 100f);

        float totalDamage = damage - enemyDefend;
        totalDamage = Mathf.Max(totalDamage, 1f);

        Debug.Log(totalDamage);
        targetEnemy.TakeDamage(totalDamage);

        // ติดสถานะได้เฉพาะเป้าหมายที่ยังไม่ตายจากหมัดนี้ — ฝั่งศัตรูใช้โชคของตัวเองตัวเดียว ไม่มีอาวุธมาบวก
        if (targetEnemy.IsAlive)
        {
            Debug.Log($"[OnHit] enemyCombat → manager {(StatusEffectManager.Instance != null ? "พร้อม" : "= null!")} | ธาตุ = {enemy.elementType} | enemyLuck = {enemy.Luck}");
            StatusEffectManager.Instance?.TryApplyOnHit(targetEnemy.gameObject, enemy.elementType, enemy.Luck);
        }
        else
        {
            Debug.Log("[OnHit] enemyCombat → ผู้เล่นตายจากนัดนี้แล้ว ข้ามการติดสถานะ");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectCheck.transform.position, detectRadius);
    }
}
