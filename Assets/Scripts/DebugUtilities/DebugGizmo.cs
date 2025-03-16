using UnityEngine;

namespace Gameplay.DebugUtilities
{
    public class DebugGizmo : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.green;

        private void OnDrawGizmos()
        {
            // Set Gizmo color
            Gizmos.color = gizmoColor;

            // Draw a wireframe sphere at the GameObject's position
            Gizmos.DrawWireSphere(transform.position, 1.0f); // Radius of 1
        }
    }
}