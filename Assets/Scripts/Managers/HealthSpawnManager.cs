using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    // TODO: This whole thing should be refactored, probably a script that goes in the enemies
    public class HealthSpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject healthPickupPrefab;
        [SerializeField] private int minHealthPickupAmount = 1;
        [SerializeField] private int maxHealthPickupAmount = 3;
        [SerializeField] private float spawnRandomDirectionForce = 5f;
        [SerializeField] private float spawnUpwardForce = 10f;

        private void Start()
        {
            EventManager.AddListener<EnemyKillEvent>(SpawnHealthPickups);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(SpawnHealthPickups);
        }

        private void SpawnHealthPickups(EnemyKillEvent obj)
        {
            var enemyPosition = obj.Enemy.transform.position;
            var healthPickupAmount = Random.Range(minHealthPickupAmount, maxHealthPickupAmount + 1);

            for (int i = 0; i < healthPickupAmount; i++)
            {
                var pickup = Instantiate(healthPickupPrefab, enemyPosition, Quaternion.identity);

                // Add a bit of upward and random force to the pickup
                if (!pickup.TryGetComponent<Rigidbody>(out var rb))
                {
                    throw new System.Exception("Health pickup prefab does not have a Rigidbody component");
                }

                rb.AddForce(Vector3.up * spawnUpwardForce + Random.insideUnitSphere * spawnRandomDirectionForce,
                    ForceMode.Impulse);
            }
        }
    }
}