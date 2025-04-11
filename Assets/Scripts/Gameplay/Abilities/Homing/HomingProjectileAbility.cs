using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Abilities.Homing
{
    public class HomingProjectileAbility : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private int maxHits = 3;
        [SerializeField] private int damage = 10;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private LayerMask hittableLayers;
        [SerializeField] private float cooldownTime = 2f;

        [Header("Effects")] [Tooltip("Audio clip to play once ability is used.")] [SerializeField]
        private AudioClip abilitySound;

        [Header("References")] [SerializeField]
        private HomingProjectile projectilePrefab;

        [SerializeField] private Transform spawnPoint;


        private Camera _camera;
        private float _lastAttackTime = -Mathf.Infinity;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // TODO: use actual input system
            if (!Input.GetMouseButtonDown(1) || !(Time.time >= _lastAttackTime + cooldownTime)) return;

            FireProjectile();
            _lastAttackTime = Time.time;
        }

        private void FireProjectile()
        {
            if (!_camera) return;

            Transform firstTarget = null;

            // Try to find an enemy in front of the camera, to set as the first target
            if (Physics.SphereCast(_camera.transform.position, 3f, _camera.transform.forward,
                    out RaycastHit hit, maxDistance, hittableLayers))
            {
                if (hit.transform.TryGetComponent(out Damageable damageable))
                {
                    if (!damageable.Health.TryGetComponent<Enemy2.Enemy>(out var enemy))
                    {
                        Debug.LogError("Enemy component not found on damageable object");
                    }

                    firstTarget = enemy.Center.transform;
                }
            }

            var projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            projectile.Initialize(projectileSpeed, maxHits, damage, firstTarget);

            if (abilitySound)
            {
                AudioUtility.CreateSfx(abilitySound, transform.position,
                    AudioUtility.AudioGroups.DamageTick);
            }
        }
    }
}