using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Gameplay.Shared;
using UnityEngine;

namespace Gameplay.Pickups
{
    public class HealthPickup : BasePickup
    {
        [SerializeField] private int healAmount = 20;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float pickupDistance = 1.5f;

        protected override void OnPickup(PlayerController playerController)
        {
            Health playerHealth = playerController.GetComponent<Health>();

            MoveToPlayerAndHeal(playerController, playerHealth).Forget();
        }

        private async UniTaskVoid MoveToPlayerAndHeal(PlayerController playerController, Health playerHealth)
        {
            while (true)
            {
                Vector3 targetPosition = playerController.transform.position + Vector3.up;
                float distance = Vector3.Distance(transform.position, targetPosition);

                if (distance <= pickupDistance)
                {
                    break;
                }

                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

                await UniTask.Yield();
            }

            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }

        protected override bool CanPickup(PlayerController playerController)
        {
            Health playerHealth = playerController.GetComponent<Health>();
            return playerHealth != null && playerHealth.GetHealthRatio() < 1f;
        }
    }
}