using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Shared;
using Gameplay.Weapons;
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

        private CancellationTokenSource _cancellationToken;

        protected override void StartAttack(GameObject target)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();
            ShootBurst(target, _cancellationToken.Token)
                .Forget();
        }

        protected override void CancelAttack()
        {
            _cancellationToken?.Cancel();
            _cancellationToken = null;
        }

        private async UniTaskVoid ShootBurst(GameObject target, CancellationToken cancellationToken)
        {
            PlayAttackEffects();


            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeAttack), cancellationToken: cancellationToken);

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                // Check if the enemy has been destroyed in the meantime
                if (!IsAttacking)
                    break;

                Vector3 shotDirection = GetShotDirectionWithinSpread(target);
                var projectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                projectile.Shoot(gameObject, attackDamage);

                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenProjectiles),
                    cancellationToken: cancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(delayAfterAttack), cancellationToken: cancellationToken);
            EndAttack();
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
    }
}