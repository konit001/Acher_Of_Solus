using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : CharacterHealth
{
    public playerStatus playerStats;
    public GameObject playerHealthBar;
    
    protected override void Start()
    {
        maxHealth = playerStats.maxHealth;
        currentHp = maxHealth;
        base.Start();
                 
    }

    public override void takeDamage(float damage)
    {
        base.takeDamage(damage);
    }

    // เมื่อผู้เล่นเลือดหมด ฟังก์ชัน Die จาก CharacterHealth จะทำงาน
    protected override void Die()
    {
        base.Die();
        
        // เรียกให้หน้าต่าง Game Over เด้งขึ้นมา
        if (GameOverController.instance != null)
        {
            GameOverController.instance.ShowGameOver();
        }
    }
}