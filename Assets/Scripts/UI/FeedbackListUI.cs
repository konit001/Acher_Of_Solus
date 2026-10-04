using UnityEngine;
using System.Collections.Generic;

// วางบน GameObject "Feebback" ใน Persistence.prefab — กล่องรวมแถว toast เหนือช่องอาวุธ
// โครงเดียวกับ MaterialListUI: prefab + spawn ลูกเข้าไป แล้วซ่อนแทนการ Destroy
public class FeedbackListUI : MonoBehaviour
{
    [SerializeField] private GameObject prefab;       // FeedbackSlot.prefab
    [SerializeField] private float holdTime = 2f;     // ค้างให้อ่านกี่วินาทีก่อนเริ่มจาง
    [SerializeField] private float fadeTime = 0.5f;

    private readonly List<FeedbackSlotUI> slots = new List<FeedbackSlotUI>();

    void Awake()
    {
        // แถวที่วางไว้ในฉากอยู่แล้วเอามาใช้ก่อน จะได้ไม่สร้างซ้ำซ้อน
        slots.AddRange(GetComponentsInChildren<FeedbackSlotUI>(true));
        foreach (var slot in slots) slot.Hide();
    }

    // ─── Public API ───────────────────────────────────────────

    public void Show(BaseItemData item)
    {
        if (item == null) return;
        if (item.itemType == ItemType.Coin) return;   // เหรียญมีตัวนับของตัวเองที่ CoinManager แล้ว

        FeedbackSlotUI slot = FindSlot(item) ?? FreeSlot() ?? CreateSlot();
        if (slot == null) return;                     // ยังไม่ได้ผูก prefab ใน Inspector

        slot.gameObject.SetActive(true);
        slot.Show(item, holdTime, fadeTime);
    }

    // ─── Private ──────────────────────────────────────────────

    // แถวที่โชว์ของชิ้นนี้ค้างอยู่ — เจอแล้วเอามาบวกจำนวนทับ
    private FeedbackSlotUI FindSlot(BaseItemData item)
    {
        foreach (var slot in slots)
            if (slot.gameObject.activeSelf && slot.Item == item) return slot;

        return null;
    }

    private FeedbackSlotUI FreeSlot()
    {
        foreach (var slot in slots)
            if (!slot.gameObject.activeSelf) return slot;

        return null;
    }

    private FeedbackSlotUI CreateSlot()
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[{name}] ยังไม่ได้ผูก prefab ของแถว feedback ใน Inspector", this);
            return null;
        }

        var slot = Instantiate(prefab, transform, false).GetComponent<FeedbackSlotUI>();
        slots.Add(slot);
        return slot;
    }
}
