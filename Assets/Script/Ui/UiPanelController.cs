using UnityEngine;
using UI.Input;

public enum UiPanelType { Inventory, Equipment, Skill, Map, Tasks, Codex }

public class UiPanelController : MonoBehaviour
{
    public static UiPanelController instance;

    [SerializeField] private GameObject uiPanel;        // กรอบหน้าต่างหลักที่ครอบ MenuBarController (Inventory/Equipment/Skill/Map/Tasks/Codex)
    // [SerializeField] private GameObject equipmentPanel; // แผงอุปกรณ์สวมใส่ ที่โชว์คู่กับแท็บ Inventory/Equipment

    public bool IsPanelOpen { get; private set; }
    public UiPanelType? CurrentPanel { get; private set; }

    // เก็บไว้เพื่อความเข้ากันได้กับโค้ดเดิม (ArtifactSlot / weaponSlot ใช้เช็คตอนคลิกขวาเพื่อถอดของ)
    public bool IsInventoryOpen => IsPanelOpen && CurrentPanel == UiPanelType.Inventory;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ─── Public API ───────────────────────────────────────────

    public void HandleInput()
    {
        // ปุ่ม ESC ไม่ได้จัดการที่นี่ ให้ PauseController เป็นคนตัดสินใจแทน
        if (uiInput.instance.InventoryUiInput) TogglePanel(UiPanelType.Inventory);
        else if (uiInput.instance.EquipmentUiInput) TogglePanel(UiPanelType.Equipment);
        else if (uiInput.instance.skillUiInput) TogglePanel(UiPanelType.Skill);
        else if (uiInput.instance.MapUiInput) TogglePanel(UiPanelType.Map);
        else if (uiInput.instance.TasksUiInput) TogglePanel(UiPanelType.Tasks);
        else if (uiInput.instance.CodexUiInput) TogglePanel(UiPanelType.Codex);
    }

    public void TogglePanel(UiPanelType panel)
    {
        if (IsPanelOpen && CurrentPanel == panel)
            CloseAll();
        else
            OpenPanel(panel);
    }

    public void OpenPanel(UiPanelType panel)
    {
        IsPanelOpen = true;
        CurrentPanel = panel;
        uiPanel.SetActive(true);
        Time.timeScale = 0;

        MenuBarController.instance.OpenPage((int)panel);
        // equipmentPanel.SetActive(panel == UiPanelType.Inventory || panel == UiPanelType.Equipment);

        if (panel == UiPanelType.Inventory)
            InventoryManager.instance.DisplayItems();
    }

    // เปลี่ยนให้เป็น public เพื่อให้ PauseController เรียกใช้งานคำสั่งนี้ได้
    public void CloseAll()
    {
        IsPanelOpen = false;
        CurrentPanel = null;
        uiPanel.SetActive(false);
        // equipmentPanel.SetActive(false);
        Time.timeScale = 1;

        MenuBarController.instance?.CloseAll();
    }

    // คงไว้เพื่อความเข้ากันได้กับโค้ดเดิม (PauseController เรียกใช้ตอนกดปุ่ม Inventory จาก Pause Menu)
    public void ToggleInventory() => TogglePanel(UiPanelType.Inventory);
}