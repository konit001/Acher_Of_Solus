using UnityEngine;

// [CreateAssetMenu(fileName = "SkillEffect", menuName = "SkillData/SkillEffect")]
public class SkillEffect : ScriptableObject
{
    [Header("Skill Info")]
    public int skillId;
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Weapon Tree")]
    public WeaponsType ownerWeaponType;
    public int tier;
    public SkillEffect requiredPreviousSkill;

    [Header("Unlock Cost")]
    public int skillPointCost;
}
