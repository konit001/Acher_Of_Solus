using UnityEngine;

// [CreateAssetMenu(fileName = "weaponsData", menuName = "ItemData/weaponsData")]
public class weaponsData : BaseItemData
{
    [Header("Weapon Info")]
    public int weaponId;
    public ElementType elementType;
    public Rarity rarity;

    [Header("Weapon Status")]
    public float ElementalBonus;
    [Range(0f, 100f)] public float CritRate;
    public float CritDamage;

    [Header("FX")]
    public AnimationClip attackAnim;
    public GameObject hitEffectPrefab;
    public AudioClip attackSfx;
}