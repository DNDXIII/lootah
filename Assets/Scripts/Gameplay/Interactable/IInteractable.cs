using Gameplay.Player;

namespace Gameplay.Interactable
{
    public interface IInteractable
    {
        void Interact(PlayerController player);
        string GetInteractionText();
    }
}