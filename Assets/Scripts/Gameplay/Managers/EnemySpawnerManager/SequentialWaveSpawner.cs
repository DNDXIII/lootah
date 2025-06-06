﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Managers.EnemySpawnerManager
{
    public class SequentialWaveSpawner : MonoBehaviour
    {
        [SerializeField] private float delayAfterWave = 1f;

        [SerializeField] private bool startOnAwake;
        [SerializeField] private UnityEvent onWavesEnd;

        private int _currentWave;
        private readonly List<WaveEnemySpawner> _waveEnemySpawners = new();


        private void Awake()
        {
            // Find all the children of the spawner
            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<WaveEnemySpawner>(out var waveSpawner))
                {
                    _waveEnemySpawners.Add(waveSpawner);
                    waveSpawner.OnWaveEnd += OnWaveEnd;
                }
            }
        }

        private void Start()
        {
            if (startOnAwake)
            {
                SpawnWave();
            }
        }

        public void SpawnWave()
        {
            var waveToSpawn = _waveEnemySpawners[_currentWave];
            waveToSpawn.SpawnWave().Forget();
            _currentWave++;
        }

        private void OnWaveEnd()
        {
            if (_currentWave >= _waveEnemySpawners.Count)
            {
                onWavesEnd.Invoke();
                return;
            }

            Invoke(nameof(SpawnWave), delayAfterWave);
        }
    }
}