﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Shared;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Managers.EnemySpawnerManager
{
    public class RandomWaveSpawner : MonoBehaviour
    {
        [SerializeField] private float delayAfterWave = 1f;
        [SerializeField] private float delayBeforeFirstWave = 3f;

        [Tooltip("If -1, will spawn waves indefinitely")] [SerializeField]
        private int maxWaves = -1;

        [SerializeField] private UnityEvent onWavesEnd;


        [Header("UI fields")] [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI enemiesLeftText;

        private int _enemyCount;
        private int _waveCount;
        private readonly List<WaveEnemySpawner> _waveEnemySpawners = new();


        private void Awake()
        {
            // Find all the children of the spawner
            for (var i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).TryGetComponent<WaveEnemySpawner>(out var waveSpawner)) continue;
                _waveEnemySpawners.Add(waveSpawner);
                waveSpawner.OnWaveEnd += OnWaveEnd;
            }

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
        }

        private void Start()
        {
            SpawnAfterDelay(delayBeforeFirstWave).Forget();
        }

        private void OnEnemyKilled(EnemyKillEvent obj)
        {
            _enemyCount--;
            enemiesLeftText.text = $"Enemies Left: {_enemyCount}";
        }

        private async UniTaskVoid SpawnAfterDelay(float delay)
        {
            await UniTask.WaitForSeconds(delay);
            SpawnWave().Forget();
        }

        private async UniTaskVoid SpawnWave()
        {
            // find a random wave spawner
            var waveToSpawn = _waveEnemySpawners[Random.Range(0, _waveEnemySpawners.Count)];
            _enemyCount += await waveToSpawn.SpawnWave();
            _waveCount++;

            waveText.text = $"Wave: {_waveCount}";
            enemiesLeftText.text = $"Enemies Left: {_enemyCount}";
        }

        private void OnWaveEnd()
        {
            if (maxWaves != -1 && _waveCount >= maxWaves)
            {
                onWavesEnd.Invoke();
                return;
            }

            SpawnAfterDelay(delayAfterWave).Forget();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
            foreach (var waveEnemySpawner in _waveEnemySpawners)
            {
                waveEnemySpawner.OnWaveEnd -= OnWaveEnd;
            }
        }
    }
}