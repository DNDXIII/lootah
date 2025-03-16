using Managers;
using Shared;
using UnityEngine;

namespace Gameplay.Abilities.SpawnFly
{
    [CreateAssetMenu(menuName = "Abilities/SpawnOnEnemyDeath")]
    public class SpawnFlyPassiveAbility : AbstractPassiveAbility
    {
        [SerializeField] private GameObject flyPrefab;
        [SerializeField] private float spawnChance = 0.5f;

        private void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (evt.Enemy && Random.value < spawnChance)
            {
                Instantiate(flyPrefab, evt.Enemy.transform.position + Vector3.up, Quaternion.identity);
            }
        }


        public override void Activate()
        {
            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
        }

        public override void Deactivate()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }

        // Just in case
        private void OnDisable()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}