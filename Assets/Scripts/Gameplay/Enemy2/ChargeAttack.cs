using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Shared;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Enemy2
{
    public class ChargeAttack : EnemyAttack
    {
        [SerializeField] private float overshootDistance = 3f;
        [SerializeField] private float chargeSpeed = 20f;
        [SerializeField] private float chargeAcceleration = 10f;
        [SerializeField] private float attackDuration = 2f;
        [SerializeField] private string attackAnimationBoolName = "ChargeAttack";

        [Header("References")] [SerializeField] [Required]
        private NavMeshAgent navMeshAgent;

        [SerializeField] [Required] private Collider attackCollider;
        [SerializeField] [Required] private EnemyAnimationController enemyAnimationController;


        private float _attackStartTime;
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
            PerformAttack(target, _cancellationToken.Token).Forget();
        }

        protected override void CancelAttack()
        {
            _cancellationToken?.Cancel();
            _cancellationToken = null;
        }

        private async UniTaskVoid PerformAttack(GameObject target, CancellationToken token)
        {
            PlayAttackEffects();

            await UniTask.WaitForSeconds(delayBeforeAttack, cancellationToken: token);

            attackCollider.enabled = true;
            navMeshAgent.enabled = true;
            navMeshAgent.stoppingDistance = 1f;
            navMeshAgent.speed = chargeSpeed;
            navMeshAgent.acceleration = chargeAcceleration;

            var targetPosition = target.transform.position;

            // Calculate a forward direction from the enemy to the target
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Move further beyond the target 
            Vector3 finalPosition = targetPosition + direction * overshootDistance;

            // Set the destination to the overshoot position
            navMeshAgent.SetDestination(finalPosition);
            navMeshAgent.isStopped = false;
            _attackStartTime = Time.time;
            enemyAnimationController?.SetBool(attackAnimationBoolName, true);
            // Wait until getting close enough to the overshoot position
            while (Vector3.Distance(transform.position, finalPosition) > navMeshAgent.stoppingDistance &&
                   _attackStartTime + attackDuration > Time.time)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                await UniTask.WaitForFixedUpdate();
            }

            attackCollider.enabled = false;
            enemyAnimationController?.SetBool(attackAnimationBoolName, false);

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