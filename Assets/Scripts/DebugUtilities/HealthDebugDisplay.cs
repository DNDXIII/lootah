using Gameplay.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace DebugUtilities
{
    public class HealthDebugDisplay : MonoBehaviour
    {
        [SerializeField] private float displayHeight = 1.5f;
        [SerializeField] private Vector2 size = new(1f, 0.1f);

        [Header("Health Bar Colors")] [SerializeField]
        private Color healthyColor = Color.green;

        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private Color backgroundColor = new(0.2f, 0.2f, 0.2f, 0.7f);


        // References
        private Health _health;
        private Canvas _canvas;
        private RectTransform _healthBarBg;
        private RectTransform _healthBarFill;

        private void Awake()
        {
            _health = GetComponent<Health>();

            if (_health == null)
            {
                Debug.LogError("HealthDebugDisplay requires a Health component on the same GameObject.");
                enabled = false;
                return;
            }

            CreateDebugDisplay();
        }

        private void CreateDebugDisplay()
        {
            // Create canvas
            GameObject canvasObj = new GameObject("HealthDebugCanvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.up * displayHeight;
            canvasObj.transform.rotation = Quaternion.identity;

            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;

            // Add canvas scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100f;

            // Add billboard component to always face camera
            canvasObj.AddComponent<Billboard>();

            // Create health bar background
            GameObject healthBarBg = new GameObject("HealthBarBg");
            healthBarBg.transform.SetParent(_canvas.transform);
            healthBarBg.transform.localPosition = Vector3.zero;
            healthBarBg.transform.localScale = Vector3.one;

            _healthBarBg = healthBarBg.AddComponent<RectTransform>();
            _healthBarBg.sizeDelta = size;

            UnityEngine.UI.Image bgImage = healthBarBg.AddComponent<Image>();
            bgImage.color = backgroundColor;

            // Create health bar fill
            GameObject healthBarFill = new GameObject("HealthBarFill");
            healthBarFill.transform.SetParent(_healthBarBg);
            healthBarFill.transform.localPosition = Vector3.zero;

            _healthBarFill = healthBarFill.AddComponent<RectTransform>();
            _healthBarFill.sizeDelta = size;
            _healthBarFill.anchorMin = Vector2.zero;
            _healthBarFill.anchorMax = Vector2.one;
            _healthBarFill.offsetMin = Vector2.zero;
            _healthBarFill.offsetMax = Vector2.zero;

            Image fillImage = healthBarFill.AddComponent<Image>();
            fillImage.color = healthyColor;

            // Subscribe to health events
            _health.OnDamaged += (_, _) => UpdateDisplay();
            _health.OnHealed += _ => UpdateDisplay();
            _health.OnDie += UpdateDisplay;

            // Initial update
            UpdateDisplay();
        }

        private void Update()
        {
            // Update in real-time (you could optimize this to only update when health changes)
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            float healthRatio = _health.GetHealthRatio();

            if (!_healthBarFill) return;
            // Update health bar fill
            _healthBarFill.anchorMax = new Vector2(healthRatio, 1);

            // Update color based on health percentage
            Image fillImage = _healthBarFill.GetComponent<Image>();
            if (fillImage)
            {
                fillImage.color = Color.Lerp(criticalColor, healthyColor, healthRatio);
            }
        }
    }
}