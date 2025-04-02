using System.Collections;
using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Enemy.Attacks
{
    [RequireComponent(typeof(Collider))]
    public class MeleeEnemyAttack : AbstractEnemyAttack
    {
        [SerializeField] private float delayBeforeAttack = .5f;
        [SerializeField] private Collider attackCollider;

        private void Awake()
        {
            attackCollider.isTrigger = true;
            attackCollider.enabled = false;
        }

        protected override void PerformAttack(Damageable target)
        {
            // Delay for a bit before attacking
            StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            if (attackSfx)
            {
                AudioUtility.CreateSfx(attackSfx, transform.position, AudioUtility.AudioGroups.EnemyAttack, 1f);
            }

            yield return new WaitForSeconds(delayBeforeAttack);

            attackCollider.enabled = true;

            yield return new WaitForSeconds(attackDuration);
            attackCollider.enabled = false;
        }

        // Damage the player if they are in the area while it is active
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<Damageable>().TakeDamage(attackDamage, EnemyController.gameObject, false);


            // Disable the collider after dealing damage, so the player doesn't take damage multiple times
            attackCollider.enabled = false;
        }
    }
}