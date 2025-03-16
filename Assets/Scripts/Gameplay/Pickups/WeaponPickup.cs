using Gameplay.Inventory;
using Gameplay.Items.Instances;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Pickups
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class WeaponPickup : BasePickup
    {
        [SerializeField] private WeaponItem item;

        protected override void OnPickup(PlayerController playerController)
        {
            PlayerInventoryManager.Instance.AddItemToInventory(item);
        }
    }
}