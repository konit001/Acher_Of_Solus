using UnityEngine;
using System.Collections.Generic;

// วางบน GameObject "Material" ที่มี GridLayoutGroup — ดูแลแถววัสดุทั้งหมดของช่องอาวุธหนึ่งช่อง
// โครงเดียวกับ StatusEffectUi: grid + prefab + spawn ลูกเข้าไปใน grid
public class MaterialListUI : MonoBehaviour
{
    public GameObject Ui;   // กลุ่มวัสดุทั้งอัน ซ่อนตอนไม่มีวัสดุต้องโชว์

    [Tooltip("เว้นว่างไว้ = ใช้ transform ของ GameObject เดียวกัน")]
    public Transform grid;
    public GameObject prefab;

    private readonly List<MaterialSlotUI> slots = new List<MaterialSlotUI>();
    private bool initialized;

    void Awake()
    {
        EnsureInitialized();
    }

    // ช่องอาวุธถูกสวมใส่ตั้งแต่ตอนแผง Equipment ยังปิดอยู่ ตัวนี้จึงถูกสั่งงานได้ก่อน Awake จะทำงาน
    // ต้องเตรียมตัวเองแบบเรียกซ้ำได้และทำจริงแค่ครั้งเดียว —
    // ของเดิม Awake ไปเก็บแถวที่ CreateSlot สร้างไปแล้วเข้า slots ซ้ำอีกรอบ
    // พอ ShowCosts รอบถัดมา HideExtraSlots ก็ไปซ่อนแถวที่เพิ่งโชว์ไป วัสดุเลยหายทั้งแผง
    private void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;

        if (grid == null) grid = transform;

        // แถวที่วางไว้ใน prefab อยู่แล้วเอามาใช้ก่อน จะได้ไม่สร้างซ้ำซ้อน
        slots.AddRange(grid.GetComponentsInChildren<MaterialSlotUI>(true));
    }

    // ─── Public API ───────────────────────────────────────────

    // แสดงวัสดุที่ต้องใช้ของขั้นอัปเกรดถัดไป — แถวไม่พอก็สร้างเพิ่ม ที่เกินก็ซ่อน
    public void ShowCosts(List<WeaponLevelData.MaterialCost> costs)
    {
        EnsureInitialized();

        int used = costs != null ? costs.Count : 0;

        SetUiActive(used > 0);

        for (int i = 0; i < used; i++)
        {
            if (slots.Count <= i) CreateSlot();
            if (slots.Count <= i) break;   // ยังไม่ได้ผูก prefab ใน Inspector

            UpdateSlot(slots[i], costs[i]);
        }

        HideExtraSlots(used);
    }

    public void HideAll()
    {
        EnsureInitialized();

        SetUiActive(false);
        HideExtraSlots(0);
    }

    // ─── Slot ─────────────────────────────────────────────────

    // spawn แถววัสดุเพิ่มอีกหนึ่งแถว
    public MaterialSlotUI CreateSlot()
    {
        EnsureInitialized();

        if (prefab == null)
        {
            Debug.LogWarning($"[{name}] ยังไม่ได้ผูก prefab ของแถววัสดุใน Inspector", this);
            return null;
        }

        GameObject instance = Instantiate(prefab, grid, false);
        MaterialSlotUI slot = instance.GetComponent<MaterialSlotUI>();
        slots.Add(slot);
        return slot;
    }

    // เขียนข้อมูลวัสดุหนึ่งชนิดลงแถวที่มีอยู่แล้ว
    public void UpdateSlot(MaterialSlotUI slot, WeaponLevelData.MaterialCost cost)
    {
        if (slot == null || cost == null) return;

        int have = WeaponProgressManager.instance != null
            ? WeaponProgressManager.instance.GetOwnedAmount(cost.material)
            : 0;

        slot.Show();
        slot.SetIcon(cost.material != null ? cost.material.itemImage : null);
        slot.SetFill(cost.amount > 0 ? (float)have / cost.amount : 1f);
        slot.SetAmount(have, cost.amount);
    }

    // ซ่อนแถวตั้งแต่ลำดับที่ usedCount เป็นต้นไป (ซ่อนไม่ Destroy เพราะถูกเรียกบ่อย)
    public void HideExtraSlots(int usedCount)
    {
        for (int i = usedCount; i < slots.Count; i++)
            if (slots[i] != null) slots[i].Hide();
    }

    // ─── Private ──────────────────────────────────────────────

    // guard Ui != gameObject: ถ้าลาก GameObject ตัวเองมาใส่ SetActive(false) จะปิดตัวเองจนสั่งงานต่อไม่ได้
    private void SetUiActive(bool active)
    {
        if (Ui != null && Ui != gameObject) Ui.SetActive(active);
    }
}
