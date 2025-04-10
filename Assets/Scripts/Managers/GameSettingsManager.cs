﻿using System;
using Shared;

namespace Managers
{
    public class GameSettingsManager : Singleton<GameSettingsManager>
    {
        public Action<GameDifficulty> OnGameDifficultyChanged;

        private GameDifficulty _gameDifficulty = GameDifficulty.Normal;

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