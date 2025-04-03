using InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryMenu;
        [SerializeField] private GameObject hud;
        [SerializeField] private PauseMenuUiToolkit pauseMenuUiToolkit;

        private PlayerInputsManager _playerInputsManager;
        private bool _isInventoryOpen;
        private bool _isPauseMenuOpen;
        private InputManager _inputManager;


        private void Awake()
        {
            _playerInputsManager = FindFirstObjectByType<PlayerInputsManager>();

            _inputManager = new InputManager();
            _inputManager.UI.Inventory.performed += _ => OnToggleInventory();
            _inputManager.UI.Pause.performed += _ => OnTogglePause();
        }


        private void Start()
        {
            hud.SetActive(true);
            inventoryMenu.SetActive(false);
            pauseMenuUiToolkit.SetActive(false);

            pauseMenuUiToolkit.OnResume += OnTogglePause;
        }

        private void OnTogglePause()
        {
            if (!_isPauseMenuOpen && !_isInventoryOpen)
            {
                Pause();
                _isPauseMenuOpen = true;
                pauseMenuUiToolkit.SetActive(true);
                hud.SetActive(false);
            }

            else if (_isPauseMenuOpen)
            {
                hud.SetActive(true);
                pauseMenuUiToolkit.SetActive(false);
                _isPauseMenuOpen = false;
                Resume();
            }
        }

        private void OnToggleInventory()
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
            // Unregister the event to avoid memory leaks
            pauseMenuUiToolkit.OnResume -= OnTogglePause;
        }
    }
}