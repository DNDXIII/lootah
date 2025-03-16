using Gameplay.Items.Blueprints;
using Gameplay.Weapons.WeaponGeneration;

namespace Gameplay.Items.Instances
{
    [System.Serializable]
    public class WeaponItem : BaseItem
    {
        public string weaponName;
        public WeaponType weaponType;
        public WeaponRarity rarity;

        public WeaponStats weaponStats;

        public WeaponItem(WeaponBlueprint weaponBlueprint, WeaponStats weaponStats) : base(weaponBlueprint)
        {
            weaponName = weaponBlueprint.weaponName;
            weaponType = weaponBlueprint.weaponType;
            rarity = weaponBlueprint.rarity;
            this.weaponStats = weaponStats;
        }

        public override string GetDescription()
        {
            return $"{rarity}\n" +
                   $"Damage: {weaponStats.damage}\n" +
                   $"Clip Size: {weaponStats.clipSize}\n" +
                   $"Bullets Per Shot: {weaponStats.bulletsPerShot}";
        }
    }
}