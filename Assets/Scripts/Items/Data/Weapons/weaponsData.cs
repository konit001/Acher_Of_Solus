using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "weaponsData", menuName = "ItemData/weaponsData")]
public class weaponsData : BaseItemData
{
    [Header("Weapon Info")]
    public WeaponsType weaponType;
    public int weaponId;
    public ElementType elementType;
    public Rarity rarity;

    [Header("Weapon Status")]
    public float ElementalBonus;
    [Range(0f, 100f)] public float CritRate;
    public float CritDamage;
    [Tooltip("โอกาสทำให้เป้าหมายติดสถานะตามธาตุ (%)")]
    [Range(0f, 100f)] public float Luck;
    public int level;

    [Tooltip("ตารางวัสดุและโบนัสของการอัปเกรด")]
    public WeaponLevelData levelData;

    // ต้องอัพเกรดอาวุธถึงเลเวลนี้ก่อนสกิลตัวนั้นถึงจะ "ปลดล็อกได้" — ปลดล็อกจริงยังต้องใช้ skill point ผ่าน SkillTreeManager
    [Header("Skill Unlock")]
    public ActiveSkillData first;
    public int firstUnlockLevel = 1;
    public ActiveSkillData second;
    public int secondUnlockLevel = 1;
    public ActiveSkillData third;
    public int thirdUnlockLevel = 1;

    public struct WeaponSkillUnlock
    {
        public ActiveSkillData skill;
        public int requiredLevel;
    }

    public IEnumerable<WeaponSkillUnlock> UnlockableSkills
    {
        get
        {
            if (first != null) yield return new WeaponSkillUnlock { skill = first, requiredLevel = firstUnlockLevel };
            if (second != null) yield return new WeaponSkillUnlock { skill = second, requiredLevel = secondUnlockLevel };
            if (third != null) yield return new WeaponSkillUnlock { skill = third, requiredLevel = thirdUnlockLevel };
        }
    }

    [Header("FX")]
    public AnimationClip attackAnim;
    public GameObject hitEffectPrefab;
    public AudioClip attackSfx;
}