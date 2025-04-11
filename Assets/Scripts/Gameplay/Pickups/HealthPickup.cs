using Gameplay.Player;
using Gameplay.Shared;
using UnityEngine;

namespace Gameplay.Pickups
{
    public class HealthPickup : BasePickup
    {
        [SerializeField] private int healAmount = 20;

        protected override void OnPickup(PlayerController playerController)
        {
            Health playerHealth = playerController.GetComponent<Health>();
            playerHealth.Heal(healAmount);
        }
    }
}