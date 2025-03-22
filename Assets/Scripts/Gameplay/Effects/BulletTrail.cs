using UnityEngine;

namespace Gameplay.Effects
{
    [RequireComponent(typeof(LineRenderer))]
    public class BulletTrail : MonoBehaviour
    {
        [SerializeField] private float trailDuration;
        [SerializeField] private float startWidth;


        private LineRenderer _lineRenderer;
        private float _elapsedTime;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Initialize(Vector3 start, Vector3 end)
        {
            // Set initial properties
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
            _lineRenderer.startWidth = startWidth;
            _lineRenderer.endWidth = startWidth * 0.3f;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / trailDuration); // Normalize time

            // Shrink width over time
            float newWidth = Mathf.Lerp(startWidth, 0f, t);
            _lineRenderer.startWidth = newWidth;
            _lineRenderer.endWidth = newWidth * 0.3f;

            // Destroy when fully faded
            if (_elapsedTime >= trailDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}