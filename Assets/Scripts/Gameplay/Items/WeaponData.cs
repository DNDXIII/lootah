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
            var rarity = GetRandomRarity();

            WeaponItem weaponItem = new()
            {
                Id = id,
                Damage = damage,
                DelayBetweenShots = delayBetweenShots,
                BulletSpreadAngle = bulletSpreadAngle,
                ClipSize = clipSize,
                BulletsPerShot = bulletsPerShot,
                Rarity = rarity
            };

            var maxModifierCount = GetMaxModifierCount(rarity);

            return possibleModifiers.OrderBy(x => Random.value).ToList()
                .GetRange(0, Mathf.Min(maxModifierCount, possibleModifiers.Count))
                .Aggregate(weaponItem, (current, modifier) =>
                {
                    Debug.Log("Applying modifier: " + modifier.name);
                    return modifier.Apply(current);
                });
        }


        // TODO: For now this just a static method that returns a random rarity. Implement a proper rarity system
        private static WeaponRarity GetRandomRarity()
        {
            var randomValue = Random.Range(0, 100);

            return randomValue switch
            {
                < 50 => WeaponRarity.Common,
                < 80 => WeaponRarity.Uncommon,
                < 95 => WeaponRarity.Rare,
                _ => WeaponRarity.Legendary
            };
        }

        private static int GetMaxModifierCount(WeaponRarity rarity)
        {
            return rarity switch
            {
                WeaponRarity.Common => 0,
                WeaponRarity.Uncommon => 2,
                WeaponRarity.Rare => 4,
                WeaponRarity.Legendary => 6,
                _ => 0
            };
        }
    }
}