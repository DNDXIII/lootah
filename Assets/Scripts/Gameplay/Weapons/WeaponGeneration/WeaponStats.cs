using System;

namespace Gameplay.Weapons.WeaponGeneration
{
    [Serializable]
    public class WeaponStats
    {
        public int damage;
        public float delayBetweenShots;
        public float bulletSpreadAngle;
        public int clipSize;
        public int bulletsPerShot;

        public override string ToString()
        {
            return
                $"Damage: {damage}\n" +
                $"Delay Between Shots: {delayBetweenShots}\n";
        }
    }
}