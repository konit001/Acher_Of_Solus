using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : CharacterHealth
{
    public enemyStatus enemy;
    public GameObject HpBarPrefab;

    [Header("Loot Drop")]
    public List<lootItem> lootTable = new List<lootItem>();

    protected override void Start()
    {
        maxHealth = enemy.maxHealth;
        base.Start();
    }

    protected override void Update()
    {
        showHealthBar();
        base.Update();
    }


    void showHealthBar()
    {
        if (currentHp < enemy.maxHealth)
        {
            HpBarPrefab.SetActive(true);
        }
    }

    public void TakeDamage(float damage, Color popupColor, bool isCrit)
    {
        base.TakeDamage(damage);
        DamagePopUp.Create((int)damage, transform.position + Vector3.up, popupColor, isCrit);
    }
    public void TakeDamage(float damage, Color popupColor)
    {
        TakeDamage(damage, popupColor, false);
    }

    public override void TakeDamage(float damage)
    {
        TakeDamage(damage, Color.white, false);
    }

    protected override void Die()
    {
        base.Die();
        GiveExp(enemy.ExpReward);
        CursorManager.instance.SetMode(MouseChange.Normal);
        foreach (lootItem item in lootTable)
        {
            if (Random.Range(0f, 100f) <= item.dropChance)
            {
                for (int i = 0; i < item.dropAmount; i++)
                {
                    Instantiate(item.dropPrefab, transform.position, Quaternion.identity);
                }
            }
        }
    }

    public void GiveExp(int amount)
    {
        LevelSystem.instance.GetExp(amount);
    }
}
