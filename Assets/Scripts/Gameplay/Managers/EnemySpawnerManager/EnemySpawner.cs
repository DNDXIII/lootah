using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Enemy2;
using Gameplay.Shared;
using Managers;
using Shared;
using Shared.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Gameplay.Managers.EnemySpawnerManager
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private ParticleSystem spawnEffect;
        [SerializeField] private AudioClip spawnSfx;

        [Header("Spawn Settings")] [Tooltip("Time to wait before spawning the enemies")] [SerializeField]
        private float spawnDelay;

        [Tooltip("Should the enemies be spawned on start?")] [SerializeField]
        private bool spawnOnStart;

        [Tooltip("Time to wait between each enemy spawn")] [SerializeField]
        private float delayBetweenSpawn = .1f;

        [Header("Events")] [Tooltip("Event invoked when all enemies are dead")] [SerializeField]
        private UnityEvent onEnemiesDead;

        [Tooltip("If the object should be destroyed when all enemies are dead")] [SerializeField]
        private bool destroyOnDeath = true;

        private Transform[] _spawnPoints;

        public Action OnEnemiesKilled;

        public int EnemyCount { get; private set; }

        private void Awake()
        {
            // find all the children of the spawner
            _spawnPoints = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
            {
                _spawnPoints[i] = transform.GetChild(i);
            }
        }

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnEnemies().Forget();
            }
        }

        public void FireAndForgetSpawnEnemies()
        {
            SpawnEnemies().Forget();
        }


        public async UniTask<int> SpawnEnemies()
        {
            Preconditions.CheckNotNull(enemyConfig);

            // Sleep for a bit before spawning the enemies
            await UniTask.WaitForSeconds(spawnDelay);

            foreach (var point in _spawnPoints)
            {
                var enemy = EnemyFactory.Instance.Create(enemyConfig, point.position);
                EnemyCount += 1;
                enemy.OnDie += OnEnemyKilled;

                if (spawnEffect)
                {
                    // spawn slightly above the ground
                    var spawnPosition = point.position + Vector3.up;
                    Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
                }

                if (spawnSfx)
                {
                    AudioUtility.CreateSfx(spawnSfx, transform.position,
                        AudioUtility.AudioGroups.EnemySpawn,
                        1f);
                }

                await UniTask.WaitForSeconds(delayBetweenSpawn);
            }

            return EnemyCount;
        }

        private void OnEnemyKilled(Enemy2.Enemy enemy)
        {
            EnemyCount--;
            enemy.OnDie -= OnEnemyKilled;

            if (EnemyCount > 0) return;
            OnEnemiesKilled?.Invoke();
            onEnemiesDead?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}