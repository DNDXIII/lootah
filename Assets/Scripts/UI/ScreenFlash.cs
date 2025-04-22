using Gameplay.Shared;
using Managers;
using Shared;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScreenFlash : MonoBehaviour
    {
        public Image flashImage; // Assign the ScreenFlash image in the Inspector
        public float flashDuration = 0.5f; // How long the flash lasts

        private readonly Color _flashColor = new(1, 0, 0, 1); // Red with full opacity
        private readonly Color _transparentColor = new(1, 0, 0, 0); // Red with zero opacity
        private float _flashTimer;

        private void Start()
        {
            if (flashImage != null)
            {
                flashImage.color = _transparentColor; // Ensure it's initially transparent
            }

            EventManager.AddListener<PlayerDamageEvent>(OnPlayerDamaged);
        }


        private void OnPlayerDamaged(PlayerDamageEvent obj)
        {
            TriggerFlash();
        }

        private void Update()
        {
            if (_flashTimer > 0)
            {
                _flashTimer -= Time.deltaTime;
                float t = _flashTimer / flashDuration;
                flashImage.color = Color.Lerp(_transparentColor, _flashColor, t); // Fade in/out
            }
            else if (flashImage.color != _transparentColor)
            {
                flashImage.color = _transparentColor; // Reset to transparent
            }
        }

        private void TriggerFlash()
        {
            _flashTimer = flashDuration; // Start the flash
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<PlayerDamageEvent>(OnPlayerDamaged);
        }
    }
}