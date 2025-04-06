using Gameplay.Player;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DashManager : MonoBehaviour
    {
        [SerializeField] private Image firstDashImage;
        [SerializeField] private Image secondDashImage;

        [SerializeField] private Color activeColor = Color.red;
        [SerializeField] private Color inactiveColor = Color.gray;

        private PlayerController _playerController;

        private void Start()
        {
            _playerController = ActorManager.Instance.Player;
        }

        private void Update()
        {
            var currentDashes = _playerController.AvailableDashCount;

            switch (currentDashes)
            {
                case 2:
                    firstDashImage.color = activeColor;
                    secondDashImage.color = activeColor;
                    break;
                case 1:
                    firstDashImage.color = activeColor;
                    secondDashImage.color = inactiveColor;
                    break;
                default:
                    firstDashImage.color = inactiveColor;
                    secondDashImage.color = inactiveColor;
                    break;
            }
        }
    }
}