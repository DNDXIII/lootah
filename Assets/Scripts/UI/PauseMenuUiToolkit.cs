using System;
using Managers;
using Shared;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class PauseMenuUiToolkit : MonoBehaviour
    {
        public Action OnResume;

        private UIDocument _document;
        private Button _resumeButton;
        private Button _optionsButton;
        private Button _backToBaseButton;
        private Button _quitGameButton;


        private void Awake()
        {
            _document = GetComponent<UIDocument>();

            _resumeButton = _document.rootVisualElement.Q<Button>("ResumeButton");
            _resumeButton.RegisterCallback<ClickEvent>(OnResumeButton);

            _optionsButton = _document.rootVisualElement.Q<Button>("OptionsButton");
            _optionsButton.RegisterCallback<ClickEvent>(OnOptionsButton);

            _backToBaseButton = _document.rootVisualElement.Q<Button>("QuitToBaseButton");
            _backToBaseButton.RegisterCallback<ClickEvent>(OnBackToBaseButton);

            _quitGameButton = _document.rootVisualElement.Q<Button>("ExitGameButton");
            _quitGameButton.RegisterCallback<ClickEvent>(OnQuitGameButton);
        }


        public void SetActive(bool isActive)
        {
            _document.rootVisualElement.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
        }


        private void OnDisable()
        {
            _resumeButton.UnregisterCallback<ClickEvent>(OnResumeButton);
            _optionsButton.UnregisterCallback<ClickEvent>(OnOptionsButton);
            _backToBaseButton.UnregisterCallback<ClickEvent>(OnBackToBaseButton);
            _quitGameButton.UnregisterCallback<ClickEvent>(OnQuitGameButton);
        }

        private static void OnQuitGameButton(ClickEvent evt)
        {
            Application.Quit();
        }

        private void OnBackToBaseButton(ClickEvent evt)
        {
            OnResume?.Invoke();
            SceneLoaderManager.Instance.LoadLevel("BaseScene");
        }

        private static void OnOptionsButton(ClickEvent evt)
        {
            // get difficulty as int
            var previousDifficulty = GameSettingsManager.Instance.GameDifficulty;
            //cycle to next difficulty
            GameSettingsManager.Instance.GameDifficulty =
                (GameDifficulty)(((int)previousDifficulty + 1) % Enum.GetValues(typeof(GameDifficulty)).Length);
            Debug.Log("new difficulty: " + GameSettingsManager.Instance.GameDifficulty);
        }

        private void OnResumeButton(ClickEvent clickEvent)
        {
            OnResume?.Invoke();
        }
    }
}