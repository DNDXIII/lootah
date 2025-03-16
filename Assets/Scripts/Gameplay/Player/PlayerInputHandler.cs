using InputSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float lookSensitivity = 1f;

        private bool _fireInputWasHeld;
        private bool _jumpInputWasHeld;

        private PlayerInputsManager _playerInputsManager;


        private void Start()
        {
            _playerInputsManager = GetComponent<PlayerInputsManager>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            _fireInputWasHeld = GetFireInputHeld();
            _jumpInputWasHeld = GetJumpInputHeld();
        }

        private bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (!CanProcessInput()) return Vector3.zero;

            var moveVector = _playerInputsManager.move;

            Vector3 move = new Vector3(moveVector.x, 0f, moveVector.y);
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        public float GetLookInputsHorizontal()
        {
            if (!CanProcessInput()) return 0;

            var lookX = _playerInputsManager.look.x * lookSensitivity;

            return Gamepad.current != null ? lookX * Time.deltaTime : lookX;
        }

        public float GetLookInputsVertical()
        {
            if (!CanProcessInput()) return 0;

            var lookY = _playerInputsManager.look.y * lookSensitivity;

            return Gamepad.current != null ? lookY * Time.deltaTime : lookY;
        }

        public bool GetJumpInputDown()
        {
            return GetJumpInputHeld() && !_jumpInputWasHeld;
        }

        public bool GetJumpInputHeld()
        {
            return CanProcessInput() && _playerInputsManager.jump;
        }

        public bool GetFireInputDown()
        {
            return GetFireInputHeld() && !_fireInputWasHeld;
        }

        public bool GetFireInputReleased()
        {
            return !GetFireInputHeld() && _fireInputWasHeld;
        }

        public bool GetFireInputHeld()
        {
            return CanProcessInput() && _playerInputsManager.shoot;
        }


        public bool GetSprintInputHeld()
        {
            return CanProcessInput() && _playerInputsManager.sprint;
        }

        public bool GetReloadButtonDown()
        {
            return CanProcessInput() && _playerInputsManager.reload;
        }

        public bool GetSwitchWeaponInput()
        {
            return CanProcessInput() && _playerInputsManager.switchWeapon;
        }

        public bool GetAbilityInput()
        {
            return CanProcessInput() && _playerInputsManager.aim;
        }

        public bool GetInteractInput()
        {
            return CanProcessInput() && _playerInputsManager.interact;
        }
    }
}