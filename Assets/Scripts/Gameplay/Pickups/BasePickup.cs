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

            if (!CanPickup(pickingPlayer))
                return;

            if (onPickupSound)
            {
                AudioUtility.CreateSfx(onPickupSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
            }

            OnPickup(pickingPlayer);
        }

        protected abstract void OnPickup(PlayerController playerController);

        protected virtual bool CanPickup(PlayerController playerController)
        {
            // This method can be overridden to add additional checks before allowing the pickup
            return true;
        }
    }
}