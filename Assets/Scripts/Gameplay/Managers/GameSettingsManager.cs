using System;
using Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Managers
{
    public class GameSettingsManager : Singleton<GameSettingsManager>
    {
        // Defaults
        private const GameDifficulty DefaultGameDifficulty = GameDifficulty.Normal;
        private const float DefaultMouseSensitivity = 1.0f;

        // PlayerPref keys
        private const string GameDifficultyKey = "GameDifficulty";
        private const string MouseSensitivityKey = "MouseSensitivity";


        public Action<GameDifficulty> OnGameDifficultyChanged;
        public Action<float> OnMouseSensitivityChanged;


        public GameDifficulty GetGameDifficulty()
        {
            string savedValue = PlayerPrefs.GetString(GameDifficultyKey, DefaultGameDifficulty.ToString());
            return Enum.TryParse(savedValue, out GameDifficulty difficulty) ? difficulty : DefaultGameDifficulty;
        }

        public void SetGameDifficulty(GameDifficulty value)
        {
            PlayerPrefs.SetString(GameDifficultyKey, value.ToString());
            PlayerPrefs.Save();
            OnGameDifficultyChanged?.Invoke(value);
        }

        public float GetMouseSensitivity()
        {
            return PlayerPrefs.GetFloat(MouseSensitivityKey, DefaultMouseSensitivity);
        }

        public void SetMouseSensitivity(float value)
        {
            PlayerPrefs.SetFloat(MouseSensitivityKey, value);
            PlayerPrefs.Save();
            OnMouseSensitivityChanged?.Invoke(value);
        }
    }
}