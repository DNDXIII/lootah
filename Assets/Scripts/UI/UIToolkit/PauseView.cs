using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.UIToolkit
{
    public class PauseView : UIView
    {
        private Button _resumeButton;
        private Button _optionsButton;
        private Button _backToBaseButton;
        private Button _quitGameButton;


        public PauseView(VisualElement topElement) : base(topElement)
        {
            base.SetVisualElements();
        }

        protected override void SetVisualElements()
        {
            _resumeButton = TopElement.Q<Button>("ResumeButton");
            _optionsButton = TopElement.Q<Button>("OptionsButton");
            _backToBaseButton = TopElement.Q<Button>("QuitToBaseButton");
            _quitGameButton = TopElement.Q<Button>("ExitGameButton");
        }

        protected override void RegisterButtonCallbacks()
        {
            _resumeButton.RegisterCallback<ClickEvent>(OnResumeButton);
            _optionsButton.RegisterCallback<ClickEvent>(OnOptionsButton);
            _backToBaseButton.RegisterCallback<ClickEvent>(OnBackToBaseButton);
            _quitGameButton.RegisterCallback<ClickEvent>(OnQuitGameButton);
        }

        public override void Dispose()
        {
            base.Dispose();
            UnregisterButtonCallbacks();
        }

        private void UnregisterButtonCallbacks()
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

        private static void OnBackToBaseButton(ClickEvent evt)
        {
            MainMenuUIEvents.ResumeGame?.Invoke();
            SceneLoaderManager.Instance.LoadLevel("BaseScene");
        }

        private static void OnOptionsButton(ClickEvent evt)
        {
            MainMenuUIEvents.SettingsScreenShown?.Invoke();
        }

        private static void OnResumeButton(ClickEvent clickEvent)
        {
            MainMenuUIEvents.ResumeGame?.Invoke();
        }
    }
}