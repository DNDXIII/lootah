using Gameplay.Player;
using Managers;
using UnityEngine;

namespace Gameplay.Pickups
{
    [RequireComponent(typeof(Collider))]
    public abstract class BasePickup : MonoBehaviour
    {
        [SerializeField] private AudioClip onPickupSound;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            PlayerController pickingPlayer = other.GetComponent<PlayerController>();

            if (onPickupSound)
            {
                AudioUtility.CreateSfx(onPickupSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
            }

            OnPickup(pickingPlayer);
            Destroy(gameObject);
        }

        protected abstract void OnPickup(PlayerController playerController);
    }
}