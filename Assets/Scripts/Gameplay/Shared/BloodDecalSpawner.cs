using UnityEngine;

namespace Gameplay.Shared
{
    [RequireComponent(typeof(Health))]
    public class BloodDecalSpawner : MonoBehaviour
    {
        [Header("Decal Settings")] [SerializeField]
        private GameObject[] bloodDecalPrefabs;

        [SerializeField] private int minDecals = 1;
        [SerializeField] private int maxDecals = 4;
        [SerializeField] private float spawnRadius = 2f;
        [SerializeField] private LayerMask layerMask;

        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnDamaged += SpawnBloodDecals;
        }

        private void SpawnBloodDecals(float damageAmount, GameObject damageSource)
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out var hit, 3f, layerMask)) return;

            int decalCount = Random.Range(minDecals, maxDecals + 1);

            for (int i = 0; i < decalCount; i++)
            {
                // Random offset within a circle
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
                Vector3 spawnPos = hit.point + offset + Vector3.up * 0.01f;

                Quaternion rotation = Quaternion.Euler(90, Random.Range(0f, 360f), 0);

                GameObject decalPrefab = bloodDecalPrefabs[Random.Range(0, bloodDecalPrefabs.Length)];
                Instantiate(decalPrefab, spawnPos, rotation);
            }
        }

        private void OnDisable()
        {
            _health.OnDamaged -= SpawnBloodDecals;
        }
    }
}