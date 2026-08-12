using UnityEngine;
using UI.Input;

public class UiPanelController : MonoBehaviour
{
    public static UiPanelController instance;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equipmentPanel;

    public bool IsInventoryOpen { get; private set; }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ─── Public API ───────────────────────────────────────────

    public void HandleInput()
    {
        // ให้จัดการเฉพาะปุ่มเปิดกระเป๋า (ส่วนปุ่ม ESC เราย้ายไปให้ PauseController ตัดสินใจแทนแล้ว)
        if (uiInput.instance.EquipmentUiInput)
            ToggleInventory();
    }

    public void SetInventoryOpen(bool open)
    {
        IsInventoryOpen = open;
        inventoryPanel.SetActive(open);
        Time.timeScale = open ? 0 : 1;

        if (open)
            InventoryManager.instance.DisplayItems();
    }

    // เปลี่ยนให้เป็น public เพื่อให้ PauseController เรียกใช้งานคำสั่งนี้ได้
    public void CloseAll()
    {
        SetInventoryOpen(false);
        equipmentPanel.SetActive(false);
    }

    // ─── Private (หรือจะปล่อยเป็น public ก็ได้) ──────────────

    public void ToggleInventory()
    {
        SetInventoryOpen(!IsInventoryOpen);
        equipmentPanel.SetActive(true); // อันนี้ตอนปิด CloseAll มันจะซ่อนให้เอง
    }
}