using UnityEngine;

namespace Gameplay.Shared
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cameraLocation;

        private void Start()
        {
            if (Camera.main != null) _cameraLocation = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (!_cameraLocation) return;
            Vector3 direction = _cameraLocation.position - transform.position;
            direction.y = 0; // Keep only horizontal rotation
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}