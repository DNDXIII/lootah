using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private string interactionText = "Press E to interact";
        [SerializeField] private string interactionUnavailableText = "Can't interact";
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private UnityEvent onInteract;

        public string GetInteractionText()
        {
            return isInteractable ? interactionText : interactionUnavailableText;
        }

        public void Interact()
        {
            if (isInteractable)
            {
                onInteract.Invoke();
            }
        }
    }
}