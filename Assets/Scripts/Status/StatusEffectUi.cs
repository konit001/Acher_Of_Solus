using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectUi : MonoBehaviour
{
    public GameObject Ui;   // แผงทั้งอัน ซ่อนตอนไม่มีสถานะติด
    public Transform grid;
    public GameObject prefab;

    private readonly Dictionary<ActiveStatusEffect, StatusEffectIconSlot> activeSlots
        = new Dictionary<ActiveStatusEffect, StatusEffectIconSlot>();

    [Tooltip("เว้นว่างไว้ = ใช้ StatusEffectController บน GameObject เดียวกัน")]
    public StatusEffectController target;

    private StatusEffectController ownController;

    void Awake()
    {
        ownController = GetComponent<StatusEffectController>();
    }

    // ลำดับการเลือก: ช่องที่ตั้งเอง → controller บนตัวเอง → ของผู้เล่น (เผื่อ HUD ที่วางบน Canvas ลอยๆ)
    // หาใหม่ทุกเฟรม ไม่ cache ผลลัพธ์ เพราะ StatusEffectManager.Instance ยังไม่มีตอน Awake
    private StatusEffectController ResolveController()
    {
        if (target != null) return target;
        if (ownController != null) return ownController;
        return StatusEffectManager.Instance?.Player;
    }

    void Update()
    {
        // ยังไม่มี event ตอน add/remove ใน StatusEffectController จึง poll ทุกเฟรม
        StatusEffectController controller = ResolveController();
        if (controller == null) return;

        IReadOnlyList<ActiveStatusEffect> effects = controller.ActiveEffects;

        // ซ่อนแผงทั้งอันตอนไม่มีสถานะติด
        // guard Ui != gameObject: ถ้าลาก GameObject ตัวเองมาใส่ SetActive(false) จะปิด component นี้จน Update ไม่ถูกเรียกอีกเลย
        if (Ui != null && Ui != gameObject)
            Ui.SetActive(effects.Count > 0);

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
