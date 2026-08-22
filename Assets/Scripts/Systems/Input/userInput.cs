using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class userInput : MonoBehaviour
    {
        public static userInput instance;
        public InputActionAsset inputSystem;
        public Vector2 moveInput; 
        public Vector2 lookInput;
        public bool attackLeftInput;
        public bool attackRightInput;
        public bool interactInput;
        public bool jumpInput;
        public bool sprintInput;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _attackLeftAction;
        private InputAction _attackRightAction;
        private InputAction _interactAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;


        public void Awake()
        {
            instance = this;
            var playerInput = inputSystem.FindActionMap("Player");
            var uiInput = inputSystem.FindActionMap("UI");
            setupInputSystem(playerInput);
        }
        public void Update() => updateInputSystem();
        public void setupInputSystem(InputActionMap player)
        {
            _moveAction = player.FindAction("Move");
            _lookAction = player.FindAction("Look");
            _attackLeftAction = player.FindAction("AttackLeft");
            _attackRightAction = player.FindAction("AttackRight");
            _interactAction = player.FindAction("Interact");
            _jumpAction = player.FindAction("Jump");
            _sprintAction = player.FindAction("Sprint");
        }
        public void updateInputSystem()
        {
            moveInput = _moveAction.ReadValue<Vector2>();
            lookInput = _lookAction.ReadValue<Vector2>();
            attackLeftInput = _attackLeftAction.WasPressedThisFrame();
            attackRightInput = _attackRightAction.WasPressedThisFrame();
            interactInput = _interactAction.WasReleasedThisFrame();
            jumpInput = _jumpAction.WasReleasedThisFrame();
            sprintInput = _sprintAction.WasPressedThisFrame();
        }
        private void OnEnable() => inputSystem.Enable();
        private void OnDisable() => inputSystem.Disable();
    }
}

