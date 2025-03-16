using System.Collections;
using Gameplay.Enemy.Boss;
using Gameplay.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemy.Attacks
{
    public class AreaAttack : AbstractEnemyAttack
    {
        [SerializeField] private float warningDuration = 2f;
        [SerializeField] private float areaActiveDuration = 0.3f;

        [Tooltip(
            "If true, the attack will be centered on the player's position. " +
            "Otherwise, it will stay in its starting location.")]
        [SerializeField]
        private bool centerAttackOnPlayer;

        [Header("References")] [SerializeField]
        private AreaDamage[] attackZones;


        protected override void Start()
        {
            base.Start();

            if (attackZones.Length == 0)
            {
                Debug.LogError("No attack zones assigned to the BossAreaAttack component.");
            }

            foreach (var attackZone in attackZones)
            {
                attackZone.Init(gameObject, attackDamage);
            }
        }


        protected override void PerformAttack(Damageable target)
        {
            StartCoroutine(AreaAttackRoutine(target));
        }


        private IEnumerator AreaAttackRoutine(Damageable target)
        {
            // Pick a random attack zone
            AreaDamage selectedZone = attackZones[Random.Range(0, attackZones.Length)];

            if (centerAttackOnPlayer)
            {
                selectedZone.transform.position = target.transform.position;
            }

            // Start the warning effect
            selectedZone.EnableWarningEffect();

            // Wait for the warning duration
            yield return new WaitForSeconds(warningDuration);

            // Damage players inside the rectangular area and disable the warning
            selectedZone.DisableWarningEffect();
            selectedZone.EnableAreaCollider();

            yield return new WaitForSeconds(areaActiveDuration);

            // Disable the area collider
            selectedZone.DisableAreaCollider();
        }
    }
}