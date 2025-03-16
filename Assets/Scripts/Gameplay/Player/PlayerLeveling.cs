using Managers;
using Shared;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerLeveling : MonoBehaviour
    {
        private int _currentLevel = 1;
        private int _currentExperience;
        private int _experienceToNextLevel = 100;

        public int CurrentLevel => _currentLevel;
        public float CurrentExperienceRatio => (float)_currentExperience / _experienceToNextLevel;

        private void Start()
        {
            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKillEvent evt)
        {
            AddExperience(evt.Enemy.BaseExperience);
        }

        private void AddExperience(int experience)
        {
            _currentExperience += experience;

            while (_currentExperience >= _experienceToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Debug.Log("Level Up!");
            _currentLevel++;
            _currentExperience -= _experienceToNextLevel;
            _experienceToNextLevel = CalculateExperienceToNextLevel();

            PlayerLevelUpEvent evt = Events.PlayerLevelUpEvent;
            EventManager.Broadcast(evt);
        }

        private int CalculateExperienceToNextLevel()
        {
            return _currentLevel * 100;
        }
        
        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}