using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Buff
{
    public BuffEffect buffEffect;
    public float Duration;
    public int currentStack;
    public float tickTimer;

    public Buff(BuffEffect effect)
    {
        buffEffect = effect;
        Duration = effect.buffDuration;
        currentStack = 1;
        tickTimer = 0f;
    }
}

public class BuffController : MonoBehaviour
{
    private const float TickInterval = 1f;

    public List<Buff> activeBuffs = new List<Buff>();
    public event Action OnBuffsChanged;

    private CharacterHealth characterHealth;

    // เปิดให้ BuffEffect.OnApply เรียก characterHealth ได้ผ่าน property นี้
    public CharacterHealth Health => characterHealth;

    private void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();
    }

    public void ApplyBuff(BuffEffect effect)
    {
        if (effect == null) return;

        Buff instance = activeBuffs.Find(b => b.buffEffect == effect);
        if (instance != null)
        {
            instance.Duration = effect.buffDuration;
            instance.currentStack = Mathf.Min(instance.currentStack + 1, Mathf.Max(1, effect.buffStack));
        }
        else
        {
            instance = new Buff(effect);
            activeBuffs.Add(instance);
        }

        effect.OnApply(instance, this);
        OnBuffsChanged?.Invoke();
    }

    private void Update()
    {
        if (activeBuffs.Count == 0) return;

        bool changed = false;
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            BuffEffect effect = buff.buffEffect;

            buff.Duration -= Time.deltaTime;
            buff.tickTimer += Time.deltaTime;

            if (effect.buffAmountPerTick > 0f && buff.tickTimer >= TickInterval)
            {
                buff.tickTimer -= TickInterval;
                float tickAmount = effect.buffAmountPerTick * buff.currentStack;
                characterHealth?.takeDamage(-tickAmount);
            }

            if (buff.Duration <= 0f)
            {
                activeBuffs.RemoveAt(i);
                changed = true;
            }
        }

        if (changed) OnBuffsChanged?.Invoke();
    }

    public float GetStatModifierPercent(StatusType stat)
    {
        float total = 0f;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.buffEffect.affectedStat == stat)
            {
                total += buff.buffEffect.statModifierPercent * buff.currentStack;
            }
        }
        return total;
    }

    public bool HasBuffType(BuffType type)
    {
        return activeBuffs.Exists(b => b.buffEffect.buffType == type);
    }

    // ใช้โดย UI เพื่อหาว่าธาตุนี้กำลังมีบัฟ active อยู่หรือไม่ (เอาไอคอน/สีของตัว active มาแสดงแทน default)
    public ElementalBuffEffect GetActiveElementalBuff(ElementType element)
    {
        Buff match = activeBuffs.Find(b =>
            b.buffEffect is ElementalBuffEffect elemental && elemental.elementType == element);
        return match?.buffEffect as ElementalBuffEffect;
    }
}
