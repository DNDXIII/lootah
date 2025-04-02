using System;
using Gameplay.Enemy.Attacks;
using Gameplay.Shared;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Enemy
{
    [RequireComponent(typeof(EnemyController), typeof(NavMeshAgent))]
    public class EnemyMovement : EnemyAI
    {
        // Animation
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        // AI State
        private enum AIState
        {
            Idle,
            Follow,
            Attack
        }

        // AI Settings
        [Header("AI Behavior")]
        [Tooltip("Determines if the enemy starts following the player immediately.")]
        [SerializeField]
        private bool startsFollowingPlayer;

        [Tooltip("Radius within which the enemy detects the player.")] [SerializeField]
        private float detectionRadius = 30f;

        [Tooltip("Transform from which the enemy will detect the player.")] [SerializeField]
        private Transform detectionTransform;

        [Tooltip("Layer mask for the detection raycast.")] [SerializeField]
        private LayerMask detectionLayerMask;

        [Tooltip("Time interval for updating the enemy's line of sight.")]
        private const float LosUpdateInterval = 0.5f;

        [Header("Movement Settings")] [Tooltip("Speed at which the enemy rotates towards the target.")] [SerializeField]
        private float angularSpeed = 5f;

        [Tooltip("Speed at which the enemy moves towards the target.")] [SerializeField]
        private float moveSpeed = 5f;

        [Tooltip("Acceleration of the enemy.")] [SerializeField]
        private float acceleration = 15f;

        [Tooltip("Delay before the enemy starts moving after spawning.")] [SerializeField]
        private float spawnDelay = 2f;

        [Tooltip("The radius within which the enemy can randomly move when idle.")] [SerializeField]
        private float randomMoveRadius = 5f;

        [Header("Attack Settings")] [Tooltip("The range at which the enemy will attack the player.")] [SerializeField]
        protected internal float attackRange = 2f;

        [Tooltip("The delay during which the enemy stands still after attacking.")] [SerializeField]
        private float attackMovementDelay = 1f;

        [Tooltip("The enemy attack behavior component.")] [SerializeField]
        private AbstractEnemyAttack enemyAttack;

        // Internal Components
        private Animator _animator;
        private AIState _currentState = AIState.Idle;
        private NavMeshAgent _navMeshAgent;
        private BaseHealth _health;

        // Internal Variables
        private float _lastLosUpdate = float.MinValue;
        private float _lastAttackTime = float.MinValue;
        private bool _lastLosResult;
        private bool _hasRandomlyMoved;


        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            _health = GetComponent<BaseHealth>();
            _health.OnDamaged += OnDamaged;


            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = moveSpeed;
            _navMeshAgent.acceleration = acceleration;
            _navMeshAgent.angularSpeed = angularSpeed;
        }

        private void OnDestroy()
        {
            _health.OnDamaged -= OnDamaged;
        }


        private void Update()
        {
            if (IsActive)
            {
                HandleState();
            }
        }


        private void SwitchState(AIState newState)
        {
            switch (newState)
            {
                case AIState.Idle:
                    _animator?.SetBool(IsMoving, false);

                    break;
                case AIState.Follow:
                    if (_currentState == AIState.Idle)
                    {
                        EnemyController.PlaySightSfx();
                    }

                    // Reset the random move timer
                    _hasRandomlyMoved = false;

                    _animator?.SetBool(IsMoving, true);

                    _navMeshAgent.isStopped = false;
                    break;
                case AIState.Attack:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            _currentState = newState;
        }


        private void HandleState()
        {
            if (EnemyController.IsDead)
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            switch (_currentState)
            {
                case AIState.Idle:
                    HandleIdleState();
                    break;
                case AIState.Follow:
                    HandleFollowState();
                    break;
                case AIState.Attack:
                    HandleAttackState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleAttackState()
        {
            if (!enemyAttack.CanAttack() ||
                !IsTargetInAttackRange(ActorManager.Instance.Player.transform.position) ||
                !HasLineOfSight())
            {
                if (_lastAttackTime + attackMovementDelay < Time.time)
                {
                    SwitchState(AIState.Follow);
                }
            }
            else
            {
                _navMeshAgent.isStopped = true;
                enemyAttack.Attack(ActorManager.Instance.Player.Damageable);
                EnemyController.PlayAttackAnimation();
                _lastAttackTime = Time.time;
            }
        }

        private void MoveToRandomLocation()
        {
            // Pick a random point inside a sphere centered around the player
            Vector3 randomPoint = transform.position + UnityEngine.Random.onUnitSphere * randomMoveRadius;

            // Ensure the random point is on the same plane (ignore Y-axis for 2D movement)
            randomPoint.y = transform.position.y;

            _navMeshAgent.SetDestination(randomPoint);
            _navMeshAgent.isStopped = false;
        }

        //Check if there is a line of sight between the enemy and the player
        private bool HasLineOfSight()
        {
            // Only update the line of sight every 0.5 seconds
            if (!(_lastLosUpdate + LosUpdateInterval < Time.time)) return _lastLosResult;
            _lastLosUpdate = Time.time;

            var playerPosition = ActorManager.Instance.Player.transform.position + Vector3.up;
            // Draw the line in debug mode
            var detectionOrigin = detectionTransform ? detectionTransform.position : transform.position;

            _lastLosResult = Physics.Raycast(detectionOrigin,
                                 playerPosition - detectionOrigin, out var hit, detectionRadius, detectionLayerMask) &&
                             hit.collider.CompareTag("Player");

            return _lastLosResult;
        }

        private void HandleFollowState()
        {
            var playerPosition = ActorManager.Instance.Player.transform.position;
            var inAttackRange = IsTargetInAttackRange(playerPosition);
            var hasLineOfSight = HasLineOfSight();

            // if we are not in attack range, or we don't have line of sight, move closer to the player
            if (!inAttackRange || !hasLineOfSight)
            {
                _navMeshAgent.SetDestination(playerPosition);
                _navMeshAgent.isStopped = false;
                return;
            }

            // If we are in attack range and have line of sight, switch to attack state, if we can attack
            if (enemyAttack.CanAttack())
            {
                SwitchState(AIState.Attack);

                return;
            }

            // Otherwise just move in a random direction
            if (_hasRandomlyMoved) return;
            MoveToRandomLocation();
            _hasRandomlyMoved = true;
        }

        private bool IsTargetInAttackRange(Vector3 playerPosition)
        {
            return Vector3.Distance(playerPosition, transform.position) <= attackRange;
        }

        // TODO: Frequently called, can be optimized by adding a timer
        private void HandleIdleState()
        {
            if (spawnDelay > 0)
            {
                spawnDelay -= Time.deltaTime;
                return;
            }

            if (startsFollowingPlayer || IsPlayerInDetectionRadius() && HasLineOfSight())
            {
                SwitchState(AIState.Follow);
            }
        }

        private bool IsPlayerInDetectionRadius()
        {
            return Vector3.Distance(ActorManager.Instance.Player.transform.position, transform.position) <=
                   detectionRadius;
        }

        private void OnDamaged(float arg0, GameObject arg1)
        {
            if (_currentState == AIState.Idle)
            {
                SwitchState(AIState.Follow);
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}