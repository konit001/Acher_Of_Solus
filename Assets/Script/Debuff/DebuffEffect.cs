using UnityEngine;

[CreateAssetMenu(fileName = "DebuffEffect", menuName = "BuffEffect/DebuffEffect")]
public class DebuffEffect : ScriptableObject
{
    [Header("Debuff Information")]
    public string debuffName;
    public string description;
    public Sprite icon;
    public ElementType elementType;
    public DebuffType debuffType;

    [Header("Debuff Effects")]
    public float debuffDamage;
    public float debuffDamagePerTick;
    public float debuffDuration;
    public int debuffStack;
    [Header("Stat Modifier")]
    public StatusType affectedStat;
    public float statModifierPercent;

}
