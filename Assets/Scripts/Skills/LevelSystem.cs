using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem instance;
    public playerStatus playerStatus;
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // ยังไม่มีระบบ save/load — เริ่มนับใหม่จาก 1/0 ทุกครั้งที่กด Play เสมอ แทนการอ่านจาก
        // playerStatus ตรง ๆ เพราะ playerStatus เป็น ScriptableObject asset ซึ่ง Unity ไม่รีเซ็ต
        // ค่ากลับอัตโนมัติเมื่อกด Stop (ต่างจาก object ในฉาก) ค่าจึงค้างข้ามรอบ Play ถ้าไม่เขียนทับเอง
        currentLevel = 1;
        currentExp = 0;
        updateExpToNextLevel();
        playerStatus.level = currentLevel;
        playerStatus.exp = currentExp;
    }
    public void GetExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
            updateExpToNextLevel();
        }

        playerStatus.exp = currentExp;
        Debug.Log("Gained " + amount + " EXP. Current EXP: " + currentExp + "/" + expToNextLevel);
    }

    public void LevelUp()
    {
        currentLevel++;
        playerStatus.level = currentLevel;
        SkillTreeManager.Instance.AddSkillPoint(1);
        Debug.Log("Level Up! Current Level: " + currentLevel + ", Skill Points: " + SkillTreeManager.Instance.skillPoints);
    }

    public void updateExpToNextLevel()
    {
        float exponent = currentLevel - 1;
        float calculatedExp = 100f * Mathf.Pow(1.5f, exponent);

        expToNextLevel = Mathf.RoundToInt(calculatedExp);
        Debug.Log("EXP needed for Level " + (currentLevel + 1) + " is: " + expToNextLevel);
    }

}
