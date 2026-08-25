using UnityEngine;

// หน้า Equipment — เก็บ weapon กับ artifact ในกองของหน้าตัวเอง (สืบทอดมาจาก ItemStorage)
// และดูแลช่องสวมใส่ (socket) ทั้งอาวุธและ artifact
public class EquipmentManager : ItemStorage
{
    public static EquipmentManager instance;

    [SerializeField] private weaponSlot[] weaponSlots;
    [SerializeField] private weaponSlot[] hudWeaponSlots;
    [SerializeField] private ArtifactSlot[] artifactSlots;

    [Header("--- ฝั่งขวาของหน้า Equipment ---")]
    [Tooltip("GameObject 'EquipmentItem' ที่เป็นกริดเก็บของ — โชว์ตอนแท็บ Weapon / Artifact")]
    [SerializeField] private GameObject itemGridRoot;
    [Tooltip("GameObject 'SkillTree' — โชว์ตอนแท็บ Charector")]
    [SerializeField] private GameObject skillTreeRoot;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ─── แท็บย่อย ─────────────────────────────────────────────

    // แท็บ Charector ไม่รับ item เลย โชว์ skill tree แทนกริด
    public void ShowCategory(EquipmentCategory category)
    {
        bool isCharector = category == EquipmentCategory.Charector;

        if (skillTreeRoot != null) skillTreeRoot.SetActive(isCharector);
        if (itemGridRoot != null) itemGridRoot.SetActive(!isCharector);

        if (!isCharector) DisplayItems();
    }

    // ของถูกเก็บรวมกันทั้ง weapon และ artifact แท็บมีผลแค่ตอนแสดงเท่านั้น
    protected override bool ShouldDisplay(BaseItemData item)
    {
        if (item == null || UiPanelController.instance == null) return false;

        switch (UiPanelController.instance.CurrentCategory)
        {
            case EquipmentCategory.Weapon:
                return item.itemType == ItemType.Weapon;

            case EquipmentCategory.Artifact:
                return item.itemType == ItemType.Artifact;

            default:
                return false;   // แท็บ Charector ซ่อนกริดอยู่แล้ว ไม่ต้องวาดอะไร
        }
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
        RemoveItem(weapon);
    }

    public void UnEquipWeapon(Slot slotType)
    {
        int index = (int)slotType;
        AddItem(weaponSlots[index].GetCurrentItem());
        weaponSlots[index].Clear();
        hudWeaponSlots[index].Clear();
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
