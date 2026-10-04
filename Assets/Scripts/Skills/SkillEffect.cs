using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    [Header("Skill Info")]
    public int skillId;
    public string skillName;
    [TextArea] public string description;
    public SkillTier tier;
    public Sprite icon;

    [Header("Skill Tree")]
    public SkillEffect requiredPreviousSkill;

    [Header("Unlock Cost")]
    public int skillPointCost;
}
