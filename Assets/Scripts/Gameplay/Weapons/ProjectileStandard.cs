﻿using System.Collections.Generic;
using Gameplay.Player;
using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class ProjectileStandard : ProjectileBase
    {
        [Header("General")] [Tooltip("Radius of this projectile's collision detection")]
        public float Radius = 0.01f;

        [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
        public Transform Root;

        [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
        public Transform Tip;

        [Tooltip("LifeTime of the projectile")]
        public float MaxLifeTime = 5f;

        [Tooltip("VFX prefab to spawn upon impact")]
        public GameObject ImpactVfx;

        [Tooltip("LifeTime of the VFX before being destroyed")]
        public float ImpactVfxLifetime = 5f;

        [Tooltip("Offset along the hit normal where the VFX will be spawned")]
        public float ImpactVfxSpawnOffset = 0.1f;

        [Tooltip("Clip to play on impact")] public AudioClip ImpactSfxClip;

        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        [Header("Movement")] [Tooltip("Speed of the projectile")]
        public float Speed = 20f;

        [Tooltip("Downward acceleration from gravity")]
        public float GravityDownAcceleration = 0f;

        [Tooltip(
            "Distance over which the projectile will correct its course to fit the intended trajectory (used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction")]
        public float TrajectoryCorrectionDistance = -1;


        [Header("Debug")] [Tooltip("Color of the projectile radius debug view")]
        public Color RadiusColor = Color.red * 0.2f;

        [Header("Explosion Settings")] public bool isExplosive;
        public float explosionRadius = 5f;

        ProjectileBase m_ProjectileBase;
        Vector3 m_LastRootPosition;
        Vector3 m_Velocity;
        bool m_HasTrajectoryOverride;
        Vector3 m_TrajectoryCorrectionVector;
        Vector3 m_ConsumedTrajectoryCorrectionVector;
        List<Collider> m_IgnoredColliders;
        private bool m_HasHit;
        private float m_LifeTime;

        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        void OnEnable()
        {
            m_ProjectileBase = GetComponent<ProjectileBase>();
            m_ProjectileBase.OnShoot += OnShoot;
        }

        private new void OnShoot()
        {
            m_HasHit = false;
            m_LifeTime = 0;
            m_LastRootPosition = Root.position;
            m_Velocity = transform.forward * Speed;
            m_IgnoredColliders = new List<Collider>();

            // Ignore colliders of owner
            var ownerColliders = m_ProjectileBase.Owner.GetComponentsInChildren<Collider>();
            m_IgnoredColliders.AddRange(ownerColliders);

            // Handle case of player shooting (make projectiles not go through walls, and remember center-of-screen trajectory)
            PlayerWeaponsManager playerWeaponsManager = m_ProjectileBase.Owner.GetComponent<PlayerWeaponsManager>();
            if (!playerWeaponsManager) return;

            m_HasTrajectoryOverride = true;

            Vector3 cameraToMuzzle = (m_ProjectileBase.InitialPosition -
                                      playerWeaponsManager.weaponCamera.transform.position);

            m_TrajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                playerWeaponsManager.weaponCamera.transform.forward);
            if (TrajectoryCorrectionDistance == 0)
            {
                transform.position += m_TrajectoryCorrectionVector;
                m_ConsumedTrajectoryCorrectionVector = m_TrajectoryCorrectionVector;
            }
            else if (TrajectoryCorrectionDistance < 0)
            {
                m_HasTrajectoryOverride = false;
            }

            if (Physics.Raycast(playerWeaponsManager.weaponCamera.transform.position, cameraToMuzzle.normalized,
                    out RaycastHit hit, cameraToMuzzle.magnitude, HittableLayers, k_TriggerInteraction))
            {
                if (IsHitValid(hit))
                {
                    OnHit(hit.point, hit.normal, hit.collider);
                }
            }
        }

        private void Update()
        {
            m_LifeTime += Time.deltaTime;

            if (m_LifeTime > MaxLifeTime && !m_HasHit)
            {
                Destroy(gameObject);
                return;
            }

            // Move
            transform.position += m_Velocity * Time.deltaTime;

            // Drift towards trajectory override (this is so that projectiles can be centered 
            // with the camera center even though the actual weapon is offset)
            if (m_HasTrajectoryOverride && m_ConsumedTrajectoryCorrectionVector.sqrMagnitude <
                m_TrajectoryCorrectionVector.sqrMagnitude)
            {
                Vector3 correctionLeft = m_TrajectoryCorrectionVector - m_ConsumedTrajectoryCorrectionVector;
                float distanceThisFrame = (Root.position - m_LastRootPosition).magnitude;
                Vector3 correctionThisFrame =
                    (distanceThisFrame / TrajectoryCorrectionDistance) * m_TrajectoryCorrectionVector;
                correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
                m_ConsumedTrajectoryCorrectionVector += correctionThisFrame;

                // Detect end of correction
                if (Mathf.Approximately(m_ConsumedTrajectoryCorrectionVector.sqrMagnitude,
                        m_TrajectoryCorrectionVector.sqrMagnitude))
                {
                    m_HasTrajectoryOverride = false;
                }

                transform.position += correctionThisFrame;
            }

            // Orient towards velocity
            transform.forward = m_Velocity.normalized;

            // Gravity
            if (GravityDownAcceleration > 0)
            {
                // add gravity to the projectile velocity for ballistic effect
                m_Velocity += Vector3.down * (GravityDownAcceleration * Time.deltaTime);
            }

            // If the projectile has hit something, do not perform collision detection
            if (m_HasHit) return;

            // Hit detection
            {
                RaycastHit closestHit = new RaycastHit
                {
                    distance = Mathf.Infinity
                };
                bool foundHit = false;

                // Sphere cast
                Vector3 displacementSinceLastFrame = Tip.position - m_LastRootPosition;
                var hits = Physics.SphereCastAll(m_LastRootPosition, Radius,
                    displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers,
                    k_TriggerInteraction);
                foreach (var hit in hits)
                {
                    if (IsHitValid(hit) && hit.distance < closestHit.distance)
                    {
                        foundHit = true;
                        closestHit = hit;
                    }
                }

                if (foundHit)
                {
                    // Handle case of casting while already inside a collider
                    if (closestHit.distance <= 0f)
                    {
                        closestHit.point = Root.position;
                        closestHit.normal = -transform.forward;
                    }

                    OnHit(closestHit.point, closestHit.normal, closestHit.collider);
                }
            }

            m_LastRootPosition = Root.position;
        }

        private bool IsHitValid(RaycastHit hit)
        {
            // ignore hits with triggers that don't have a Damageable component
            if (hit.collider.isTrigger && !hit.collider.TryGetComponent<Damageable>(out _))
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            return m_IgnoredColliders == null || !m_IgnoredColliders.Contains(hit.collider);
        }

        private void OnHit(Vector3 point, Vector3 normal, Collider collisionCollider)
        {
            if (isExplosive)
            {
                DealExplosionDamage(point);
            }
            else if (collisionCollider.TryGetComponent(out Damageable damageable))
            {
                // point damage
                damageable.TakeDamage(Damage, m_ProjectileBase.Owner, true);
            }

            // impact vfx
            if (ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(ImpactVfx, point + normal * ImpactVfxSpawnOffset,
                    Quaternion.LookRotation(normal));
                if (ImpactVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
                }
            }

            // impact sfx
            if (ImpactSfxClip)
            {
                AudioUtility.CreateSfx(ImpactSfxClip, point, AudioUtility.AudioGroups.EnemyAttack, 1f);
            }

            // Self Destruct
            Destroy(gameObject);
        }

        private void DealExplosionDamage(Vector3 point)
        {
            var colliders = Physics.OverlapSphere(point, explosionRadius, HittableLayers, k_TriggerInteraction);
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out Damageable damageable))
                {
                    damageable.TakeDamage(Damage, m_ProjectileBase.Owner, true);
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = RadiusColor;
            Gizmos.DrawSphere(transform.position, Radius);
        }
    }
}