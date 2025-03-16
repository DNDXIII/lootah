using Gameplay.Player;
using UI;
using UnityEngine;

namespace Gameplay.Interactable
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private LayerMask interactableLayer;

        private Camera _camera;
        private PlayerInputHandler _playerInputHandler;

        private void Awake()
        {
            _playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit,
                    interactionDistance,
                    interactableLayer) && hit.collider.TryGetComponent(out Interactable interactable))
            {
                InteractionTextManager.Instance.ShowInteractionText(interactable.GetInteractionText());

                if (_playerInputHandler.GetInteractInput())
                {
                    interactable.Interact();
                }
            }
            else
            {
                InteractionTextManager.Instance.HideInteractionText();
            }
        }
    }
}