using System.Collections;
using Gameplay.Weapons;
using Managers;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemy2
{
    public class RangedAttack : EnemyAttack
    {
        [Header("Projectile Settings")] [Tooltip("The projectile prefab that will be fired.")] [SerializeField]
        private ProjectileBase projectilePrefab;

        [Tooltip("The number of projectiles fired per shot.")] [SerializeField]
        private int numberOfProjectiles = 3;

        [Tooltip("The delay between consecutive projectiles in a burst.")] [SerializeField]
        private float delayBetweenProjectiles = 0.5f;

        [Header("Weapon Components")] [Tooltip("The position from which projectiles are spawned.")] [SerializeField]
        private Transform weaponMuzzle;

        [Tooltip("The spread angle in degrees for projectiles.")] [SerializeField]
        private float weaponSpreadAngle = 10f;


        [SerializeField] private EnemyAttackToken attackType = EnemyAttackToken.Light;

        public override bool TryAttack(GameObject target)
        {
            if (!CanAttack()) return false;

            IsAttacking = true;
            StartCoroutine(ShootBurst(target));
            return true;
        }

        private IEnumerator ShootBurst(GameObject target)
        {
            yield return new WaitForSeconds(delayBeforeAttack);

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                // Check if the enemy has been destroyed in the meantime
                // if (EnemyController.IsDead) yield break;

                Vector3 shotDirection = GetShotDirectionWithinSpread(target);
                var projectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                projectile.Shoot(gameObject, attackDamage);

                yield return new WaitForSeconds(delayBetweenProjectiles);
            }

            yield return new WaitForSeconds(delayAfterAttack);

            IsAttacking = false;
            EnemyAttackTokenManager.Instance.ReleaseToken(attackType);
        }

        private Vector3 GetShotDirectionWithinSpread(GameObject target)
        {
            // Add a bit of height to the target position to make the shots more accurate
            var targetPosition = target.transform.position + Vector3.up;

            Vector3 toPlayer = targetPosition - weaponMuzzle.position;
            Vector3 spread = Random.insideUnitSphere * weaponSpreadAngle;
            Vector3 direction = toPlayer + spread;
            return direction.normalized;
        }

        private bool CanAttack()
        {
            return !IsAttacking &&
                   EnemyAttackTokenManager.Instance.RequestToken(attackType);
        }
    }
}