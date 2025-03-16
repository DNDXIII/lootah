using System.Collections;
using Gameplay.Enemy.Attacks;
using Gameplay.Shared;
using Managers;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AbstractEnemyAttack))]
    public class EnemyController : MonoBehaviour
    {
        // Animation
        private static readonly int IsDeadAnimationState = Animator.StringToHash("IsDead");

        // General Enemy Properties
        [Header("Enemy Stats")] [Tooltip("The base experience given when the enemy is defeated.")] [SerializeField]
        private int baseExperience = 10;

        [Tooltip("The delay before the enemy is fully considered dead.")] [SerializeField]
        private float deathDelay;

        [Header("Enemy Health")] [Tooltip("The health component of the enemy.")] [SerializeField]
        private BaseHealth health;

        [Header("Enemy Visuals")] [Tooltip("The color the enemy turns when damaged.")] [SerializeField]
        private Color damagedColor = Color.red;

        // Audio
        [Header("Enemy Audio")] [Tooltip("Sound effects played when the enemy dies.")] [SerializeField]
        private AudioClip[] deathSfx;

        [Tooltip("Sound effect played when the enemy spots the player.")] [SerializeField]
        private AudioClip sightSfx;

        // Physics
        [Header("Enemy Physics")]
        [Tooltip("The collider that should be disabled when the enemy dies.")]
        [SerializeField]
        private Collider rbCollider;

        // Internal Components
        private Animator _animator;
        private Color _originalColor;
        private Renderer _renderer;

        // Public Properties
        public bool IsDead => health.IsDead;
        public int BaseExperience => baseExperience;

        private void Awake()
        {
            if (health == null)
            {
                health = GetComponent<BaseHealth>();
            }

            health.OnDie += OnDie;
            health.OnDamaged += OnDamaged;

            _animator = GetComponentInChildren<Animator>();
            _renderer = GetComponentInChildren<Renderer>();
            _originalColor = _renderer.material.color;
        }

        public void PlayAttackAnimation()
        {
            // TODO: Can probably go into a separate class, or even in the attack directly
            if (_animator)
            {
                _animator.Play("Enemy_Attack");
            }
        }

        private void OnDie()
        {
            EnemyKillEvent evt = Events.EnemyKillEvent;
            evt.Enemy = this;
            EventManager.Broadcast(evt);

            if (deathSfx.Length > 0)
            {
                var randomIndex = Random.Range(0, deathSfx.Length);
                AudioUtility.CreateSfx(deathSfx[randomIndex], transform.position, AudioUtility.AudioGroups.EnemyDeath,
                    1f);
            }

            if (_animator)
            {
                _animator.SetBool(IsDeadAnimationState, true);
            }

            // Disable the collider so nothing else can interact with the enemy
            if (rbCollider)
            {
                rbCollider.enabled = false;
            }

            Destroy(gameObject, deathDelay);
        }

        private void OnDamaged(float damageInflicted, GameObject damageDealer)
        {
            // TODO: Implement a stagger effect only when a certain amount of damage is dealt
            // if (_animator)
            // {
            //     _animator.Play($"Enemy_Hit");
            // }

            StartCoroutine(FlashColor(damagedColor));

            EnemyDamageEvent evt = Events.EnemyDamageEvent;
            evt.Sender = gameObject;
            evt.DamageValue = damageInflicted;
            EventManager.Broadcast(evt);
        }

        private IEnumerator FlashColor(Color color)
        {
            _renderer.material.color = color;
            yield return new WaitForSeconds(0.1f);
            _renderer.material.color = _originalColor;
        }

        private void OnDestroy()
        {
            health.OnDie -= OnDie;
            health.OnDamaged -= OnDamaged;
        }

        public void PlaySightSfx()
        {
            if (sightSfx)
            {
                AudioUtility.CreateSfx(sightSfx, transform.position, AudioUtility.AudioGroups.EnemyDetection, 1f);
            }
        }
    }
}