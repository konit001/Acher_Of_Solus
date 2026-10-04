using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ActiveSkillData", menuName = "SkillData/Skill/Active")]
public class ActiveSkillData : SkillEffect
{
    [Header("Active Skill Basics")]
    public ActiveSkillType skillType;
    [Tooltip("ธาตุของสกิล ใช้แทนธาตุของอาวุธ ทั้งตอนคิด resist สี popup และการติดสถานะ")]
    public ElementType elementType;
    public float cooldown;
    [Tooltip("ตัวคูณดาเมจสุดท้าย ตำแหน่งเดียวกับ 0.25 ของหอกและ 0.1 ของปืน")]
    public float damageMultiplier = 1f;
    [Tooltip("ดาเมจของตัวสกิลเอง บวกเพิ่มเข้ากับ baseAttack ของผู้เล่น + ดาเมจอาวุธ")]
    public float baseDamage = 1f;

    [System.Serializable]
    public struct MeleeData
    {
        public Vector2 hitBoxSize;
        public float meleeRange;
        public float knockbackForce;
    }

    [ShowIf("skillType", ActiveSkillType.Melee)]
    public MeleeData meleeStats;

    [System.Serializable]
    public struct RangedData
    {
        public GameObject projectilePrefab;
        public float projectileSpeed;
        public float projectileLifeTime;
        public int projectileCount;
        public LayerMask targetLayers;
    }
    [ShowIf("skillType", ActiveSkillType.Ranged)]
    public RangedData rangedStats;


    [Header("AOE Specifics")]
    [ShowIf("skillType", ActiveSkillType.AOE)] public float effectRadius;

    [Header("Buff Specifics")]
    [Tooltip("สถานะที่จะติดให้ตัวเอง ใช้ StatusEffectSO ตัวเดียวกับระบบสถานะ")]
    [ShowIf("skillType", ActiveSkillType.Buff)] public StatusEffectSO buffEffect;

    [Header("FX")]
    public AnimationClip castAnim;
    public GameObject castEffectPrefab;
    public GameObject hitEffectPrefab;
    public AudioClip castSfx;
}
