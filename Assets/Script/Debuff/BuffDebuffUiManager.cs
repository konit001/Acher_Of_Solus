using UnityEngine;
using System.Collections.Generic;

// แทนที่บทบาทของ DebuffUiManager เดิม แบบ generic ใช้ได้ทั้ง player/enemy
// ผูกกับ GameObject ไหนก็ได้ที่มี DebuffController/BuffController (+ Gauge controller ถ้ามี)
public class BuffDebuffUiManager : MonoBehaviour
{
    [Header("References")]
    public DebuffController debuffController;
    public BuffController buffController;
    public DebuffGaugeController debuffGaugeController;
    public BuffGaugeController buffGaugeController;

    [Header("UI")]
    public GameObject iconSlotPrefab; // ชี้ Buff_Debuff_Icon_Canvas (ต้องมี BuffDebuffIconSlot ติดอยู่)
    public Transform gridContainer;   // ชี้ Buff_Debuff_Canvas

    [System.Serializable]
    public class ElementIconEntry
    {
        public ElementType element;
        public Sprite defaultIcon;
    }

    [Header("Element Slots (อยู่หน้าสุดเสมอ เรียงตามลิสต์นี้)")]
    public List<ElementIconEntry> trackedElements = new List<ElementIconEntry>();

    private class DynamicEntry
    {
        public GameObject instance;
        public BuffDebuffIconSlot slot;
    }

    private readonly List<BuffDebuffIconSlot> elementSlots = new List<BuffDebuffIconSlot>();
    private readonly List<DynamicEntry> dynamicEntries = new List<DynamicEntry>();

    private void Awake()
    {
        BuildElementSlots();
    }

    private void OnEnable()
    {
        if (debuffController != null) debuffController.OnDebuffsChanged += RefreshDynamicEntries;
        if (buffController != null) buffController.OnBuffsChanged += RefreshDynamicEntries;
        RefreshDynamicEntries();
    }

    private void OnDisable()
    {
        if (debuffController != null) debuffController.OnDebuffsChanged -= RefreshDynamicEntries;
        if (buffController != null) buffController.OnBuffsChanged -= RefreshDynamicEntries;
    }

    private void Update()
    {
        UpdateElementSlots();
    }

    private void BuildElementSlots()
    {
        if (iconSlotPrefab == null || gridContainer == null) return;

        foreach (ElementIconEntry entry in trackedElements)
        {
            GameObject instance = Instantiate(iconSlotPrefab, gridContainer);
            BuffDebuffIconSlot slot = instance.GetComponent<BuffDebuffIconSlot>();
            if (slot != null)
            {
                slot.SetIcon(entry.defaultIcon);
                slot.SetFill(0f);
            }
            elementSlots.Add(slot);
        }
    }

    private void UpdateElementSlots()
    {
        for (int i = 0; i < trackedElements.Count && i < elementSlots.Count; i++)
        {
            BuffDebuffIconSlot slot = elementSlots[i];
            if (slot == null) continue;

            ElementType element = trackedElements[i].element;

            ElementalDebuffEffect activeDebuff = debuffController != null ? debuffController.GetActiveElementalDebuff(element) : null;
            ElementalBuffEffect activeBuff = buffController != null ? buffController.GetActiveElementalBuff(element) : null;

            if (activeDebuff != null)
            {
                slot.SetIcon(activeDebuff.icon != null ? activeDebuff.icon : trackedElements[i].defaultIcon);
                slot.SetFill(1f);
            }
            else if (activeBuff != null)
            {
                slot.SetIcon(activeBuff.icon != null ? activeBuff.icon : trackedElements[i].defaultIcon);
                slot.SetFill(1f);
            }
            else
            {
                slot.SetIcon(trackedElements[i].defaultIcon);
                float debuffRatio = debuffGaugeController != null ? debuffGaugeController.GetGaugeRatio(element) : 0f;
                float buffRatio = buffGaugeController != null ? buffGaugeController.GetGaugeRatio(element) : 0f;
                slot.SetFill(Mathf.Max(debuffRatio, buffRatio));
            }
        }
    }

    // ช่องไดนามิก: debuff/buff แบบ General (ไม่ผูกธาตุ) ที่กำลัง active — สร้าง/ทำลายตามรายการจริง ต่อท้ายช่องธาตุ
    private void RefreshDynamicEntries()
    {
        foreach (DynamicEntry entry in dynamicEntries)
        {
            if (entry.instance != null) Destroy(entry.instance);
        }
        dynamicEntries.Clear();

        if (iconSlotPrefab == null || gridContainer == null) return;

        if (debuffController != null)
        {
            foreach (Debuff debuff in debuffController.activeDebuffs)
            {
                if (debuff.debuffEffect is GeneralDebuffEffect general)
                {
                    SpawnDynamicSlot(general.icon);
                }
            }
        }

        if (buffController != null)
        {
            foreach (Buff buff in buffController.activeBuffs)
            {
                if (buff.buffEffect is GeneralBuffEffect general)
                {
                    SpawnDynamicSlot(general.icon);
                }
            }
        }
    }

    private void SpawnDynamicSlot(Sprite icon)
    {
        GameObject instance = Instantiate(iconSlotPrefab, gridContainer);
        BuffDebuffIconSlot slot = instance.GetComponent<BuffDebuffIconSlot>();
        if (slot != null)
        {
            slot.SetIcon(icon);
            slot.SetFill(1f);
        }
        dynamicEntries.Add(new DynamicEntry { instance = instance, slot = slot });
    }
}
