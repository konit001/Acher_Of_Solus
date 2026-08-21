using System.Collections.Generic;
using UnityEngine;

public class ActiveStatusEffect
{
    public StatusEffectSO EffectSO;
    public float Duration;
    public float tickTimer;
    public int stackCount = 1;
    public GameObject spawnedVfx;
}

public class StatusEffectController : MonoBehaviour
{
    public List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

    private CharacterHealth characterHealth;

    void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();
    }

    public void ApplyEffect(StatusEffectSO effectSO)
    {
        // already have this effect? stack it instead of adding a duplicate
        ActiveStatusEffect existing = activeEffects.Find(e => e.EffectSO == effectSO);

        if (existing != null)
        {
            existing.stackCount = Mathf.Min(existing.stackCount + 1, effectSO.maxStack);
            if (effectSO.refreshDurationOnStack)
                existing.Duration = effectSO.duration;
            return;
        }

        ActiveStatusEffect newEffect = new ActiveStatusEffect
        {
            EffectSO = effectSO,
            Duration = effectSO.duration,
            stackCount = 1
        };

        if (effectSO.vfxPrefab != null)
            newEffect.spawnedVfx = Instantiate(effectSO.vfxPrefab, transform);

        // tint the HP bar while an elemental debuff is active
        if (effectSO.elementDebuff != elementDebuffType.None)
            characterHealth?.ChangeHpBar(effectSO.elementDebuff, true);

        activeEffects.Add(newEffect);
    }

    void Update()
    {
        // backwards loop so removing an expired effect mid-loop is safe
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveStatusEffect effect = activeEffects[i];
            effect.Duration -= Time.deltaTime;
            effect.tickTimer += Time.deltaTime;

            if (effect.tickTimer >= effect.EffectSO.tickInterval)
            {
                effect.tickTimer = 0f;
                effect.EffectSO.OnTick(gameObject, effect); // effect-specific logic (damage, etc.)
            }

            if (effect.Duration <= 0f)
                RemoveEffect(i);
        }
    }

    private void RemoveEffect(int index)
    {
        ActiveStatusEffect effect = activeEffects[index];

        if (effect.spawnedVfx != null)
            Destroy(effect.spawnedVfx);

        // revert HP bar tint once the debuff ends
        if (effect.EffectSO.elementDebuff != elementDebuffType.None)
            characterHealth?.ChangeHpBar(effect.EffectSO.elementDebuff, false);

        activeEffects.RemoveAt(index);
    }
}
