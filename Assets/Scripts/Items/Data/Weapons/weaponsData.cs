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
    // ค่าเริ่มต้นเท่านั้น — เลเวลตอนเล่นจริงอยู่ที่ WeaponProgressManager
    // (ห้ามเขียนค่ากลับลงตรงนี้ เพราะจะเป็นการเขียนทับไฟล์ .asset)
    public int level;

    [Tooltip("ตารางวัสดุและโบนัสของการอัปเกรด")]
    public WeaponLevelData levelData;

    [Header("Skill Unlock")]
    public SkillEffect first;
    public SkillEffect second;
    public SkillEffect third;

    [Header("FX")]
    public AnimationClip attackAnim;
    public GameObject hitEffectPrefab;
    public AudioClip attackSfx;
}