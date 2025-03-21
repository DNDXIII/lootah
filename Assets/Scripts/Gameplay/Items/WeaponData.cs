using System.Collections.Generic;
using System.Linq;
using Gameplay.Items.Modifiers;
using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
    public class WeaponData : ScriptableObject
    {
        // Static data
        public int id;
        public string itemName;
        public string description;
        public Sprite sprite;
        public GameObject prefab;

        // Base Stats
        public int damage;
        public float delayBetweenShots;
        public float bulletSpreadAngle;
        public int clipSize;
        public int bulletsPerShot;

        public List<BaseWeaponModifier> possibleModifiers = new();


        public WeaponItem GenerateWeaponItem()
        {
            WeaponItem weaponItem = new()
            {
                Id = id,
                Damage = damage,
                DelayBetweenShots = delayBetweenShots,
                BulletSpreadAngle = bulletSpreadAngle,
                ClipSize = clipSize,
                BulletsPerShot = bulletsPerShot
            };

            return possibleModifiers.Aggregate(weaponItem, (current, modifier) => modifier.Apply(current));
        }
    }
}