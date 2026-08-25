using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UI.Input;
public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("--- Managers ---")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private LootManager lootManager;

    [Header("--- Controllers ---")]
    [SerializeField] private UiPanelController panelController;
    [SerializeField] private ItemUseDispatcher itemUseDispatcher;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        panelController.HandleInput();
    }

    // ─── Public API (delegate ต่อทั้งหมด) ────────────────────

    // ด่านแยกทางจุดเดียวของเกม — ของชิ้นไหนควรอยู่หน้าไหน ตัดสินที่นี่
    public ItemStorage StorageFor(BaseItemData item)
        => ItemPageRouting.BelongsToEquipmentPage(item)
            ? (ItemStorage)equipmentManager
            : inventoryManager;

    public void AddItem(BaseItemData item)
    {
        if (item == null) return;
        StorageFor(item).AddItem(item);
    }

    public void RemoveItem(BaseItemData item)
        => RemoveItem(item, 1);

    public void RemoveItem(BaseItemData item, int amount)
    {
        if (item == null) return;
        StorageFor(item).RemoveItem(item, amount);
    }

    public void UseItem(BaseItemData item)
        => itemUseDispatcher.Dispatch(item);

    public void UnEquipWeapon(Slot slotType)
        => equipmentManager.UnEquipWeapon(slotType);

    public void UnEquipArtifact(ArtifactSlot slot)
        => equipmentManager.UnEquipArtifact(slot);
}
