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

        [Tooltip("Rotation speed of the projectile.")] [SerializeField]
        private float initialRotationSpeed = 10f;

        [Tooltip("How much the rotation speed increases per second")] [SerializeField]
        private float rotationAcceleration = 0.5f;

        [Tooltip("Maximum rotation speed of the projectile.")] [SerializeField]
        private float maxRotationSpeed = 20f;

        [Tooltip("Max vertical offset of the projectile when it finds a new target")] [SerializeField]
        private float maxVerticalOffset = 2f;

        [Tooltip("Vertical offset falloff speed of the projectile.")] [SerializeField]
        private float verticalOffsetFalloff = 0.5f;

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
        private float _currentVerticalOffset;
        private float _currentRotationSpeed;


        public void Initialize(float speed, int maxHits, int damage, Transform firstTarget = null)
        {
            _speed = speed;
            _maxHits = maxHits;
            _damage = damage;
            _currentTarget = firstTarget;
        }


        private void LateUpdate()
        {
            if (!_currentTarget)
            {
                MoveWithoutTarget();
                return;
            }

            MoveTowardsTarget();
        }

        private void MoveWithoutTarget()
        {
            // Try finding a close by target
            if (TryFindNextTarget(out var nextTarget))
            {
                SetNextTarget(nextTarget);
                return;
            }

            // just move forward if we don't have a target, and we can't find one
            transform.position += transform.forward * (_speed * Time.deltaTime);
        }

        private void SetNextTarget(Transform nextTarget)
        {
            _currentTarget = nextTarget;
            // Adds a vertical offset to the projectile's position so its movement is more dynamic
            _currentVerticalOffset = Random.Range(-maxVerticalOffset, maxVerticalOffset);
            // Reset the rotation speed to the initial value
            _currentRotationSpeed = initialRotationSpeed;
        }


        private void MoveTowardsTarget()
        {
            // Make the vertical offset go to zero over time
            _currentVerticalOffset = Mathf.MoveTowards(_currentVerticalOffset, 0f,
                verticalOffsetFalloff * Time.deltaTime);

            Vector3 targetPosition =
                (_currentTarget.position + Vector3.up * 1.5f) + (Vector3.up * _currentVerticalOffset);
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Gradually increase the rotation speed over time
            _currentRotationSpeed = Mathf.Min(_currentRotationSpeed + rotationAcceleration * Time.deltaTime,
                maxRotationSpeed);

            // Smoothly rotate towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, _currentRotationSpeed * Time.deltaTime);

            // Move forward in the current facing direction
            transform.position += transform.forward * (_speed * Time.deltaTime);
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
                Debug.Log($"Hit {closestHit.collider.name} at {closestHit.point}");
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
            if (!hitCollider.TryGetComponent(out Damageable damageable))
            {
                // We probably hit a wall or environment.

                // If we have a target, we want to ignore this hit so the projectile can keep going
                if (_currentTarget)
                {
                    return;
                }

                // If we don't have a target, we want to destroy the projectile, since it has no target to hit
                Destroy(gameObject);
                return;
            }

            HandleHitDamageable(hitPoint, hitNormal, damageable);
        }

        private void HandleHitDamageable(Vector3 hitPoint, Vector3 hitNormal, Damageable damageable)
        {
            damageable.TakeDamage(_damage, gameObject, false);
            PlayHitEffects(hitPoint, hitNormal);

            // ignore the collider of the damageable object, so that we don't hit it again
            var colliders = damageable.Health.GetComponentsInChildren<Collider>();
            _ignoredColliders.AddRange(colliders);

            _currentHits += 1;
            if (_currentHits >= _maxHits)
            {
                Destroy(gameObject);
                return;
            }

            TryFindNextTarget(out var nextTarget);
            // Always set the next target, even if we don't find one, so that the projectile can keep moving
            SetNextTarget(nextTarget);
        }

        private bool TryFindNextTarget(out Transform nextTarget)
        {
            var colliders = Physics.OverlapSphere(transform.position, maxDistance, hittableLayers);
            Collider closestCollider = null;
            float closestDistance = Mathf.Infinity;

            foreach (var coll in colliders)
            {
                if (!coll.TryGetComponent(out Damageable _) || _ignoredColliders.Contains(coll)) continue;

                float distance = Vector3.Distance(transform.position, coll.transform.position);
                if (!(distance < closestDistance)) continue;
                closestDistance = distance;
                closestCollider = coll;
            }

            if (closestCollider)
            {
                nextTarget = closestCollider.transform;
                return true;
            }

            nextTarget = null;
            return false;
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
                AudioUtility.CreateSfx(impactSound, hitPoint, AudioUtility.AudioGroups.EnemyAttack, spatialBlend: 1f,
                    randomizePitch: true);
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