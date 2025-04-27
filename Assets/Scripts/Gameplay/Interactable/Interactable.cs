using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionText = "Press E to interact";
        [SerializeField] private string interactionUnavailableText = "Can't interact";
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private UnityEvent onInteract;
        [SerializeField] private bool singleUse = true;

        private bool _hasBeenUsed;

        public void Interact(PlayerController player)
        {
            if (!isInteractable || singleUse && _hasBeenUsed) return;
            _hasBeenUsed = true;
            onInteract.Invoke();        }

        public string GetInteractionText()
        {
            return isInteractable ? interactionText : interactionUnavailableText;
        }
    }
}