using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class AbstractPassiveAbility: ScriptableObject
    {
        public abstract void Activate();

        public abstract void Deactivate();
    }
}