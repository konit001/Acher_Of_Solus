using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public enum SubStat
{ 
    MaxHealth, BaseAttack, ElementalBonus, CritRate, CritDamage, Defend
}

[System.Serializable]
public class ArtifactItem
{
    public artifactData baseData;
    [Header("Rolled Stats")]
    public float maxHealth = 0;
    public float baseAttack = 0;
    public float elementalBonus = 0;
    public float critRate = 0;
    public float critDamage = 0;
    public float defend = 0;

    public ArtifactItem(artifactData data)
    {
        baseData = data;
        RollSubStat();
    }

    private void RollSubStat()
    {
        // Create a list of all possible sub-stats
        List<SubStat> selectedSubStats = System.Enum.GetValues(typeof(SubStat)).Cast<SubStat>().ToList();

        // Randomly select 3 sub-stats
        List<SubStat> rolledSubStats = new List<SubStat>();
        for (int i = 0 ; i <  3 ; i++)
        {
            int index = Random.Range(0, selectedSubStats.Count);
            rolledSubStats.Add(selectedSubStats[index]);
            selectedSubStats.RemoveAt(index);
        }

        // Roll values for the selected sub-stats
        foreach (SubStat Stat in rolledSubStats)
        {
            switch (Stat)
            {
                case SubStat.MaxHealth:
                    maxHealth = Random.Range(baseData.maxHealth.x, baseData.maxHealth.y);
                    break;
                case SubStat.BaseAttack:
                    baseAttack = Random.Range(baseData.baseAttack.x, baseData.baseAttack.y);
                    break;
                case SubStat.ElementalBonus:
                    elementalBonus = Random.Range(baseData.ElementalBonus.x, baseData.ElementalBonus.y);
                    break;
                case SubStat.CritRate:
                    critRate = Random.Range(baseData.CritRate.x, baseData.CritRate.y);
                    break;
                case SubStat.CritDamage:
                    critDamage = Random.Range(baseData.CritDamage.x, baseData.CritDamage.y);
                    break;
                case SubStat.Defend:
                    defend = Random.Range(baseData.Defend.x, baseData.Defend.y);
                    break;
            }
        }
    }
}
