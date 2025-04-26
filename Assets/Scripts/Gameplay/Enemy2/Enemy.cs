using System;
using Cysharp.Threading.Tasks;
using Gameplay.Shared;
using Managers;
using Shared.Utils;
using Sirenix.OdinInspector;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Gameplay.Enemy2
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(BehaviorGraphAgent))]
    [RequireComponent(typeof(EnemyAnimationController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        [Required] [SerializeField] public Transform center;
        public UnityAction<Enemy> OnDie;

        public EnemyConfig Config { get; private set; }

        public Transform Center => center;

        public Health Health { get; private set; }
        private IObjectPool<Enemy> _pool;
        private BehaviorGraphAgent _behaviorGraphAgent;
        private EnemyAnimationController _animationController;
        private NavMeshAgent _navMeshAgent;


        private void Awake()
        {
            Health = Preconditions.CheckNotNull(GetComponent<Health>());
            _behaviorGraphAgent = Preconditions.CheckNotNull(GetComponent<BehaviorGraphAgent>());
            _animationController = Preconditions.CheckNotNull(GetComponent<EnemyAnimationController>());
            _navMeshAgent = Preconditions.CheckNotNull(GetComponent<NavMeshAgent>());
        }

        private void Start()
        {
            Health.OnDie += OnHealthDeath;
        }

        public void Initialize(EnemyConfig config, Vector3 position, IObjectPool<Enemy> pool = null)
        {
            Config = config;
            _pool = pool;
            _navMeshAgent.Warp(position);
            Health.SetMaxHealth(config.health);
            SetActive();
        }


        private void OnHealthDeath()
        {
            EnemyKillEvent evt = Events.EnemyKillEvent;
            evt.Enemy = this;
            EventManager.Broadcast(evt);

            _behaviorGraphAgent.End();
            _animationController.PlayDeathAnimation();
            _navMeshAgent.isStopped = true;

            OnDie?.Invoke(this);
            DelayedDeath().Forget();
        }

        private void OnDestroy()
        {
            // When the enemy is destroyed, we need to release it back to the pool just so we don't have a dangling reference
            _pool?.Release(this);
        }

        private async UniTaskVoid DelayedDeath()
        {
            await UniTask.Delay(5000);
            _pool?.Release(this);
            _pool = null;
            SetInactive();
        }

        public Enemy SetActive()
        {
            gameObject.SetActive(true);
            Health.Reset();
            _animationController.Initialize();
            _behaviorGraphAgent.Restart();
            _navMeshAgent.isStopped = false;
            return this;
        }

        public Enemy SetInactive()
        {
            gameObject.SetActive(false);
            return this;
        }
    }
}