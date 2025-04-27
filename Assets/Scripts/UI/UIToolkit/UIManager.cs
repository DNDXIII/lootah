using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.UIToolkit
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        private UIDocument _mainMenuDocument;

        private UIView _currentView;
        private UIView _previousView;

        // List of all UIViews
        private readonly List<UIView> _allViews = new();

        private UIView _pauseView; // Landing screen
        private UIView _settingsView; // Overlay screen for settings

        // VisualTree string IDs for UIViews; each represents one branch of the tree
        private const string PauseViewName = "PauseScreen";
        private const string SettingsViewName = "SettingsScreen";

        private void OnEnable()
        {
            _mainMenuDocument = GetComponent<UIDocument>();

            SetupViews();

            SubscribeToEvents();

            // Start with the home screen
            ShowModalView(_pauseView);
        }

        private void SetupViews()
        {
            VisualElement root = _mainMenuDocument.rootVisualElement;

            _pauseView = new PauseView(root.Q<VisualElement>(PauseViewName));
            _settingsView = new SettingsView(root.Q<VisualElement>(SettingsViewName));

            // Track UI Views in a List for disposal 
            _allViews.Add(_pauseView);
            _allViews.Add(_settingsView);

            // UI Views enabled by default
            _pauseView.Show();
        }

        private void ShowModalView(UIView newView)
        {
            _currentView?.Hide();

            _previousView = _currentView;
            _currentView = newView;

            _currentView?.Show();
        }

        private void SubscribeToEvents()
        {
            MainMenuUIEvents.SettingsScreenShown += OnSettingsScreenShown;
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();

            foreach (UIView view in _allViews)
            {
                view.Dispose();
            }
        }

        private void UnsubscribeFromEvents()
        {
            MainMenuUIEvents.SettingsScreenShown -= OnSettingsScreenShown;
            MainMenuUIEvents.SettingsScreenHidden -= OnSettingsScreenHidden;
        }


        private void OnSettingsScreenShown()
        {
            ShowModalView(_settingsView);
        }

        private void OnSettingsScreenHidden()
        {
            _settingsView.Hide();

            if (_previousView == null) return;
            _previousView.Show();
            _currentView = _previousView;
        }
    }
}