using System;
using UnityEngine;

namespace Gameplay.Shared
{
    public class RangeDetector : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private LayerMask detectionLayerMask;
        [SerializeField] private bool showDebugGizmos = true;

        public GameObject DetectedTarget { get; private set; }

        private readonly Collider[] results = new Collider[1];

        private void FixedUpdate()
        {
            UpdateDetection();
        }


        public GameObject UpdateDetection()
        {
            var numberDetected =
                Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, detectionLayerMask);

            if (numberDetected == 0)
            {
                DetectedTarget = null;
                return DetectedTarget;
            }

            DetectedTarget = results[0].gameObject;
            return DetectedTarget;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos || enabled == false) return;

            Gizmos.color = DetectedTarget != null ? Color.green : Color.red;

            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}