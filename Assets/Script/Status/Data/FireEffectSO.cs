using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "StatusEffectSO/Element/FireEffectSO")]
public class FireEffectSO : StatusEffectSO
{
    public override void OnTick(GameObject target, ActiveStatusEffect instance)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return;

        float damagePerTick = GetStatValue(StatusType.Health); // flat damage, not a percent
        damageable.TakeDamage(damagePerTick * instance.stackCount);
    }
}
