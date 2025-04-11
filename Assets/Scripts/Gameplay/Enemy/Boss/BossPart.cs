using Gameplay.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Enemy.Boss
{
    [RequireComponent(typeof(Health))]
    public class BossPart : MonoBehaviour
    {
        public UnityAction OnDie;

        private Health _health;

        // TODO: Debug
        public Renderer localRenderer;
        private Color _originalColor;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnDie += HandleDeath;

            _originalColor = localRenderer.material.color;
        }

        public void SetInvincibility(bool isInvincible)
        {
            _health.SetInvincibility(isInvincible);

            localRenderer.material.color = isInvincible ? Color.blue : _originalColor;
        }


        private void HandleDeath()
        {
            OnDie?.Invoke();
            localRenderer.material.color = Color.red;
        }

        private void OnDestroy()
        {
            _health.OnDie -= HandleDeath;
        }
    }
}