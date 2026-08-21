using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSkillData", menuName = "SkillData/Skill/Passive")]
public class PassiveSkillData : SkillEffect
{
    [Header("Passive Skill")]
    public StatusType affectedStat;
    public float statModifierPercent;
}
