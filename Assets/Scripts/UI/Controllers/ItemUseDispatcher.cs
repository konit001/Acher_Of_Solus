using UnityEngine;

public class ItemUseDispatcher : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private LootManager lootManager;

    public void Dispatch(BaseItemData item)
    {
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.LootBag:
                lootManager.OpenLootBag(item);
                break;
            case ItemType.Weapon:
                equipmentManager.EquipWeapon(item);
                break;
            case ItemType.Artifact:
                equipmentManager.EquipArtifact(item);
                break;
            case ItemType.Consumable:
                // consumableManager.Use(item); ← เพิ่มทีหลังได้เลย
                break;
            case ItemType.Coin:
                // coinManager.Collect(item);
                break;
        }
    }
}
