using System.Collections.Generic;
using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Abilities.Homing
{
    public class HomingProjectile : MonoBehaviour
    {
        [Header("Effects")] [Tooltip("Audio clip to play on impact.")] [SerializeField]
        private AudioClip impactSound;

        [Tooltip("Particle system to play on impact.")] [SerializeField]
        private ParticleSystem impactEffect;

        [Header("Targeting")] [Tooltip("Layer mask for detecting enemies.")] [SerializeField]
        private LayerMask hittableLayers;

        [Tooltip("The maximum distance the projectile can travel.")] [SerializeField]
        private float maxDistance = 50f;

        [Header("Collision detection")] [Tooltip("Radius of this projectile's collision detection")]
        public float radius = 0.01f;

        [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
        public Transform root;

        [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
        public Transform tip;


        private float _speed = 10f;
        private int _damage = 10;
        private int _maxHits = 3;
        private int _currentHits;
        private Transform _currentTarget;
        private readonly List<Collider> _ignoredColliders = new();
        private Vector3 _lastRootPosition;


        public void Initialize(float speed, int maxHits, int damage, Transform initialTarget)
        {
            _speed = speed;
            _maxHits = maxHits;
            _damage = damage;
            _currentTarget = initialTarget;
        }

        private void LateUpdate()
        {
            // Update the projectile's position and rotation
            if (!_currentTarget) return;

            // TODO: Need to find the target center somehow, probably need an Enemy class with a center property
            Vector3 direction = ((_currentTarget.position + Vector3.up) - transform.position).normalized;
            transform.position += direction * (_speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void FixedUpdate()
        {
            HitDetection();
        }

        private void HitDetection()
        {
            RaycastHit closestHit = new RaycastHit
            {
                distance = Mathf.Infinity
            };
            bool foundHit = false;

            // Sphere cast
            Vector3 displacementSinceLastFrame = tip.position - _lastRootPosition;
            var hits = Physics.SphereCastAll(_lastRootPosition, radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hittableLayers,
                QueryTriggerInteraction.Collide);
            foreach (var hit in hits)
            {
                if (!IsHitValid(hit) || !(hit.distance < closestHit.distance)) continue;
                foundHit = true;
                closestHit = hit;
            }

            if (foundHit)
            {
                // Handle case of casting while already inside a collider
                if (closestHit.distance <= 0f)
                {
                    closestHit.point = root.position;
                    closestHit.normal = -transform.forward;
                }

                OnHit(closestHit.point, closestHit.normal, closestHit.collider);
            }

            _lastRootPosition = root.position;
        }

        private void OnHit(Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider)
        {
            PlayHitEffects(hitPoint, hitNormal);

            if (!hitCollider.TryGetComponent(out Damageable damageable))
            {
                // We probably hit a wall or environment, destroy the projectile
                Destroy(gameObject);
                return;
            }

            // We hit a damageable object, apply damage
            damageable.TakeDamage(_damage, gameObject, false);
            // ignore the collider of the damageable object, so that we don't hit it again
            var colliders = damageable.Health.GetComponentsInChildren<Collider>();
            _ignoredColliders.AddRange(colliders);

            _currentHits += 1;
            if (_currentHits >= _maxHits)
            {
                Destroy(gameObject);
                return;
            }

            FindNextTarget(hitPoint);
        }

        private void FindNextTarget(Vector3 hitPoint)
        {
            var colliders = Physics.OverlapSphere(hitPoint, maxDistance, hittableLayers);
            Collider closestCollider = null;
            float closestDistance = Mathf.Infinity;
            foreach (var coll in colliders)
            {
                if (!coll.TryGetComponent(out Damageable _) || _ignoredColliders.Contains(coll)) continue;

                float distance = Vector3.Distance(hitPoint, coll.transform.position);
                if (!(distance < closestDistance)) continue;
                closestDistance = distance;
                closestCollider = coll;
            }

            if (closestCollider)
            {
                _currentTarget = closestCollider.transform;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void PlayHitEffects(Vector3 hitPoint, Vector3 hitNormal)
        {
            // impact vfx
            if (impactEffect)
            {
                Instantiate(impactEffect, hitPoint + hitNormal, Quaternion.LookRotation(hitNormal));
            }

            // impact sfx
            if (impactSound)
            {
                AudioUtility.CreateSfx(impactSound, hitPoint, AudioUtility.AudioGroups.EnemyAttack, 1f);
            }
        }


        private bool IsHitValid(RaycastHit hit)
        {
            // ignore hits with triggers that don't have a Damageable component
            if (hit.collider.isTrigger && !hit.collider.TryGetComponent<Damageable>(out _))
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            return _ignoredColliders == null || !_ignoredColliders.Contains(hit.collider);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}