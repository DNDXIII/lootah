using System.Collections;
using Gameplay.Enemy2;
using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Abilities.SpawnFly
{
    [RequireComponent(typeof(Collider))]
    public class FlyController : MonoBehaviour
    {
        private const float SearchDelay = 1f;

        [SerializeField] private float searchRadius = 10f;
        [SerializeField] private LayerMask enemyLayer; // Set in the Inspector for optimization
        [SerializeField] private float moveSpeed = 5f; // Movement speed
        [SerializeField] private float initialUpwardSpeed = 20f; // Initial upward speed
        [SerializeField] private int damage = 10; // Damage dealt to the enemy

        [Header("Time to wait before moving to the enemy")] [SerializeField]
        private float idleTimeAfterSpawn = .5f;

        [Header("Upward movement speed decay")] [SerializeField]
        private float upwardSpeedDecay = .1f;

        private Enemy2.Enemy _closestEnemy;
        private Rigidbody _rigidbody;
        private float _lastEnemySearchTime = float.MinValue;
        private float _currentUpwardSpeed;
        private bool _isIdle = true;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _closestEnemy = FindClosestEnemy();
            _currentUpwardSpeed = initialUpwardSpeed;

            StartCoroutine(UpwardMovement());
        }


        private void Update()
        {
            if (_isIdle) return;
            HandleActive();
        }


        private IEnumerator UpwardMovement()
        {
            float timeElapsed = 0f;

            // Move upwards while gradually reducing speed
            while (timeElapsed < idleTimeAfterSpawn)
            {
                _rigidbody.MovePosition(transform.position + Vector3.up * (_currentUpwardSpeed * Time.deltaTime));

                // Gradually decrease speed as it moves up
                _currentUpwardSpeed = Mathf.Max(0f, _currentUpwardSpeed - upwardSpeedDecay * Time.deltaTime);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // After delay, stop moving up and start moving towards the enemy
            _isIdle = false;
        }


        private void HandleActive()
        {
            // Move if we have a valid enemy
            if (_closestEnemy)
            {
                MoveTowardsEnemy();
                return;
            }

            // Check if the current enemy is valid (exists and active)
            if (!_closestEnemy || !_closestEnemy.gameObject.activeInHierarchy &&
                _lastEnemySearchTime + SearchDelay < Time.time)
            {
                _lastEnemySearchTime = Time.time;
                _closestEnemy = FindClosestEnemy();
            }

            else
            {
                // TODO: Think what to do if there is no enemy
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isIdle) return;
            if (!other.TryGetComponent(out Enemy2.Enemy _)) return;

            if (other.TryGetComponent(out Damageable damageable))
            {
                damageable.TakeDamage(damage, ActorManager.Instance.Player.gameObject, false);
            }

            Destroy(gameObject);
        }


        private Enemy2.Enemy FindClosestEnemy()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
            Enemy2.Enemy closest = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider col in hitColliders)
            {
                Enemy2.Enemy enemy = col.GetComponent<Enemy2.Enemy>();
                if (!enemy) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (!(distance < minDistance)) continue;
                minDistance = distance;
                closest = enemy;
            }

            return closest;
        }

        private void MoveTowardsEnemy()
        {
            if (!_closestEnemy) return;
            // Add 1 in the y-axis to make the fly fly a bit higher
            Vector3 targetPosition = _closestEnemy.transform.position + Vector3.up;
            Vector3 direction = (targetPosition - transform.position).normalized;
            _rigidbody.MovePosition(transform.position + direction * (moveSpeed * Time.deltaTime));
        }


        // Debug visualization in the Unity Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan; // Set color of the sphere
            Gizmos.DrawWireSphere(transform.position, searchRadius); // Draw the search area
        }
    }
}