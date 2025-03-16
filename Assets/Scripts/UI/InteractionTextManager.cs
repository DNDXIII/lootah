using TMPro;
using UnityEngine;

namespace UI
{
    public class InteractionTextManager : MonoBehaviour
    {
        public static InteractionTextManager Instance { get; private set; }

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

        public void ShowInteractionText(string text)
        {
            _tooltipText.text = text;
            _tooltip.SetActive(true);
        }

        public void HideInteractionText()
        {
            _tooltip.SetActive(false);
        }
    }
}