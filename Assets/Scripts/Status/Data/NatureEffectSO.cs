using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "StatusEffectSO/Element/NatureEffectSO")]
public class NatureEffectSO : StatusEffectSO
{
    // the configured Health value is a percent-of-max-HP budget for the whole effect;
    // this scales it down to the slice dealt by a single tick
    private const float PercentToTickRatio = 0.025f;

    public override void OnTick(GameObject target, ActiveStatusEffect instance)
    {
        CharacterHealth health = target.GetComponent<CharacterHealth>();
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (health == null || damageable == null) return;

        float percentOfMaxHp = GetStatValue(StatusType.Health); // e.g. 0.05 = 5% of max HP
        float damagePerTick = health.MaxHealth * (percentOfMaxHp * PercentToTickRatio);
        damageable.TakeDamage(damagePerTick * instance.stackCount);
    }
}
