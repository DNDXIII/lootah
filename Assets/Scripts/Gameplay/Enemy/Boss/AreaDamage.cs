using Gameplay.Shared;
using UnityEngine;

namespace Gameplay.Enemy.Boss
{
    [RequireComponent(typeof(Collider))]
    public class AreaDamage : MonoBehaviour
    {
        [SerializeField] private GameObject warningEffect;
        [SerializeField] private GameObject damageEffect;

        private Collider _areaCollider;
        private GameObject _owner;
        private float _damage = 30f;


        private void Awake()
        {
            _areaCollider = GetComponent<Collider>();
            _areaCollider.enabled = false;
            _areaCollider.isTrigger = true;

            damageEffect.SetActive(false);
            warningEffect.SetActive(false);
        }


        public void Init(GameObject owner, float damage)
        {
            _damage = damage;
            _owner = owner;
        }

        public void EnableWarningEffect()
        {
            warningEffect.SetActive(true);
        }

        public void DisableWarningEffect()
        {
            warningEffect.SetActive(false);
        }

        public void EnableAreaCollider()
        {
            damageEffect.SetActive(true);
            _areaCollider.enabled = true;
        }

        public void DisableAreaCollider()
        {
            damageEffect.SetActive(false);
            _areaCollider.enabled = false;
        }

        // Damage the player if they are in the area while it is active
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<Damageable>().TakeDamage(_damage, _owner, false);
            // Disable the collider after dealing damage, so the player doesn't take damage multiple times
            _areaCollider.enabled = false;
        }
    }
}