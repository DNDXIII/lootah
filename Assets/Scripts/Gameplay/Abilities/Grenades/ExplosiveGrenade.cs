using Gameplay.Managers;
using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Abilities.Grenades
{
    [RequireComponent(typeof(Rigidbody))]
    public class ExplosiveGrenade : MonoBehaviour
    {
        [SerializeField] private ParticleSystem explosionParticle; // Explosion effect prefab
        [SerializeField] private float explosionRadius = 5f; // Damage radius
        [SerializeField] private LayerMask hitLayers; // Layers the grenade can hit
        [SerializeField] private float damage = 20f; // Damage applied to objects
        [SerializeField] private float fuseTime = 3f;

        private void Start()
        {
            Invoke(nameof(Explode), fuseTime);
        }

        private void Explode()
        {
            // Play the explosion effect
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.TryGetComponent(out Damageable damageable))
                {
                    damageable.TakeDamage(damage, ActorManager.Instance.Player.gameObject, false);
                }
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            // Draw the explosion radius in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}