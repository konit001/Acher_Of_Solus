using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [SerializeField] private weaponSlot[] weaponSlots;
    [SerializeField] private weaponSlot[] hudWeaponSlots;
    [SerializeField] private ArtifactSlot[] artifactSlots;

       private InventoryManager inventoryManager
        => InventoryManager.instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void Start()
    {
        
    }
    // ─── Weapon ───────────────────────────────────────────────

    public void EquipWeapon(BaseItemData weapon)
    {
        if (weapon == null || weapon.itemType != ItemType.Weapon) return;

        int targetIndex = 0;
        bool isFull = true;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].IsEmpty)
            {
                targetIndex = i;
                isFull = false;
                break;
            }
            targetIndex = i;
        }

        if (isFull)
        {
            targetIndex = 0;
            inventoryManager.AddItem(weaponSlots[targetIndex].GetCurrentItem());
        }

        weaponSlots[targetIndex].SetItem(weapon, 1);
        hudWeaponSlots[targetIndex].SetItem(weapon, 1);
        inventoryManager.RemoveItem(weapon);
    }

    public void UnEquipWeapon(Slot slotType)
    {
        int index = (int)slotType;
        inventoryManager.AddItem(weaponSlots[index].GetCurrentItem());
        weaponSlots[index].Clear();
        hudWeaponSlots[index].Clear();
    }

    // ─── Artifact ─────────────────────────────────────────────

    public void EquipArtifact(BaseItemData artifact)
    {
        if (artifact == null || artifact.itemType != ItemType.Artifact) return;

        artifactData artiData = artifact as artifactData;
        if (artiData == null) return;

        int targetIndex = -1;
        for (int i = 0; i < artifactSlots.Length; i++)
        {
            if (artifactSlots[i].artifactType == artiData.artifactType)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1)
        {
            Debug.LogWarning("ไม่มีช่องสวมใส่สำหรับ Artifact ประเภท: " + artiData.artifactType);
            return;
        }

        if (!artifactSlots[targetIndex].IsEmpty)
            inventoryManager.AddItem(artifactSlots[targetIndex].GetCurrentItem());

        artifactSlots[targetIndex].SetItem(artifact, 1);
        inventoryManager.RemoveItem(artifact);
    }

    public void UnEquipArtifact(ArtifactSlot slot)
    {
        if (slot == null || slot.IsEmpty) return;
        inventoryManager.AddItem(slot.GetCurrentItem());
        slot.Clear();
    }
}

