using Gameplay.Player;
using InventoryPart3;
using UnityEngine;

namespace Gameplay.Pickups
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class WeaponPickup : BasePickup
    {
        [SerializeField] private WeaponItem item;

        protected override void OnPickup(PlayerController playerController) =>
            Debug.Log("TODO: Change to the new inventory thing");
        // PlayerInventoryManager.Instance.AddItemToInventory(item);
    }
}