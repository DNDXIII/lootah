using Gameplay.Managers;
using Managers;
using Shared;
using UnityEngine;

namespace Gameplay.Enemy2
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        // Animation
        private static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");

        [Header("Animation")] [Tooltip("Animator component handling attack animations.")] [SerializeField]
        private Animator animator;

        // Sound
        [Header("Audio")] [Tooltip("Sound effect played during the attack.")] [SerializeField]
        private AudioClip attackSfx;

        // Attack Settings
        [Header("Attack Settings")] [Tooltip("The type of attack the enemy uses.")] [SerializeField]
        protected EnemyAttackToken attackType = EnemyAttackToken.Light;

        [Tooltip("The damage dealt by the attack.")] [SerializeField]
        protected int attackDamage = 20;

        // Timing Settings
        [Header("Attack Timing")] [Tooltip("The delay before the first attack after charging.")] [SerializeField]
        protected float delayBeforeAttack;

        [Tooltip("The delay after the attack before the next action.")] [SerializeField]
        protected float delayAfterAttack;

        // State
        [Header("State")]
        [Tooltip("Indicates whether the enemy is currently attacking.")]
        public bool IsAttacking { get; private set; }


        private bool CanAttack()
        {
            return !IsAttacking &&
                   EnemyAttackTokenManager.Instance.RequestToken(attackType);
        }

        public bool TryAttack(GameObject target)
        {
            if (!CanAttack()) return false;
            IsAttacking = true;
            StartAttack(target);
            return true;
        }

        protected abstract void StartAttack(GameObject target);

        protected void PlayAttackEffects()
        {
            if (animator)
            {
                animator.SetTrigger(AttackTrigger);
            }

            if (attackSfx)
            {
                AudioUtility.CreateSfx(attackSfx, transform.position, AudioUtility.AudioGroups.EnemyAttack, .8f);
            }
        }

        protected void EndAttack()
        {
            IsAttacking = false;
            EnemyAttackTokenManager.Instance.ReleaseToken(attackType);
        }

        private void OnDisable()
        {
            if (!IsAttacking) return;
            EndAttack();
        }
    }
}