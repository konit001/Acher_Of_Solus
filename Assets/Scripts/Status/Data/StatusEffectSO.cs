using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatusChange
{
    public StatusType status;
    public float value;
}

public abstract class StatusEffectSO : ScriptableObject
{
    public string effectName;
    public float duration;
    public float tickInterval;
    public Sprite icon;

    public StatusEffectType statusEffectType;
    public ElementType element;
    public elementDebuffType elementDebuff;
    public GeneralDebuffType generalDebuff;
    public List<StatusChange> statModifiers;
    public int maxStack = 1;
    public bool refreshDurationOnStack = true;
    public GameObject vfxPrefab;

    public abstract void OnTick(GameObject target, ActiveStatusEffect instance);

    public virtual void OnApply(GameObject target, ActiveStatusEffect instance) { }
    public virtual void OnRemove(GameObject target, ActiveStatusEffect instance) { }
    public virtual void OnEffectUpdate(GameObject target, ActiveStatusEffect instance, float deltaTime) { }

    // looks up a configured StatusChange value by type; each subclass decides how to interpret it (flat vs. percent, etc.)
    protected float GetStatValue(StatusType type)
    {
        if (statModifiers == null) return 0f;
        foreach (StatusChange change in statModifiers)
            if (change.status == type) return change.value;
        return 0f;
    }
}


