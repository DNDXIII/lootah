using Gameplay.Shared;
using Managers;
using Shared;
using UnityEngine;

namespace Gameplay.Player
{
    public class RecoverablePlayerHealth : BaseHealth
    {
        [SerializeField] private float maxRecoverableHealth = 30f;
        [SerializeField] private float recoverableHealthDecayDelay = 5f;

        /*
         * Time at which the recoverable health will decay to 0
         */
        private float _recoverableHealthDecayTime;
        private float _recoverableHealth;


        public float GetRecoverableHealth() => _recoverableHealth;

        public float GetHealthAndRecoverableHealthRatio() =>
            (CurrentHealth + _recoverableHealth) / maxHealth;

        protected override void Start()
        {
            base.Start();
            EventManager.AddListener<EnemyDamageEvent>(OnEnemyDamaged);
            OnDamaged += HandleTakeDamage;
        }


        private void Update()
        {
            if (_recoverableHealth > 0 && Time.time >= _recoverableHealthDecayTime)
            {
                _recoverableHealth = 0;
            }
        }

        private void HandleTakeDamage(float damage, GameObject source)
        {
            // handle the recoverable health
            _recoverableHealth = Mathf.Clamp(_recoverableHealth + damage, 0f, maxRecoverableHealth);
            _recoverableHealthDecayTime = Time.time + recoverableHealthDecayDelay;
        }


        private void OnEnemyDamaged(EnemyDamageEvent obj)
        {
            if (_recoverableHealth <= 0) return;

            var damageRecovered = Mathf.Min(_recoverableHealth, obj.DamageValue);
            _recoverableHealth -= damageRecovered;
            Heal(damageRecovered);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyDamageEvent>(OnEnemyDamaged);
            // remove the on damaged callback
            OnDamaged -= HandleTakeDamage;
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<EnemyDamageEvent>(OnEnemyDamaged);
            OnDamaged -= HandleTakeDamage;
        }
    }
}