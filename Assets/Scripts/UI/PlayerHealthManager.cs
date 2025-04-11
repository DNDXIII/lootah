using Gameplay.Managers;
using Gameplay.Player;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerHealthManager : MonoBehaviour
    {
        [SerializeField] private Slider redBar; // Represents actual health
        [SerializeField] private Slider yellowBar; // Represents recoverable health

        private RecoverablePlayerHealth _playerHealth;

        private void Start()
        {
            _playerHealth = ActorManager.Instance.Player.Health;
        }

        private void Update()
        {
            redBar.value = _playerHealth.GetHealthRatio();
            yellowBar.value = _playerHealth.GetHealthAndRecoverableHealthRatio();
        }
    }
}