﻿using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Shared
{
    public class BaseHealth : MonoBehaviour
    {
        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction OnDie;


        [Tooltip("Maximum amount of health")] public float maxHealth = 100f;

        private bool _isInvincible;

        public bool IsDead { get; private set; }
        protected float CurrentHealth { get; private set; }
        public float GetHealthRatio() => CurrentHealth / maxHealth;

        protected virtual void Start()
        {
            CurrentHealth = maxHealth;
        }

        public void Heal(float healAmount)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

            // call OnHeal action
            float trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
            {
                OnHealed?.Invoke(trueHealAmount);
            }
        }

        public void SetInvincibility(bool isInvincible)
        {
            _isInvincible = isInvincible;
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (_isInvincible)
                return;

            float healthBefore = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);

            // call OnDamage action
            float trueDamageAmount = healthBefore - CurrentHealth;

            if (trueDamageAmount > 0f)
            {
                OnDamaged?.Invoke(trueDamageAmount, damageSource);
            }

            HandleDeath();
        }

        public void Kill()
        {
            CurrentHealth = 0f;

            // call OnDamage action
            OnDamaged?.Invoke(maxHealth, null);

            HandleDeath();
        }

        private void HandleDeath()
        {
            if (IsDead)
                return;

            // call OnDie action
            if (!(CurrentHealth <= 0f)) return;
            IsDead = true;
            OnDie?.Invoke();
        }
    }
}