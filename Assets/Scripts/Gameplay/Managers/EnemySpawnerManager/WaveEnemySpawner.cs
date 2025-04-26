using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
                if (!transform.GetChild(i).TryGetComponent<EnemySpawner>(out var enemySpawner)) continue;
                _enemySpawners.Add(enemySpawner);
                enemySpawner.OnEnemiesKilled += OnSpawnerEnemiesKilled;
            }
        }

        public async UniTask<int> SpawnWave()
        {
            var spawnTasks = Enumerable.Select(_enemySpawners, SpawnAndRegister).ToList();

            var results = await UniTask.WhenAll(spawnTasks);

            _enemyCount = results.Sum();
            _activeSpawners = _enemySpawners.Count;

            return _enemyCount;
        }

        private static async UniTask<int> SpawnAndRegister(EnemySpawner enemySpawner)
        {
            return await enemySpawner.SpawnEnemies();
        }

        private void OnSpawnerEnemiesKilled()
        {
            _activeSpawners--;
            if (_activeSpawners != 0) return;

            // All enemies from all spawners are dead
            OnWaveEnd?.Invoke();
        }
    }
}