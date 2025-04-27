using System.Collections.Generic;
using System.Linq;
using Gameplay.Managers;
using Shared;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.UIToolkit
{
    public class SettingsView : UIView
    {
        private Slider _mouseSensitivitySlider;
        private DropdownField _gameDifficultyDropdown;
        private DropdownField _resolutionDropdown;

        public SettingsView(VisualElement topElement) : base(topElement)
        {
            base.SetVisualElements();
        }

        protected override void SetVisualElements()
        {
            _mouseSensitivitySlider = TopElement.Q<Slider>("MouseSensitivitySlider");
            _mouseSensitivitySlider.value = GameSettingsManager.Instance.GetMouseSensitivity();

            _gameDifficultyDropdown = TopElement.Q<DropdownField>("GameDifficultyDropdown");
            _gameDifficultyDropdown.choices = new List<string>(System.Enum.GetNames(typeof(GameDifficulty)));
            _gameDifficultyDropdown.value = GameSettingsManager.Instance.GetGameDifficulty().ToString();

            _resolutionDropdown = TopElement.Q<DropdownField>("ResolutionDropdown");
            var resolutionOptions = Screen.resolutions
                .Select(res => $"{res.width}x{res.height} {res.refreshRateRatio} Hz")
                .ToList();
            _resolutionDropdown.choices = resolutionOptions;
            Resolution currentRes = Screen.currentResolution;
            string currentResString = $"{currentRes.width}x{currentRes.height} {currentRes.refreshRateRatio} Hz";
            _resolutionDropdown.value =
                resolutionOptions.Contains(currentResString) ? currentResString : resolutionOptions[0];
        }


        protected override void RegisterButtonCallbacks()
        {
            _mouseSensitivitySlider.RegisterValueChangedCallback(OnMouseSensitivityChanged);
            _gameDifficultyDropdown.RegisterValueChangedCallback(OnDifficultyChanged);
            _resolutionDropdown.UnregisterValueChangedCallback(OnResolutionChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            UnregisterButtonCallbacks();
        }

        private void UnregisterButtonCallbacks()
        {
            _mouseSensitivitySlider.UnregisterValueChangedCallback(OnMouseSensitivityChanged);
            _gameDifficultyDropdown.UnregisterValueChangedCallback(OnDifficultyChanged);
            _resolutionDropdown.RegisterValueChangedCallback(OnResolutionChanged);
        }

        private static void OnDifficultyChanged(ChangeEvent<string> evt)
        {
            if (!System.Enum.TryParse(evt.newValue, out GameDifficulty selectedDifficulty)) return;
            GameSettingsManager.Instance.SetGameDifficulty(selectedDifficulty);
        }

        private static void OnMouseSensitivityChanged(ChangeEvent<float> evt)
        {
            GameSettingsManager.Instance.SetMouseSensitivity(evt.newValue);
        }

        private static void OnResolutionChanged(ChangeEvent<string> evt)
        {
            string[] parts = evt.newValue.Split(' ')[0].Split('x');
            if (parts.Length != 2) return;

            if (!int.TryParse(parts[0], out int width) || !int.TryParse(parts[1], out int height)) return;
            Screen.SetResolution(width, height, Screen.fullScreenMode);
            Debug.Log($"Resolution changed to: {width}x{height}");
        }
    }
}