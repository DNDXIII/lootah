using System.Collections.Generic;
using Shared;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.Enemy2
{
    public class EnemyFactory : Singleton<EnemyFactory>, IEntityFactory<Enemy, EnemyConfig>
    {
        private readonly Dictionary<EnemyConfig, IObjectPool<Enemy>> _pools = new();


        public Enemy Create(EnemyConfig config, Vector3 position)
        {
            var pool = _pools.TryGetValue(config, out var existingPool)
                ? existingPool
                : _pools[config] = new ObjectPool<Enemy>(() =>
                        Instantiate(config.prefab).GetComponent<Enemy>()
                            .SetActive(),
                    enemy => enemy.SetActive(),
                    enemy => enemy.SetInactive(),
                    enemy =>
                    {
                        if (!enemy) return;
                        Destroy(enemy.gameObject);
                    },
                    true, 10, 50);

            Enemy enemy = pool.Get();
            enemy.Initialize(config, position, pool);
            return enemy;
        }
    }
}