using System.Collections.Generic;
using Gameplay.Abilities;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerAbilityManager : MonoBehaviour
    {
        public List<AbstractPassiveAbility> abilities;

        private void Start()
        {
            foreach (var ability in abilities)
            {
                ability.Activate();
            }
        }


        // Add a new ability to the player
        public void AddAbility(AbstractPassiveAbility passiveAbility)
        {
            if (passiveAbility == null || abilities.Contains(passiveAbility)) return;

            abilities.Add(passiveAbility);
            passiveAbility.Activate();
        }

        // Remove an ability from the player
        public void RemoveAbility(AbstractPassiveAbility passiveAbility)
        {
            if (passiveAbility == null || !abilities.Contains(passiveAbility))
                return;

            passiveAbility.Deactivate();
            abilities.Remove(passiveAbility);
        }
    }
}