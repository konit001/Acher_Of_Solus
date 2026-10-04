using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ฝั่งขวาของแผง Upgrade — แสดงสกิลที่อาวุธที่เลือกอยู่ปลดล็อกได้ทั้งหมด
// โครงเดียวกับ MaterialListUI: grid + prefab + spawn ลูกเข้าไปใน grid
public class SkillListUI : MonoBehaviour
{
    public static SkillListUI instance;

    public Transform grid;
    public GameObject prefab; // SkillSlot.prefab
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI skillPointText;

    private readonly List<SkillSlotUI> slots = new List<SkillSlotUI>();

    void Awake()
    {
        instance = this;
        if (grid == null) grid = transform;
        slots.AddRange(grid.GetComponentsInChildren<SkillSlotUI>(true));
    }

    public void ShowForWeapon(weaponsData weapon)
    {
        if (weaponNameText != null) weaponNameText.text = weapon != null ? weapon.itemName : "";

        // SkillTreeManager อาจยังไม่เกิดตอนอาวุธเริ่มเกมถูกสวมใส่ — กันไว้ไม่ให้ NullReferenceException ตรงนี้ทำให้
        // EquipWeapon() ที่เรียก ShowInfo() อยู่ต้องหยุดกลางคัน (จะทำให้ hudWeaponSlots ของอาวุธชิ้นถัดไปไม่ถูกอัปเดต)
        if (SkillTreeManager.Instance == null)
        {
            if (skillPointText != null) skillPointText.text = "";
            foreach (SkillSlotUI slot in slots)
                if (slot != null) slot.Bind(default, 0);
            return;
        }

        if (skillPointText != null) skillPointText.text = "Skill Point : " + SkillTreeManager.Instance.skillPoints;

        int i = 0;
        if (weapon != null)
        {
            int weaponLevel = WeaponProgressManager.instance != null ? WeaponProgressManager.instance.GetLevel(weapon) : 0;
            foreach (weaponsData.WeaponSkillUnlock unlock in weapon.UnlockableSkills)
            {
                if (slots.Count <= i) CreateSlot();
                if (slots.Count <= i) break; // ยังไม่ได้ผูก prefab ใน Inspector

                if (slots[i] != null) slots[i].Bind(unlock, weaponLevel);
                i++;
            }
        }

        for (; i < slots.Count; i++)
            if (slots[i] != null) slots[i].Bind(default, 0);
    }

    private void CreateSlot()
    {
        if (prefab == null) return;

        GameObject instance = Instantiate(prefab, grid, false);
        SkillSlotUI slot = instance.GetComponent<SkillSlotUI>();
        if (slot == null)
        {
            Debug.LogWarning($"[{name}] prefab ของ SkillSlot ไม่มี component SkillSlotUI", this);
            Destroy(instance);
            return;
        }
        slots.Add(slot);
    }
}
