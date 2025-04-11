using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Shared
{
    public class LineOfSightDetector : MonoBehaviour
    {
        [FormerlySerializedAs("detectionRadius")]
        [Tooltip("Radius within which the enemy detects the player.")]
        [SerializeField]
        private float maxDetectionDistance = 30f;

        [Tooltip("Transform from which the enemy will detect the player.")] [SerializeField]
        private Transform detectionTransform;

        [Tooltip("Layer mask for the detection raycast.")] [SerializeField]
        private LayerMask detectionLayerMask;

        [Tooltip("Time interval for updating the enemy's line of sight.")] [SerializeField]
        private float losUpdateInterval = 0.2f;

        private float _lastLosUpdate = float.MinValue;
        private bool _lastLosResult;

        public bool PerformDetection(Vector3 transformPosition)
        {
            // Only update the line of sight every 0.5 seconds
            if (!(_lastLosUpdate + losUpdateInterval < Time.time)) return _lastLosResult;
            _lastLosUpdate = Time.time;

            var playerPosition = transformPosition + Vector3.up;
            // Draw the line in debug mode
            var detectionOrigin = detectionTransform ? detectionTransform.position : transform.position;

            _lastLosResult = Physics.Raycast(detectionOrigin,
                                 playerPosition - detectionOrigin, out var hit, maxDetectionDistance,
                                 detectionLayerMask) &&
                             hit.collider.CompareTag("Player");


            // Debug line
            Debug.DrawLine(detectionOrigin, playerPosition, Color.red, 1f);

            return _lastLosResult;
        }

      
    }
}