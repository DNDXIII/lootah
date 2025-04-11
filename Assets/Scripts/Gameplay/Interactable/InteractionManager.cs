using Gameplay.Managers;
using Gameplay.Player;
using Managers;
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

        private void FixedUpdate()
        {
            // debug the ray 
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * interactionDistance, Color.red);

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit,
                    interactionDistance,
                    interactableLayer))
            {
                // TODO: Refactor this to use just the interface dude
                if (hit.collider.TryGetComponent(out Interactable interactable))
                {
                    InteractionTextManager.Instance.ShowInteractionText(interactable.GetInteractionText());

                    if (_playerInputHandler.GetInteractInput())
                    {
                        interactable.Interact();
                    }
                }
                else if (hit.collider.TryGetComponent(out IInteractable iinteractable))
                {
                    InteractionTextManager.Instance.ShowInteractionText(iinteractable.GetInteractionText());

                    if (_playerInputHandler.GetInteractInput())
                    {
                        iinteractable.Interact(ActorManager.Instance.Player);
                    }
                }
            }
            else
            {
                InteractionTextManager.Instance.HideInteractionText();
            }
        }
    }
}