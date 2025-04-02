using Gameplay.Shared;
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

        [Header("References")] [SerializeField]
        private HomingProjectile projectilePrefab;

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

            // Check if there is a target in front of the camera
            if (!Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, maxDistance,
                    hittableLayers) || !hit.transform.TryGetComponent(out Damageable firstTarget)) return;

            var projectile = Instantiate(projectilePrefab, _camera.transform.position,
                Quaternion.LookRotation(_camera.transform.forward));
            projectile.Initialize(projectileSpeed, maxHits, damage, firstTarget.transform);
        }
    }
}