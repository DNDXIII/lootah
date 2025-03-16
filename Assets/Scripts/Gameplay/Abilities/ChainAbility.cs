using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Shared;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Abilities
{
    public class ChainAbility : MonoBehaviour
    {
        [Header("Lightning Settings")] [Tooltip("Maximum distance the initial zap can reach.")] [SerializeField]
        private float maxDistance = 50f;

        [Tooltip("Maximum range for chain jumps between enemies.")] [SerializeField]
        private float chainRange = 10f;

        [Tooltip("Maximum number of times the lightning can chain to new enemies.")] [SerializeField]
        private int maxChains = 3;

        [Tooltip("Damage dealt per zap.")] [SerializeField]
        private int damage = 25;

        [Header("Cooldown Settings")]
        [Tooltip("Cooldown time in seconds before the ability can be used again.")]
        [SerializeField]
        private float cooldownTime = 2f;

        private float lastAttackTime = -Mathf.Infinity;

        [FormerlySerializedAs("enemyLayer")]
        [Header("Layer Settings")]
        [Tooltip("Layer mask for detecting enemies.")]
        [SerializeField]
        private LayerMask layerMask;

        [Header("Effects")] [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private AudioClip impactSfx;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // TODO: use actual input system
            if (Input.GetMouseButtonDown(1) && Time.time >= lastAttackTime + cooldownTime)
            {
                PerformChainLightning();
                lastAttackTime = Time.time; // Update last attack time
            }
        }

        private void PerformChainLightning()
        {
            var hitBaseHealths = new List<BaseHealth>();
            var hitDamageables = new List<Damageable>();

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, maxDistance,
                    layerMask) && hit.transform.TryGetComponent(out Damageable firstTarget))
            {
                // Actually hit something and that something is an enemy
                hitDamageables.Add(firstTarget);
                hitBaseHealths.Add(firstTarget.Health);
                ChainToNextEnemy(firstTarget.Health, hitBaseHealths, hitDamageables);

                var hitTransforms = hitBaseHealths.Select(health => health.transform.position + Vector3.up).ToList();
                StartCoroutine(ShowBoltEffect(hitTransforms));

                foreach (Damageable enemy in hitDamageables)
                {
                    enemy.TakeDamage(damage, ActorManager.Instance.Player.gameObject);
                }
            }
            else
            {
                // If no enemy was hit, show the bolt effect going into the distance
                StartCoroutine(ShowBoltEffect(new List<Vector3>
                    { _camera.transform.position + _camera.transform.forward * maxDistance }));
            }

            if (impactSfx)
            {
                AudioUtility.CreateSfx(impactSfx, transform.position,
                    AudioUtility.AudioGroups.DamageTick, 0f);
            }
        }

        private void ChainToNextEnemy(BaseHealth currentEnemy, List<BaseHealth> hitEnemies,
            List<Damageable> hitDamageables)
        {
            for (int i = 0; i < maxChains; i++)
            {
                var enemiesInRange = Physics.OverlapSphere(currentEnemy.transform.position, chainRange, layerMask);

                BaseHealth closestEnemy = null;
                Damageable closestDamageable = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider enemy in enemiesInRange)
                {
                    if (!enemy.TryGetComponent(out Damageable damageable)) continue;
                    if (hitEnemies.Contains(damageable.Health)) continue;
                    float distance = Vector3.Distance(currentEnemy.transform.position, enemy.transform.position);
                    if (!(distance < closestDistance)) continue;
                    closestDistance = distance;
                    closestEnemy = damageable.Health;
                    closestDamageable = damageable;
                }

                if (closestEnemy)
                {
                    hitEnemies.Add(closestEnemy);
                    hitDamageables.Add(closestDamageable);
                    currentEnemy = closestEnemy;
                }
                else
                {
                    break;
                }
            }
        }

        private IEnumerator ShowBoltEffect(List<Vector3> positions)
        {
            positions.Insert(0, _camera.transform.position);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(2f); // Bolt duration

            lineRenderer.enabled = false;
        }
    }
}