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
    private Core core;

    // เปิดให้ DebuffEffect.OnApply เรียก characterHealth ได้ผ่าน property นี้
    public CharacterHealth Health => characterHealth;

    private void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();
        core = GetComponent<Core>();
    }

    public void ApplyDebuff(DebuffEffect effect)
    {
        if (effect == null) return;

        Debuff instance = activeDebuffs.Find(d => d.debuffEffect == effect);
        if (instance != null)
        {
            instance.Duration = effect.debuffDuration;
            instance.currentStack = Mathf.Min(instance.currentStack + 1, Mathf.Max(1, effect.debuffStack));
            Debug.Log($"[Debuff] {gameObject.name}: ต่ออายุ '{effect.debuffName}' (stack {instance.currentStack}/{Mathf.Max(1, effect.debuffStack)})");
        }
        else
        {
            instance = new Debuff(effect);
            activeDebuffs.Add(instance);
            string elementInfo = effect is ElementalDebuffEffect elemental ? elemental.elementType.ToString() : "General";
            Debug.Log($"[Debuff] {gameObject.name}: ติดดีบัฟใหม่ '{effect.debuffName}' ธาตุ {elementInfo} ระยะเวลา {effect.debuffDuration}s");
        }

        effect.OnApply(instance, this);
        RecomputeLockFlags();
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

        if (changed)
        {
            RecomputeLockFlags();
            OnDebuffsChanged?.Invoke();
        }
    }

    // รวม disablesMovement/disablesAction จากดีบัฟที่ active ทั้งหมดแบบ OR แล้วสั่ง Core
    // ต้องคำนวณใหม่ทุกครั้งที่ list เปลี่ยน (ไม่ toggle เดี่ยวๆ ต่อดีบัฟ) เพื่อไม่ให้ปลดล็อกพลาด
    // ตอนมีดีบัฟล็อกซ้อนกันหลายตัวแล้วตัวหนึ่งหมดอายุก่อน
    private void RecomputeLockFlags()
    {
        if (core == null) return;

        bool movementLocked = false;
        bool actionLocked = false;

        foreach (Debuff debuff in activeDebuffs)
        {
            if (debuff.debuffEffect.disablesMovement) movementLocked = true;
            if (debuff.debuffEffect.disablesAction) actionLocked = true;
        }

        core.SetMovementDisabled(movementLocked);
        core.SetActionDisabled(actionLocked);
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

    // ใช้โดย UI เพื่อหาว่าธาตุนี้กำลังมีดีบัฟ active อยู่หรือไม่ (เอาไอคอน/สีของตัว active มาแสดงแทน default)
    public ElementalDebuffEffect GetActiveElementalDebuff(ElementType element)
    {
        Debuff match = activeDebuffs.Find(d =>
            d.debuffEffect is ElementalDebuffEffect elemental && elemental.elementType == element);
        return match?.debuffEffect as ElementalDebuffEffect;
    }
}
