using UnityEngine;

namespace Gameplay.Items.Modifiers
{
    [CreateAssetMenu(fileName = "FireRateModifier", menuName = "Items/Modifiers/FireRate")]
    public class AdditiveFireRateModifier : BaseWeaponModifier
    {
        [SerializeField] private float minAdditiveValue;
        [SerializeField] private float maxAdditiveValue;

        public override WeaponItem Apply(WeaponItem weaponItem)
        {
                weaponItem.DelayBetweenShots -=
                    (float)System.Math.Round(Random.Range(minAdditiveValue, maxAdditiveValue), 2);
            return weaponItem;
        }
    }
}