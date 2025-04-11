using Gameplay.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WorldHealthDisplay : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private bool hideWhenFull = true;

        [Header("References")] [SerializeField] [Tooltip("The health bar GameObject that will be shown/hidden.")]
        private GameObject healthBarGameObject;

        [SerializeField] [Tooltip("The health bar image that will be filled based on the health ratio.")]
        private Image healthBarImage;

        [SerializeField] private Health health;

        private void Start()
        {
            health.OnHealed += _ => UpdateHealthBar();
            health.OnDamaged += (_, _) => UpdateHealthBar();
            health.OnDie += () => ToggleHealthBar(false);


            UpdateHealthBar();
            // Set the health bar to be active or inactive based on the current health ratio
            ToggleHealthBar(!hideWhenFull);
        }

        private void ToggleHealthBar(bool active)
        {
            healthBarGameObject.SetActive(active);
        }

        private void UpdateHealthBar()
        {
            healthBarImage.fillAmount = health.GetHealthRatio();

            if (hideWhenFull)
            {
                ToggleHealthBar(health.GetHealthRatio() < 1f);
            }
        }
    }
}