using UnityEngine;

[CreateAssetMenu(fileName = "enemyStatus", menuName = "Status/enemyStatus")]
public class enemyStatus : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    [TextArea(2, 4)]public string description;
    public enemyType enemyType;
    public ElementType elementType;
    public int ExpReward;

    [Space(10)]
    [Header("Base Stats")]
    public float maxHealth;
    public float basespeed;
    public float stamina;

    [Space(10)]
    [Header("Offensive Stats")]
    public float baseAttack;
    public float ElementalBonus;
    [Range(0f, 100f)] public float CritRate;
    public float CritDamage;

    [Space(10)]
    [Header("Defensive Stats")]
    public float Defend;
    [Range(0f, 100f)] public float ResistanceDamage;
    [Range(0f, 100f)] public float ElementalResistance;
    [Range(0f, 100f)] public float CritDamageResistance;

}
