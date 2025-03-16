using InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryMenu;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject pauseMenu;

        private PlayerInputsManager _playerInputsManager;
        private bool _isInventoryOpen;
        private bool _isPauseMenuOpen;
        private InputManager _inputManager;


        private void Awake()
        {
            _playerInputsManager = FindFirstObjectByType<PlayerInputsManager>();

            _inputManager = new InputManager();
            _inputManager.UI.Inventory.performed += OnInventory;
            _inputManager.UI.Pause.performed += OnPause;
        }


        private void Start()
        {
            hud.SetActive(true);
            inventoryMenu.SetActive(false);
            pauseMenu.SetActive(false);
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            if (!_isPauseMenuOpen && !_isInventoryOpen)
            {
                Pause();
                _isPauseMenuOpen = true;
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                hud.SetActive(!pauseMenu.activeSelf);
            }

            else if (_isPauseMenuOpen)
            {
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                hud.SetActive(!pauseMenu.activeSelf);
                _isPauseMenuOpen = false;
                Resume();
            }
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            if (!_isInventoryOpen && !_isPauseMenuOpen)
            {
                Pause();
                _isInventoryOpen = true;
                inventoryMenu.SetActive(!inventoryMenu.activeSelf);
                hud.SetActive(!inventoryMenu.activeSelf);
            }

            else if (_isInventoryOpen)
            {
                inventoryMenu.SetActive(!inventoryMenu.activeSelf);
                hud.SetActive(!inventoryMenu.activeSelf);
                _isInventoryOpen = false;
                Resume();
            }
        }

        private void Resume()
        {
            Time.timeScale = 1f;
            _playerInputsManager.SetMouseLock(true);
            // Always close the tooltip when resuming the game
            TooltipManager.Instance.HideTooltip();
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void Pause()
        {
            Time.timeScale = 0f;
            _playerInputsManager.SetMouseLock(false);

            // EventSystem.current.SetSelectedGameObject(mainMenuFirst);
        }


        private void OnEnable()
        {
            // Enable the input actions
            _inputManager.UI.Enable();
        }

        private void OnDisable()
        {
            // Disable the input actions to avoid issues when the object is inactive
            _inputManager.UI.Disable();
        }
    }
}