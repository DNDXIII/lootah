using UnityEngine;

namespace Gameplay.Items.Modifiers
{
    [CreateAssetMenu(fileName = "DamageModifier", menuName = "Items/Modifiers/Damage")]
    public class AdditiveDamageModifier : BaseWeaponModifier
    {
        [SerializeField] private int minAdditiveDamage;
        [SerializeField] private int maxAdditiveDamage;

        public override WeaponItem Apply(WeaponItem weaponItem)
        {
            weaponItem.Damage += Random.Range(minAdditiveDamage, maxAdditiveDamage);
            return weaponItem;
        }
    }
}