using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Managers.EnemySpawnerManager
{
    public class WaveEnemySpawner : MonoBehaviour
    {
        public Action OnWaveEnd;
        private readonly List<EnemySpawner> _enemySpawners = new();
        private int _activeSpawners;
        private int _enemyCount;


        private void Awake()
        {
            // Find all the children of the spawner
            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<EnemySpawner>(out var enemySpawner))
                {
                    _enemySpawners.Add(enemySpawner);
                }
            }
        }

        public int SpawnWave()
        {
            foreach (var enemySpawner in _enemySpawners)
            {
                enemySpawner.SpawnEnemies();
                _activeSpawners++;
                _enemyCount += enemySpawner.EnemyCount;
                enemySpawner.OnEnemiesKilled += OnEnemyKilled;
            }

            return _enemyCount;
        }

        private void OnEnemyKilled()
        {
            _activeSpawners--;
            if (_activeSpawners > 0) return;

            // All enemies from all spawners are dead
            OnWaveEnd?.Invoke();
        }
    }
}