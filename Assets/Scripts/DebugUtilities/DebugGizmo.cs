using UnityEngine;

namespace DebugUtilities
{
    public class DebugGizmo : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.green;
        [SerializeField] private float gizmoSize = 1f;

        private void OnDrawGizmos()
        {
            // Set Gizmo color
            Gizmos.color = gizmoColor;

            // Draw a wireframe sphere at the GameObject's position
            Gizmos.DrawWireSphere(transform.position, gizmoSize);
        }
    }
}