﻿using System.Collections;
using Gameplay.Shared;
using UnityEngine;

namespace Gameplay.Enemy2
{
    public class MeleeAttack : EnemyAttack
    {
        [SerializeField] private Collider attackCollider;
        [SerializeField] private float attackDuration = 0.5f;

        private void Awake()
        {
            attackCollider.isTrigger = true;
            attackCollider.enabled = false;
        }


        protected override void StartAttack(GameObject target)
        {
            StartCoroutine(PerformAttack());
        }

        private IEnumerator PerformAttack()
        {
            PlayAttackEffects();

            yield return new WaitForSeconds(delayBeforeAttack);

            attackCollider.enabled = true;

            yield return new WaitForSeconds(attackDuration);
            attackCollider.enabled = false;

            yield return new WaitForSeconds(delayAfterAttack);

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