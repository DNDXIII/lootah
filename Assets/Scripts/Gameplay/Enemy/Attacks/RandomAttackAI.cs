using Managers;
using UnityEngine;

namespace Gameplay.Enemy.Attacks
{
    [RequireComponent(typeof(EnemyController))]
    public class RandomAttackAI : EnemyAI
    {
        [SerializeField] private AbstractEnemyAttack[] attacks;

        [Tooltip(
            "The minimum delay after an attack before the next one can be performed. Note that attacks have their own delay as well.")]
        [SerializeField]
        private float minDelayAfterAttack = 1f;


        private float _nextAttackTime;


        public override void Activate()
        {
            base.Activate();
            _nextAttackTime = Time.time;
            EnemyController.PlaySightSfx();
        }

        private void Update()
        {
            if (!IsActive) return;

            if (Time.time >= _nextAttackTime)
            {
                PerformRandomAttack();
            }
        }

        private void PerformRandomAttack()
        {
            var randomAttack = attacks[Random.Range(0, attacks.Length)];
            if (!randomAttack.CanAttack()) return;

            randomAttack.Attack(ActorManager.Instance.Player.Damageable);
            _nextAttackTime = Time.time + randomAttack.attackDuration + minDelayAfterAttack;
        }
    }
}