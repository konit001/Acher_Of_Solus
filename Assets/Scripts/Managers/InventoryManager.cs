using UnityEngine;
using System.Collections.Generic;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] private ItemSlotUI[] itemSlots;
    private List<InventoryEntry> inventory = new List<InventoryEntry>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ─── Public API ───────────────────────────────────────────

    public void AddItem(BaseItemData item)
    {
        var entry = FindEntry(item);
        if (entry == null)
            inventory.Add(new InventoryEntry { item = item, amount = 1 });
        else
            entry.amount++;

        DisplayItems();
    }

    public void RemoveItem(BaseItemData item)
    {
        var entry = FindEntry(item);
        if (entry == null) return;

        entry.amount--;
        if (entry.amount <= 0)
            inventory.Remove(entry);

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

    // ─── Private ──────────────────────────────────────────────

    private InventoryEntry FindEntry(BaseItemData item)
    {
        for (int i = 0; i < inventory.Count; i++)
            if (inventory[i].item == item) return inventory[i];
        return null;
    }

    public void DisplayItems()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].slotIndex = i;
            if (i < inventory.Count)
                itemSlots[i].SetItem(inventory[i].item, inventory[i].amount);
            else
                itemSlots[i].Clear();
        }
    }
}
