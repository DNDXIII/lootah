using Gameplay.Enemy.Attacks;
using UnityEngine;

namespace Gameplay.Enemy.Boss
{
    [RequireComponent(typeof(EnemyController))]
    public class BossEnemyAi : EnemyAI
    {
        [SerializeField] private BossPart[] parts;
        [SerializeField] private BossPart core;
        [SerializeField] private RandomAttackAI attackAI;

        private int _activePartsCount;

        public override void Activate()
        {
            base.Activate();
            attackAI.Activate();
        }


        protected override void Start()
        {
            base.Start();
            _activePartsCount = parts.Length;
            foreach (var part in parts)
            {
                part.OnDie += HandlePartDeath;
            }

            // Make core invincible at the start
            core.SetInvincibility(true);
        }


        private void HandlePartDeath()
        {
            _activePartsCount--;

            // When all arms are destroyed, make core vulnerable
            if (_activePartsCount <= 0)
            {
                core.SetInvincibility(false);
            }
        }


        private void OnDestroy()
        {
            foreach (var arm in parts)
            {
                arm.OnDie -= HandlePartDeath;
            }
        }
    }
}