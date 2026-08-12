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

    public void AddItem(BaseItemData item)
        => inventoryManager.AddItem(item);

    public void RemoveItem(BaseItemData item)
        => inventoryManager.RemoveItem(item);

    public void UseItem(BaseItemData item)
        => itemUseDispatcher.Dispatch(item);

    public void UnEquipWeapon(Slot slotType)
        => equipmentManager.UnEquipWeapon(slotType);

    public void UnEquipArtifact(ArtifactSlot slot)
        => equipmentManager.UnEquipArtifact(slot);
}
