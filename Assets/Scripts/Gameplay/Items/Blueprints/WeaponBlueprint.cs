using Gameplay.Items.Instances;
using Gameplay.Weapons;
using Gameplay.Weapons.WeaponGeneration;
using UnityEngine;

namespace Gameplay.Items.Blueprints
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Blueprints/Weapon")]
    public class WeaponBlueprint : BaseItemBlueprint
    {
        public string weaponName;
        public WeaponType weaponType;
        public WeaponRarity rarity;

        public WeaponController weaponPrefab;
        public RarityConfig statsConfig;

        [System.Serializable]
        public class RarityConfig
        {
            [Header("Damage Settings")] [Range(0, 100)]
            public int minDamage = 1;

            [Range(0, 100)] public int maxDamage = 100;

            [Header("Delay Between Shots Settings")] [Range(0.1f, 2f)]
            public float minDelayBetweenShots = 0.1f;

            [Range(0.1f, 2f)] public float maxDelayBetweenShots = 2f;

            [Header("Bullet Spread Angle Settings")] [Range(0, 10)]
            public float minBulletSpreadAngle;

            [Range(0, 10)] public float maxBulletSpreadAngle = 10f;

            [Header("Clip Size Settings")] [Range(1, 300)]
            public int minClipSize = 1;

            [Range(1, 300)] public int maxClipSize = 300;

            [Header("Bullets Per Shot Settings")] [Range(1, 10)]
            public int minBulletsPerShot = 1;

            [Range(1, 10)] public int maxBulletsPerShot = 10;
        }

        public override BaseItem GenerateItem()
        {
            var newStats = new WeaponStats
            {
                damage = Random.Range(statsConfig.minDamage, statsConfig.maxDamage),
                delayBetweenShots = Random.Range(statsConfig.minDelayBetweenShots, statsConfig.maxDelayBetweenShots),
                bulletSpreadAngle = Random.Range(statsConfig.minBulletSpreadAngle, statsConfig.maxBulletSpreadAngle),
                clipSize = Random.Range(statsConfig.minClipSize, statsConfig.maxClipSize),
                bulletsPerShot = Random.Range(statsConfig.minBulletsPerShot, statsConfig.maxBulletsPerShot)
            };

            return new WeaponItem(this, newStats);
        }
    }
}