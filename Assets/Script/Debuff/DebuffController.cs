using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Debuff
{
    public DebuffEffect debuffEffect;
    public float Duration;
    public int currentStack;
    public float tickTimer;

    public Debuff(DebuffEffect effect)
    {
        debuffEffect = effect;
        Duration = effect.debuffDuration;
        currentStack = 1;
        tickTimer = 0f;
    }
}

public class DebuffController : MonoBehaviour
{
    private const float TickInterval = 1f;

    public List<Debuff> activeDebuffs = new List<Debuff>();
    public event Action OnDebuffsChanged;

    private CharacterHealth characterHealth;

    private void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();
    }

    public void ApplyDebuff(DebuffEffect effect)
    {
        if (effect == null) return;

        Debuff _Debuff = activeDebuffs.Find(d => d.debuffEffect == effect);
        if (_Debuff != null)
        {
            _Debuff.Duration = effect.debuffDuration;
            _Debuff.currentStack = Mathf.Min(_Debuff.currentStack + 1, Mathf.Max(1, effect.debuffStack));
            Debug.Log($"[Debuff] {gameObject.name}: ต่ออายุ '{effect.debuffName}' (stack {_Debuff.currentStack}/{Mathf.Max(1, effect.debuffStack)})");
        }
        else
        {
            activeDebuffs.Add(new Debuff(effect));
            Debug.Log($"[Debuff] {gameObject.name}: ติดดีบัฟใหม่ '{effect.debuffName}' ธาตุ {effect.elementType} ระยะเวลา {effect.debuffDuration}s");
            if (effect.debuffDamage > 0f)
            {
                characterHealth?.takeDamage(effect.debuffDamage);
                Debug.Log($"[Debuff] {gameObject.name}: โดนดาเมจแรกเข้าจาก '{effect.debuffName}' {effect.debuffDamage}");
            }
        }

        OnDebuffsChanged?.Invoke();
    }

    private void Update()
    {
        if (activeDebuffs.Count == 0) return;

        bool changed = false;
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            Debuff debuff = activeDebuffs[i];
            DebuffEffect effect = debuff.debuffEffect;

            debuff.Duration -= Time.deltaTime;
            debuff.tickTimer += Time.deltaTime;

            if (effect.debuffDamagePerTick > 0f && debuff.tickTimer >= TickInterval)
            {
                debuff.tickTimer -= TickInterval;
                float tickDamage = effect.debuffDamagePerTick * debuff.currentStack;
                characterHealth?.takeDamage(tickDamage);
                Debug.Log($"[Debuff] {gameObject.name}: '{effect.debuffName}' ติ๊กดาเมจ {tickDamage} (เหลือเวลา {debuff.Duration:F1}s)");
            }

            if (debuff.Duration <= 0f)
            {
                activeDebuffs.RemoveAt(i);
                changed = true;
                Debug.Log($"[Debuff] {gameObject.name}: '{effect.debuffName}' หมดอายุแล้ว");
            }
        }

        if (changed) OnDebuffsChanged?.Invoke();
    }

    public float GetStatModifierPercent(StatusType stat)
    {
        float total = 0f;
        foreach (Debuff debuff in activeDebuffs)
        {
            if (debuff.debuffEffect.affectedStat == stat)
            {
                total += debuff.debuffEffect.statModifierPercent * debuff.currentStack;
            }
        }
        return total;
    }

    public bool HasDebuffType(DebuffType type)
    {
        return activeDebuffs.Exists(d => d.debuffEffect.debuffType == type);
    }
}
