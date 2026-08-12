using UnityEngine;

[CreateAssetMenu(fileName = "artifactData", menuName = "ItemData/artifactData")]
public class artifactData : BaseItemData
{
    [Header("Basic Info")]
    public ArtifactType artifactType;
    public Rarity rarity;

    [Header("Effect Stats")]
    [TextArea(3, 5)] public string EffectDescription;

    public Vector2 maxHealth;
    public Vector2 baseAttack;
    public Vector2 ElementalBonus;
    public Vector2 CritRate; 
    public Vector2 CritDamage;
    public Vector2 Defend;
}
