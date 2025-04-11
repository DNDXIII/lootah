using System;
using System.Collections.Generic;
using Gameplay.Managers.EnemySpawnerManager;
using Shared;
using UnityEngine;

namespace Managers.EnemySpawnerManager
{
    public class WaveEnemySpawner : MonoBehaviour
    {
        public Action OnWaveEnd;
        private readonly List<EnemySpawner> _enemySpawners = new();
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
            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);

            foreach (var enemySpawner in _enemySpawners)
            {
                 enemySpawner.SpawnEnemies();
                _enemyCount += enemySpawner.EnemyCount;
            }

            return _enemyCount;
        }

        private void OnEnemyKilled(EnemyKillEvent obj)
        {
            _enemyCount--;
            if (_enemyCount > 0) return;
            // Wave is done
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
            OnWaveEnd?.Invoke();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}