using UnityEngine;

namespace Gameplay.Enemy2
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        [Header("Attack Properties")] [Tooltip("The damage dealt by the attack.")] [SerializeField]
        protected int attackDamage = 20;

        [Header("Shooting Delays")] [Tooltip("The delay before the first shot after charging.")] [SerializeField]
        protected float delayBeforeAttack;

        [Tooltip("The delay between consecutive projectiles in a burst.")] [SerializeField]
        protected float delayAfterAttack;

        public abstract bool TryAttack(GameObject target);
        public bool IsAttacking { get; protected set; }
    }
}