using Gameplay.Interactable;
using Gameplay.Player;
using Managers;
using UnityEngine;

namespace Gameplay.Logic
{
    [RequireComponent(typeof(Collider))]
    public class KeyTerminal : MonoBehaviour, IInteractable
    {
        [SerializeField] private KeyType requiredKeyType;
        [SerializeField] private Door door;


        public void Interact(PlayerController player)
        {
            if (player.GetComponent<PlayerKeyManager>().HasKey(requiredKeyType) && door)
            {
                door.OpenDoor();
            }
        }

        public string GetInteractionText()
        {
            return ActorManager.Instance.Player.GetComponent<PlayerKeyManager>().HasKey(requiredKeyType)
                ? "Press E to open door"
                : "You need a key to open this door";
        }
    }
}