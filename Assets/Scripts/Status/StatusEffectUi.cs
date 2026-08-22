using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectUi : MonoBehaviour
{
    public GameObject Ui; // wired in the scene but not read yet
    public Transform grid;
    public GameObject prefab;

    private readonly Dictionary<ActiveStatusEffect, StatusEffectIconSlot> activeSlots
        = new Dictionary<ActiveStatusEffect, StatusEffectIconSlot>();

    void Update()
    {
        // no add/remove events on StatusEffectController, so poll every frame
        StatusEffectController controller = StatusEffectManager.Instance?.Player;
        if (controller == null) return;

        IReadOnlyList<ActiveStatusEffect> effects = controller.ActiveEffects;

        foreach (ActiveStatusEffect effect in effects)
        {
            if (!activeSlots.TryGetValue(effect, out StatusEffectIconSlot slot))
                slot = CreateIcon(effect);

            UpdateIcon(slot, effect);
        }

        RemoveStaleIcons(effects);
    }

    // spawn an icon slot for a newly active effect
    public StatusEffectIconSlot CreateIcon(ActiveStatusEffect effect)
    {
        GameObject instance = Instantiate(prefab, grid);
        StatusEffectIconSlot slot = instance.GetComponent<StatusEffectIconSlot>();
        slot.SetIcon(effect.effectSO.icon);
        activeSlots.Add(effect, slot);
        return slot;
    }

    // refresh fill/stack display for an already-tracked slot
    public void UpdateIcon(StatusEffectIconSlot slot, ActiveStatusEffect effect)
    {
        slot.SetFill(effect.remainingTime / effect.effectSO.duration);
        slot.SetStack(effect.stackCount);
    }

    // destroy slots whose effect has expired or been removed
    public void RemoveStaleIcons(IReadOnlyList<ActiveStatusEffect> currentEffects)
    {
        List<ActiveStatusEffect> toRemove = null;

        foreach (KeyValuePair<ActiveStatusEffect, StatusEffectIconSlot> pair in activeSlots)
        {
            if (!currentEffects.Contains(pair.Key))
            {
                Destroy(pair.Value.gameObject);
                (toRemove ??= new List<ActiveStatusEffect>()).Add(pair.Key);
            }
        }

        if (toRemove == null) return;
        foreach (ActiveStatusEffect key in toRemove)
            activeSlots.Remove(key);
    }
}
