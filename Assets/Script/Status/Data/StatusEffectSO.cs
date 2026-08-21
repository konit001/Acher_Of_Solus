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
}


