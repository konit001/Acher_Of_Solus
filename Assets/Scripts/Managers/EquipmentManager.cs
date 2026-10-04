using UnityEngine;

// หน้า Equipment — เก็บ weapon กับ artifact ในกองของหน้าตัวเอง (สืบทอดมาจาก ItemStorage)
// และดูแลช่องสวมใส่ (socket) ทั้งอาวุธและ artifact
public class EquipmentManager : ItemStorage
{
    public static EquipmentManager instance;

    [SerializeField] private weaponSlot[] weaponSlots;
    [SerializeField] private weaponSlot[] hudWeaponSlots;
    [Tooltip("การ์ดอาวุธในแท็บ Upgrade (Name/Level/ปุ่ม UPGRADE) — แยกจาก weaponSlots/hudWeaponSlots เพราะเป็นช่องแสดงผลคนละจุดกัน")]
    [SerializeField] private weaponSlot[] upgradeWeaponSlots;
    [SerializeField] private ArtifactSlot[] artifactSlots;



    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ─── แท็บย่อย ─────────────────────────────────────────────

    public void ShowCategory(EquipmentCategory category)
    {
        DisplayItems();
    }

    // ของถูกเก็บรวมกันทั้ง weapon และ artifact กริดโชว์ทั้งคู่พร้อมกันเสมอ
    protected override bool ShouldDisplay(BaseItemData item)
    {
        if (item == null) return false;
        return item.itemType == ItemType.Weapon || item.itemType == ItemType.Artifact;
    }

    // ─── Weapon ───────────────────────────────────────────────

    public void EquipWeapon(weaponsData weapon)
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
            AddItem(weaponSlots[targetIndex].GetCurrentItem());
        }

        weaponSlots[targetIndex].SetItem(weapon, 1);
        hudWeaponSlots[targetIndex].SetItem(weapon, 1);
        if (upgradeWeaponSlots != null && targetIndex < upgradeWeaponSlots.Length)
            upgradeWeaponSlots[targetIndex].SetItem(weapon, 1);
        RemoveItem(weapon);
    }

    public void UnEquipWeapon(Slot slotType)
    {
        int index = (int)slotType;
        AddItem(weaponSlots[index].GetCurrentItem());
        weaponSlots[index].Clear();
        hudWeaponSlots[index].Clear();
        if (upgradeWeaponSlots != null && index < upgradeWeaponSlots.Length)
            upgradeWeaponSlots[index].Clear();
    }

    // ─── Artifact ─────────────────────────────────────────────

    public void EquipArtifact(artifactData artifact)
    {
        if (artifact == null || artifact.itemType != ItemType.Artifact) return;

        int targetIndex = -1;
        for (int i = 0; i < artifactSlots.Length; i++)
        {
            if (artifactSlots[i].artifactType == artifact.artifactType)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1)
        {
            Debug.LogWarning("ไม่มีช่องสวมใส่สำหรับ Artifact ประเภท: " + artifact.artifactType);
            return;
        }

        if (!artifactSlots[targetIndex].IsEmpty)
            AddItem(artifactSlots[targetIndex].GetCurrentItem());

        artifactSlots[targetIndex].SetItem(artifact, 1);
        RemoveItem(artifact);
    }

    public void UnEquipArtifact(ArtifactSlot slot)
    {
        if (slot == null || slot.IsEmpty) return;
        AddItem(slot.GetCurrentItem());
        slot.Clear();
    }
}
