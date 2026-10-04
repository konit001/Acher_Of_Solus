using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSkillData", menuName = "SkillData/Skill/Passive")]
public class PassiveSkillData : SkillEffect
{
    [Header("Passive Skill")]
    public List<StatusChange> statModifiers;

    public float GetBonusPercent(StatusType stat)
    {
        if (statModifiers == null) return 0f;

        float total = 0f;
        foreach (StatusChange modifier in statModifiers)
        {
            if (modifier.status == stat) total += modifier.value;
        }

        return total;
    }
}
