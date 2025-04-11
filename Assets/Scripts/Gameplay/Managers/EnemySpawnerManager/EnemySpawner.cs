using System.Collections;
using System.Collections.Generic;
using Managers;
using Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Managers.EnemySpawnerManager
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy2.Enemy enemyPrefab;
        [SerializeField] private ParticleSystem spawnEffect;
        [SerializeField] private AudioClip spawnSfx;

        [Header("Spawn Settings")] [Tooltip("Time to wait before spawning the enemies")] [SerializeField]
        private float spawnDelay;

        [Tooltip("Should the enemies be spawned on start?")] [SerializeField]
        private bool spawnOnStart = false;

        [Tooltip("Time to wait between each enemy spawn")] [SerializeField]
        private float delayBetweenSpawn = .1f;

        [Header("Events")] [Tooltip("Event invoked when all enemies are dead")] [SerializeField]
        private UnityEvent onEnemiesDead;

        [Tooltip("If the object should be destroyed when all enemies are dead")] [SerializeField]
        private bool destroyOnDeath = true;

        private readonly List<Enemy2.Enemy> _enemies = new();
        private Transform[] _spawnPoints;

        // Kinda shit, but it works immediately after they are spawned.
        //  If we used the count they might not have been all spawned yet
        public int EnemyCount => _spawnPoints.Length;


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
                SpawnEnemies();
            }
        }


        public void SpawnEnemies()
        {
            StartCoroutine(SpawnWithDelay());
        }


        private IEnumerator SpawnWithDelay()
        {
            // Sleep for a bit before spawning the enemies

            yield return new WaitForSeconds(spawnDelay);

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);

            foreach (var point in _spawnPoints)
            {
                var enemy = Instantiate(enemyPrefab, point.position, Quaternion.identity);
                _enemies.Add(enemy);

                if (!spawnEffect) continue;
                // spawn slightly above the ground
                var spawnPosition = point.position + Vector3.up;
                Instantiate(spawnEffect, spawnPosition, Quaternion.identity);

                if (spawnSfx)
                {
                    AudioUtility.CreateSfx(spawnSfx, transform.position,
                        AudioUtility.AudioGroups.EnemySpawn,
                        1f);
                }

                yield return new WaitForSeconds(delayBetweenSpawn);
            }
        }

        private void OnEnemyKilled(EnemyKillEvent obj)
        {
            var enemyController = obj.Enemy.GetComponent<Enemy2.Enemy>();
            _enemies.Remove(enemyController);

            if (_enemies.Count != 0) return;
            onEnemiesDead.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}