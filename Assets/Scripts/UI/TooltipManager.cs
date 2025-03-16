using TMPro;
using UnityEngine;

namespace UI
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        private GameObject _tooltip;
        private TextMeshProUGUI _tooltipText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;


                _tooltip = transform.GetChild(0).gameObject;
                _tooltip.SetActive(false);
                _tooltipText = _tooltip.GetComponentInChildren<TextMeshProUGUI>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            _tooltip.transform.position = Input.mousePosition;
        }

        public void ShowTooltip(string text)
        {
            _tooltipText.text = text;
            _tooltip.SetActive(true);
        }

        public void HideTooltip()
        {
            _tooltip.SetActive(false);
        }
    }
}