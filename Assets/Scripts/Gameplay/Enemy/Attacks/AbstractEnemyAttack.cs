using Gameplay.Shared;
using UnityEngine;

namespace Gameplay.Enemy.Attacks
{
    public abstract class AbstractEnemyAttack : MonoBehaviour
    {
        [Tooltip("The delay between attacks in seconds")] [SerializeField]
        protected internal float attackDelaySeconds = 2f;

        [SerializeField] public float attackDuration;
        [SerializeField] protected int attackDamage = 20;
        [SerializeField] protected AudioClip attackSfx;


        protected EnemyController EnemyController;
        private float _lastAttackTime = float.MinValue;

        protected virtual void Start()
        {
            EnemyController = GetComponent<EnemyController>();
        }

        public bool CanAttack()
        {
            return _lastAttackTime + attackDuration + attackDelaySeconds < Time.time;
        }


        public void Attack(Damageable target)
        {
            _lastAttackTime = Time.time;
            PerformAttack(target);
        }

        protected abstract void PerformAttack(Damageable target);
    }
}