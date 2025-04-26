using System;
using Shared;
using Sirenix.OdinInspector;

namespace Gameplay.Managers
{
    public class GameSettingsManager : Singleton<GameSettingsManager>
    {
        public Action<GameDifficulty> OnGameDifficultyChanged;

        private GameDifficulty _gameDifficulty = GameDifficulty.Normal;

        [ShowInInspector]
        public GameDifficulty GameDifficulty
        {
            get => _gameDifficulty;
            set
            {
                _gameDifficulty = value;
                OnGameDifficultyChanged?.Invoke(value);
            }
        }
    }
}