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
        currentLevel = playerStatus.level;
        currentExp = playerStatus.exp;
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

        // playerStatus.exp = currentExp;
        Debug.Log("Gained " + amount + " EXP. Current EXP: " + currentExp + "/" + expToNextLevel);
    }

    public void LevelUp()
    {
        currentLevel++;
        // playerStatus.level = currentLevel;
        SkillTreeManager.Instance.skillPoints++;
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
