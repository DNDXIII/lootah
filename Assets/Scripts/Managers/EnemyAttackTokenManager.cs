using System;
using Shared;
using UnityEngine;

namespace Managers
{
    public class EnemyAttackTokenManager : Singleton<EnemyAttackTokenManager>
    {
        [SerializeField] private DifficultySettings<int> lightAttackTickets = new(1, 2, 3, 4);
        [SerializeField] private DifficultySettings<int> heavyAttackTickets = new(1, 1, 2, 3);

        // Doing it like this means no need to restart the game to change difficulty
        private int _maxLightAttackTickets;
        private int _maxHeavyAttackTickets;
        private int _currentLightAttackTickets;
        private int _currentHeavyAttackTickets;


        private void Start()
        {
            InitializeTickets();

            GameSettingsManager.Instance.OnGameDifficultyChanged += OnGameDifficultyChanged;
        }


        private void OnGameDifficultyChanged(GameDifficulty newDifficulty)
        {
            InitializeTickets();
        }

        private void InitializeTickets()
        {
            _maxLightAttackTickets = lightAttackTickets.GetValue(GameSettingsManager.Instance.GameDifficulty);
            _maxHeavyAttackTickets = heavyAttackTickets.GetValue(GameSettingsManager.Instance.GameDifficulty);
            _currentLightAttackTickets = _maxLightAttackTickets;
            _currentHeavyAttackTickets = _maxHeavyAttackTickets;
        }

        public bool RequestToken(EnemyAttackToken token)
        {
            return token switch
            {
                EnemyAttackToken.Light => RequestLightAttackTicket(),
                EnemyAttackToken.Heavy => RequestHeavyAttackTicket(),
                _ => throw new ArgumentOutOfRangeException(nameof(token), token, null)
            };
        }

        public void ReleaseToken(EnemyAttackToken token)
        {
            switch (token)
            {
                case EnemyAttackToken.Light:
                    ReturnLightAttackTicket();
                    break;
                case EnemyAttackToken.Heavy:
                    ReturnHeavyAttackTicket();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(token), token, null);
            }
        }

        private bool RequestLightAttackTicket()
        {
            if (_currentLightAttackTickets <= 0) return false;

            _currentLightAttackTickets--;
            return true;
        }

        private bool RequestHeavyAttackTicket()
        {
            if (_currentHeavyAttackTickets <= 0) return false;

            _currentHeavyAttackTickets--;
            return true;
        }

        private void ReturnLightAttackTicket()
        {
            _currentLightAttackTickets = Math.Min(_currentLightAttackTickets + 1, _maxLightAttackTickets);
        }

        private void ReturnHeavyAttackTicket()
        {
            _currentHeavyAttackTickets = Math.Min(_currentHeavyAttackTickets + 1, _maxHeavyAttackTickets);
        }

        private void OnDisable()
        {
            GameSettingsManager.Instance.OnGameDifficultyChanged -= OnGameDifficultyChanged;
        }
    }
}