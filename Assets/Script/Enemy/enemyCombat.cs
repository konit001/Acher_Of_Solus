using UnityEngine;

public class enemyCombat : MonoBehaviour
{
    public enemyStatus enemy;
    public GameObject enemyHpbar;
    public bool isAttacking;
    public bool isAttacked;

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

    [Header("Debuff")]
    [Range(0f, 100f)] public float debuffChance = 20f;
    public DebuffDatabase debuffDatabase;

    private Core core;

    private void Awake()
    {
        core = GetComponent<Core>();
    }

    public bool DetectPlayer()
    {
        return detectPlayer = Physics2D.OverlapCircle(detectCheck.transform.position, detectRadius, playerLayer);
    }

    public void ExecuteShoot()
    {
        if (core != null && core.IsActionDisabled) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            ShootAtPlayer();
        }
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
                        PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
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
        targetEnemy.takeDamage(totalDamage);

        // สุ่มโอกาสติดดีบัฟตามธาตุของศัตรูตัวนี้ทุกครั้งที่ตีโดนผู้เล่น
        if (debuffDatabase == null)
        {
            Debug.LogWarning($"[EnemyDebuff] {gameObject.name}: ไม่ได้ผูก debuffDatabase ไว้ ข้ามการสุ่มดีบัฟ");
        }
        else
        {
            float roll = Random.Range(0f, 100f);
            bool success = roll <= debuffChance;
            Debug.Log($"[EnemyDebuff] {gameObject.name}: สุ่มโอกาสติดดีบัฟ roll={roll:F1} <= chance={debuffChance} -> {(success ? "ติด" : "ไม่ติด")}");

            if (success)
            {
                DebuffEffect effect = debuffDatabase.GetRandomDebuff(enemy.elementType);
                if (effect != null && targetEnemy.debuffController != null)
                {
                    targetEnemy.debuffController.ApplyDebuff(effect);
                }
                else if (targetEnemy.debuffController == null)
                {
                    Debug.LogWarning("[EnemyDebuff] ผู้เล่นไม่มี DebuffController ติดตั้งอยู่");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectCheck.transform.position, detectRadius);
    }
}