using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkillData", menuName = "SkillData/Skill/Active")]
public class ActiveSkillData : SkillEffect
{
    [Header("Active Skill")]
    public float cooldown;

    [Header("FX")]
    public AnimationClip castAnim;
    public GameObject castEffectPrefab;
    public AudioClip castSfx;

    public virtual void Cast()
    {
    }
}
