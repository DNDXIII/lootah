using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Shared;
using UnityEngine;

namespace Gameplay.Enemy2
{
    public class MeleeAttack : EnemyAttack
    {
        [SerializeField] private Collider attackCollider;
        [SerializeField] private float attackDuration = 0.5f;

        private CancellationTokenSource _cancellationToken;

        private void Awake()
        {
            attackCollider.isTrigger = true;
            attackCollider.enabled = false;
        }


        protected override void StartAttack(GameObject target)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();
            PerformAttack(_cancellationToken.Token).Forget();
        }

        protected override void CancelAttack()
        {
            _cancellationToken?.Cancel();
            _cancellationToken = null;
        }

        private async UniTaskVoid PerformAttack(CancellationToken token)
        {
            PlayAttackEffects();

            await UniTask.WaitForSeconds(delayBeforeAttack, cancellationToken: token);

            attackCollider.enabled = true;

            await UniTask.WaitForSeconds(attackDuration, cancellationToken: token);
            attackCollider.enabled = false;

            await UniTask.WaitForSeconds(delayAfterAttack, cancellationToken: token);

            EndAttack();
        }

        // Damage the player if they are in the area while it is active
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<Damageable>().TakeDamage(attackDamage, gameObject, false);


            // Disable the collider after dealing damage, so the player doesn't take damage multiple times
            attackCollider.enabled = false;
        }
    }
}