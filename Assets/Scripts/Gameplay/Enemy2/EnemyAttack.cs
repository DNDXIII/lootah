using System;
using Gameplay.Managers;
using Gameplay.Shared;
using Managers;
using Shared;
using Sirenix.OdinInspector;
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

        [Header("References")] [SerializeField] [Required]
        private Health health;

        protected virtual void Start()
        {
            health.OnDie += OnDie;
        }

        private void OnDie()
        {
            if (!IsAttacking) return;
            // Cancel the attack if the enemy dies while attacking
            CancelAttack();
            // Release the attack token if the enemy dies while attacking
            EndAttack();
        }


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

        protected abstract void CancelAttack();

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
            // It is possible that the token manager is destroyed, if the scene is getting unloaded
            if (EnemyAttackTokenManager.TryGetInstance(out var tokenManager))
            {
                tokenManager.ReleaseToken(attackType);
            }
        }

        private void OnDisable()
        {
            if (!IsAttacking) return;
            EndAttack();
        }
    }
}