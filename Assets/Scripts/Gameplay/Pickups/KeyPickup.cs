using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Pickups
{
    public class KeyPickup : BasePickup
    {
        [SerializeField] private KeyType keyType;

        protected override void OnPickup(PlayerController playerController)
        {
            playerController.GetComponent<PlayerKeyManager>().AddKey(keyType);
            Destroy(gameObject);
        }
    }
}