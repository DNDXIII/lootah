using Gameplay.Inventory;
using Gameplay.Items;
using Gameplay.Player;
using Managers;
using UnityEngine;

namespace Gameplay.Pickups
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private AudioClip pickupSound;
        private WeaponItem _weaponItem;

        public void SetWeapon(WeaponItem weapon) => _weaponItem = weapon;

        public void OnPickup()
        {
            if (pickupSound)
            {
                AudioUtility.CreateSfx(pickupSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
            }

            ActorManager.Instance.Player.GetComponent<PlayerInventory>().AddWeapon(_weaponItem);
            Destroy(gameObject);
        }
    }
}