using TMPro;
using UnityEngine;

namespace UI
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        [SerializeField] private RectTransform tooltip;
        private TextMeshProUGUI _tooltipText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;


                tooltip.gameObject.SetActive(false);
                _tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LateUpdate()
        {
            // Offset the tooltip to the right and below the mouse. Use the size of the tooltip to offset it correctly
            Vector3 offset = new Vector3(tooltip.rect.width / 2, -1 * (tooltip.rect.height / 2), 0f);

            tooltip.transform.position = Input.mousePosition + offset;
        }

        public void ShowTooltip(string text)
        {
            _tooltipText.text = text;
            tooltip.gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}