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

    public bool DetectPlayer()
    {
        return detectPlayer = Physics2D.OverlapCircle(detectCheck.transform.position, detectRadius, playerLayer);
    }

    public void ExecuteShoot()
    {
        if (!canAct) return;

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
        targetEnemy.TakeDamage(totalDamage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectCheck.transform.position, detectRadius);
    }
}
