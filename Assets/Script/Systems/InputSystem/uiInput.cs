using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Input
{
public class uiInput : MonoBehaviour
{
    public static uiInput instance;
    public InputActionAsset inputSystem;
    public bool EscapeInput;
    public bool InventoryUiInput;
    public bool EquipmentUiInput;
    public bool skillUiInput;
    public bool CodexUiInput;
    public bool MapUiInput;
    public bool TasksUiInput;
    public bool LeftClickInput;
    public bool RightClickInput;
    public bool NextInput;
    public bool PreviousInput;
    private InputAction _escapeAction;
    private InputAction _inventoryUiAction;
    private InputAction _equipmentUiAction;
    private InputAction _skillUiAction;
    private InputAction _codexUiAction;
    private InputAction _mapUiAction;
    private InputAction _tasksUiAction;
    private InputAction _LeftClickAction;
    private InputAction _RightClickAction;
    private InputAction _NextAction;
    private InputAction _PreviousAction;

    public void Awake()
    {
        instance = this;
        var uiInput = inputSystem.FindActionMap("UI");
        setupUiInputSystem(uiInput);
    }
    public void Update()
    {
        updateUiInputSystem();
    }
    public void setupUiInputSystem(InputActionMap ui)
    {
        _escapeAction = ui.FindAction("ESC");
        _inventoryUiAction = ui.FindAction("Inventory");
        _equipmentUiAction = ui.FindAction("Equipment");
        _mapUiAction = ui.FindAction("Map");
        _tasksUiAction = ui.FindAction("Tasks");
        _LeftClickAction = ui.FindAction("Click");
        _RightClickAction = ui.FindAction("RightClick");
        _NextAction = ui.FindAction("Next");
        _PreviousAction = ui.FindAction("Previous");
        _skillUiAction = ui.FindAction("Skill");
        _codexUiAction = ui.FindAction("Codex");
    }
    public void updateUiInputSystem()
    {
        EscapeInput = _escapeAction.WasPressedThisFrame();
        InventoryUiInput = _inventoryUiAction.WasPressedThisFrame();
        EquipmentUiInput = _equipmentUiAction.WasPressedThisFrame();
        MapUiInput = _mapUiAction.WasPressedThisFrame();
        TasksUiInput = _tasksUiAction.WasPressedThisFrame();
        LeftClickInput = _LeftClickAction.WasPressedThisFrame();
        RightClickInput = _RightClickAction.WasPressedThisFrame();
        NextInput = _NextAction.WasPressedThisFrame();
        PreviousInput = _PreviousAction.WasPressedThisFrame();
        skillUiInput = _skillUiAction.WasPressedThisFrame();
        CodexUiInput = _codexUiAction.WasPressedThisFrame();
    }

    private void OnEnable() => inputSystem.Enable();
    private void OnDisable() => inputSystem.Disable();

}
}
