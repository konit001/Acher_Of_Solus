using UnityEngine;

[CreateAssetMenu(fileName = "DebuffEffect", menuName = "Scriptable Objects/DebuffEffect")]
public class DebuffEffect : ScriptableObject
{
    [Header("Debuff Information")]
    public string debuffName;
    public string description;
    public Sprite icon;

    [Header("Debuff Effects")]
    public float damageOverTime;
    public float slowEffects;
    public float defenseReduction;
}
