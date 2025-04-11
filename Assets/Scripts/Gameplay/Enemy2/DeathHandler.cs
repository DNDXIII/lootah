using System;
using Gameplay.Shared;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Enemy2
{
    [RequireComponent(typeof(Health))]
    public class DeathHandler : MonoBehaviour
    {
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private BehaviorGraphAgent _behavior;

        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (_health == null)
            {
                throw new Exception("No BaseHealth component found on object " + gameObject.name);
            }

            _health.OnDie += OnDie;
            _behavior = GetComponent<BehaviorGraphAgent>();
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnDie()
        {
            if (_animator)
            {
                _animator.SetBool(IsDead, true);
            }

            if (_behavior)
            {
                _behavior.enabled = false;
            }

            if (_navMeshAgent)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.enabled = false;
            }
        }
    }
}