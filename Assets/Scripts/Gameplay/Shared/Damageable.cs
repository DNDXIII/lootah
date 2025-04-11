using System;
using Managers;
using Shared;
using UnityEngine;

namespace Gameplay.Shared
{
    [RequireComponent(typeof(Collider))]
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private Health health;

        [SerializeField] private bool isCritical;

        private Collider _collider;

        public Health Health => health;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void Start()
        {
            if (health == null)
            {
                health = GetComponent<Health>();
                if (health == null)
                    throw new Exception("No BaseHealth component found on object " + gameObject.name);
            }

            health.OnDie += OnDie;
        }

        private void OnDie()
        {
            // disable the collider so that the player won't hit this player again
            _collider.enabled = false;
        }

        public void TakeDamage(float damage, GameObject damageSource, bool canCrit)
        {
            health.TakeDamage(damage * damageMultiplier, damageSource);
            if (isCritical && canCrit)
            {
                EventManager.Broadcast(new CriticalHitEvent());
            }
        }
    }
}