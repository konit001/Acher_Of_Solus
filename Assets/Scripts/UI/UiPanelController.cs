using UnityEngine;
using UI.Input;

public enum UiPanelType
{
    Inventory,
    Equipment,
    // ไม่เคยถูกใช้จริง (SetCurrentPanel ไม่มี branch ให้) — หน้า skill tree จริง ๆ อยู่ใต้
    // EquipmentManager.EquipmentCategory.Character แทน ห้ามลบค่านี้เพราะจะเลื่อนเลข int ของ
    // Map/Tasks/Codex ที่ serialize ไว้ในที่อื่นให้เสีย ถ้าจะเลิกใช้ concept นี้จริงต้องคุยกับผู้ใช้ก่อน
    [System.Obsolete("ไม่เคยถูกใช้จริง — skill tree UI อยู่ใต้ Equipment/Character แทน")]
    Skill,
    Map,
    Tasks,
    Codex
}

public class UiPanelController : MonoBehaviour
{
    public static UiPanelController instance;

    [SerializeField] private GameObject uiPanel;
    public bool IsPanelOpen { get; private set; }
    public UiPanelType? CurrentPanel { get; private set; }

    public bool IsInventoryOpen => IsPanelOpen && CurrentPanel == UiPanelType.Inventory;
    public bool IsEquipmentOpen => IsPanelOpen && CurrentPanel == UiPanelType.Equipment;
    [SerializeField] private UiPanelType lastPanel = UiPanelType.Inventory;
    public UiPanelType LastPanel => lastPanel;
    public EquipmentCategory CurrentCategory { get; private set; } = EquipmentCategory.Character;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    // ─── Public API ───────────────────────────────────────────

    public void HandleInput()
    {
        // ปุ่ม ESC ไม่ได้จัดการที่นี่ ให้ PauseController เป็นคนตัดสินใจแทน
        if (uiInput.instance.OpenMenuInput) TogglePanel(lastPanel);
        // else if (uiInput.instance.InventoryUiInput) TogglePanel(UiPanelType.Inventory);
        // else if (uiInput.instance.EquipmentUiInput) TogglePanel(UiPanelType.Equipment);
        else if (uiInput.instance.MapUiInput) TogglePanel(UiPanelType.Map);

    }

    public void TogglePanel(UiPanelType panel)
    {
        if (IsPanelOpen && CurrentPanel == panel)
            CloseAll();
        else
            OpenPanel(panel);
    }

    public void SetCurrentPanel(UiPanelType panel)
    {
        CurrentPanel = panel;
        lastPanel = panel;

        if (panel == UiPanelType.Inventory)
            InventoryManager.instance?.DisplayItems();
        else if (panel == UiPanelType.Equipment)
            EquipmentManager.instance?.ShowCategory(CurrentCategory);
    }

    public void SetCurrentCategory(EquipmentCategory category)
    {
        CurrentCategory = category;
        EquipmentManager.instance?.ShowCategory(category);
    }

    public void OpenPanel(UiPanelType panel)
    {
        IsPanelOpen = true;
        uiPanel.SetActive(true);
        Time.timeScale = 0;

        MenuBarController.instance.OpenPage((int)panel);  // จะไปเซ็ต CurrentPanel/lastPanel ให้เอง
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