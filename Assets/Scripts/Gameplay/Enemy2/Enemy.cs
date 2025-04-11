using Gameplay.Shared;
using Managers;
using Shared;
using Unity.Behavior;
using UnityEngine;

namespace Gameplay.Enemy2
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Transform center;


        public Transform Center => center;
        public Health Health { get; private set; }


        private void Awake()
        {
            Health = GetComponent<Health>();
        }

        private void Start()
        {
            Health.OnDie += OnDie;
        }

        private void OnDie()
        {
            EnemyKillEvent evt = Events.EnemyKillEvent;
            evt.Enemy = this;
            EventManager.Broadcast(evt);

            if (TryGetComponent<BehaviorGraphAgent>(out var behaviorGraphAgent))
            {
                behaviorGraphAgent.enabled = false;
            }
        }
    }
}