using UnityEngine;

namespace Gameplay.Items.Modifiers
{
    [CreateAssetMenu(fileName = "ClipSizeModifier", menuName = "Items/Modifiers/ClipSize")]
    public class AdditiveClipSizeModifier : BaseWeaponModifier
    {
        [SerializeField] private int minAdditiveValue;
        [SerializeField] private int maxAdditiveValue;

        public override WeaponItem Apply(WeaponItem weaponItem)
        {
            weaponItem.ClipSize += Random.Range(minAdditiveValue, maxAdditiveValue);
            return weaponItem;
        }
    }
}