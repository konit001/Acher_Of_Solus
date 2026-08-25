using UnityEngine;
using System.Collections.Generic;

// ที่เก็บของหนึ่งหน้าใน UI — ถือทั้งกองของที่เก็บไว้ และช่องบนหน้าจอของหน้าตัวเอง
// หน้า Inventory กับหน้า Equipment ต่างคนต่างมีกองของตัวเอง โดยสืบทอดจากคลาสนี้
public abstract class ItemStorage : MonoBehaviour
{
    [SerializeField] protected ItemSlotUI[] itemSlots;   // ช่องบนหน้าจอของหน้านี้ (ผูกใน Inspector)
    protected readonly List<InventoryEntry> entries = new List<InventoryEntry>();

    // จำนวนช่องที่หน้านี้มี — หน้าไหนมีกี่ช่องก็ดูจากที่ผูกไว้ใน Inspector
    public int SlotCount => itemSlots.Length;

    // ─── Public API ───────────────────────────────────────────

    public void AddItem(BaseItemData item)
    {
        if (item == null) return;

        var entry = FindEntry(item);
        if (entry == null)
            entries.Add(new InventoryEntry { item = item, amount = 1 });
        else
            entry.amount++;

        DisplayItems();
    }

    public void RemoveItem(BaseItemData item)
    {
        RemoveItem(item, 1);
    }

    public void RemoveItem(BaseItemData item, int amount)
    {
        var entry = FindEntry(item);
        if (entry == null) return;

        entry.amount -= amount;
        if (entry.amount <= 0)
            entries.Remove(entry);

        DisplayItems();
    }

    public bool HasItem(BaseItemData item)
    {
        return FindEntry(item) != null;
    }

    public int GetAmount(BaseItemData item)
    {
        return FindEntry(item)?.amount ?? 0;
    }

    // วาดกองของลงช่องบนหน้าจอ ช่องที่เหลือล้างให้ว่าง
    public void DisplayItems()
    {
        int slot = 0;

        // index ของกองของกับ index ของช่องไม่ตรงกัน เพราะบางชิ้นอาจถูกกรองไม่ให้แสดง
        for (int i = 0; i < entries.Count && slot < itemSlots.Length; i++)
        {
            if (!ShouldDisplay(entries[i].item)) continue;

            itemSlots[slot].slotIndex = slot;
            itemSlots[slot].SetItem(entries[i].item, entries[i].amount);
            slot++;
        }

        for (; slot < itemSlots.Length; slot++)
        {
            itemSlots[slot].slotIndex = slot;
            itemSlots[slot].Clear();
        }
    }

    // ─── Protected ────────────────────────────────────────────

    // หน้านี้จะโชว์อะไรบ้าง — ค่าเริ่มต้นโชว์ทุกอย่างที่เก็บไว้
    // หน้าที่มีแท็บย่อยค่อย override เพื่อกรองเอาเอง
    protected virtual bool ShouldDisplay(BaseItemData item) => true;

    protected InventoryEntry FindEntry(BaseItemData item)
    {
        for (int i = 0; i < entries.Count; i++)
            if (entries[i].item == item) return entries[i];
        return null;
    }
}
