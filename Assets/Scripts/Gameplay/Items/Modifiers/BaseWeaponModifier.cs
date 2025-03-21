using UnityEngine;

namespace Gameplay.Items.Modifiers
{
    public abstract class BaseWeaponModifier : ScriptableObject
    {
        public abstract WeaponItem Apply(WeaponItem weaponItem);
    }
}