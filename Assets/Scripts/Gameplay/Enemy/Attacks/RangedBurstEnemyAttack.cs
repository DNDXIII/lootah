using System.Collections;
using Gameplay.Shared;
using Gameplay.Weapons;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemy.Attacks
{
    public class RangedBurstEnemyAttack : AbstractEnemyAttack
    {
        [Header("Projectile Settings")] [Tooltip("The projectile prefab that will be fired.")] [SerializeField]
        private ProjectileBase projectilePrefab;

        [Tooltip("The number of projectiles fired per shot.")] [SerializeField]
        private int numberOfProjectiles = 3;

        [Tooltip("The spread angle in degrees for projectiles.")] [SerializeField]
        private float weaponSpreadAngle = 10f;

        [Header("Shooting Delays")] [Tooltip("The delay before the first shot after charging.")] [SerializeField]
        public float delayBeforeFirstShot;

        [Tooltip("The delay between consecutive projectiles in a burst.")] [SerializeField]
        private float delayBetweenProjectiles = 0.5f;

        [Header("Weapon Components")] [Tooltip("The position from which projectiles are spawned.")] [SerializeField]
        private Transform weaponMuzzle;

        [Header("Audio")] [Tooltip("The sound effect played when the weapon is charging.")] [SerializeField]
        public AudioClip chargeSfx;

        protected override void PerformAttack(Damageable target)
        {
            StartCoroutine(ShootBurst(target));
        }

        private IEnumerator ShootBurst(Damageable target)
        {
            if (chargeSfx)
            {
                AudioUtility.CreateSfx(chargeSfx, weaponMuzzle.position, AudioUtility.AudioGroups.EnemyAttack, 1f);
            }

            yield return new WaitForSeconds(delayBeforeFirstShot);

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                // Check if the enemy has been destroyed in the meantime
                if (EnemyController.IsDead) yield break;

                Vector3 shotDirection = GetShotDirectionWithinSpread(target);
                var projectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                projectile.Shoot(EnemyController.gameObject, attackDamage);
                if (attackSfx)
                {
                    AudioUtility.CreateSfx(attackSfx, weaponMuzzle.position, AudioUtility.AudioGroups.EnemyAttack, 1f);
                }

                yield return new WaitForSeconds(delayBetweenProjectiles);
            }
        }

        private Vector3 GetShotDirectionWithinSpread(Damageable target)
        {
            // Add a bit of height to the target position to make the shots more accurate
            var targetPosition = target.transform.position + Vector3.up;

            Vector3 toPlayer = targetPosition - weaponMuzzle.position;
            Vector3 spread = Random.insideUnitSphere * weaponSpreadAngle;
            Vector3 direction = toPlayer + spread;
            return direction.normalized;
        }
    }
}