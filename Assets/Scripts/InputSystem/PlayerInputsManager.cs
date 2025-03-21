using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class PlayerInputsManager : MonoBehaviour
    {
        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool shoot;
        public bool aim;
        public bool reload;
        public bool switchWeapon;
        public bool interact;
        public bool inventory;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }

        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }

        public void OnSwitchWeapon(InputValue value)
        {
            SwitchWeaponInput(value.isPressed);
        }

        public void OnInventory(InputValue value)
        {
            InventoryInput(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }


#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void ReloadInput(bool valueIsPressed)
        {
            reload = valueIsPressed;
        }

        private void SwitchWeaponInput(bool valueIsPressed)
        {
            switchWeapon = valueIsPressed;
        }

        private void InventoryInput(bool valueIsPressed)
        {
            inventory = valueIsPressed;
        }

        private void InteractInput(bool valueIsPressed)
        {
            interact = valueIsPressed;
        }

        private static void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void SetMouseLock(bool newState)
        {
            cursorLocked = newState;
            cursorInputForLook = newState;
            Cursor.visible = !newState;
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}