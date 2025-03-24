using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Managers.EnemySpawnerManager
{
    public class SequentialWaveSpawner : MonoBehaviour
    {
        [SerializeField] private float delayAfterWave = 1f;

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

        public void SpawnWave()
        {
            // find a random wave spawner
            var waveToSpawn = _waveEnemySpawners[_currentWave];
            waveToSpawn.SpawnWave();
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