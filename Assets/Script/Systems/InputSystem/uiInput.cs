using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Input
{
public class uiInput : MonoBehaviour
{
    public static uiInput instance;
    public InputActionAsset inputSystem;
    public bool EscapeInput;
    public bool EquipmentUiInput;
    public bool MapUiInput;
    public bool TasksUiInput;
    public bool LeftClickInput;
    public bool RightClickInput;
    private InputAction _escapeAction;
    private InputAction _equipmentUiAction;
    private InputAction _mapUiAction;
    private InputAction _tasksUiAction;
    private InputAction _LeftClickAction;
    private InputAction _RightClickAction;
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
        _equipmentUiAction = ui.FindAction("Equipment");
        _mapUiAction = ui.FindAction("Map");
        _tasksUiAction = ui.FindAction("Tasks");
        _LeftClickAction = ui.FindAction("Click");
        _RightClickAction = ui.FindAction("RightClick");
    }
    public void updateUiInputSystem()
    {
        EscapeInput = _escapeAction.WasPressedThisFrame();
        EquipmentUiInput = _equipmentUiAction.WasPressedThisFrame();
        MapUiInput = _mapUiAction.WasPressedThisFrame();
        TasksUiInput = _tasksUiAction.WasPressedThisFrame();
        LeftClickInput = _LeftClickAction.WasPressedThisFrame();
        RightClickInput = _RightClickAction.WasPressedThisFrame();
    }

    private void OnEnable() => inputSystem.Enable();
    private void OnDisable() => inputSystem.Disable();

}
}
