namespace Gameplay.Player
{
    using UnityEngine;

    public class ViewModelLag : MonoBehaviour
    {
        [Header("References")] public Transform cameraTransform;


        [Header("Position Spring Settings")] public float positionSpringStrength = 100f;
        public float positionDamping = 15f;
        public float maxOffset = 0.15f;
        public float positionLagMultiplier = 0.005f;

        [Header("Rotation Spring Settings")] public float rotationSpringStrength = 200f;
        public float rotationDamping = 20f;
        public float maxRotation = 10f;
        public float rotationLagMultiplier = 0.5f;

        private Vector3 positionOffset;
        private Vector3 positionVelocity;

        private Vector3 rotationOffset;
        private Vector3 rotationVelocity;

        private Vector3 prevCamPos;
        private Quaternion prevCamRot;

        private void Start()
        {
            prevCamPos = cameraTransform.position;
            prevCamRot = cameraTransform.rotation;
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f) return;

            // --- POSITION SPRING BASED ON CAMERA VELOCITY (INVERSE LAG) ---
            Vector3 camVelocity = (cameraTransform.position - prevCamPos) / deltaTime;

            // Convert world-space velocity to local space
            Vector3 localCamVelocity = -cameraTransform.InverseTransformDirection(camVelocity);

            // Reverse direction to simulate lag
            Vector3 targetOffset = localCamVelocity * positionLagMultiplier;

            Vector3 springForce = (targetOffset - positionOffset) * positionSpringStrength;
            Vector3 dampingForce = -positionVelocity * positionDamping;
            Vector3 force = springForce + dampingForce;

            positionVelocity += force * deltaTime;
            positionOffset += positionVelocity * deltaTime;

            // Clamp max sway
            positionOffset = Vector3.ClampMagnitude(positionOffset, maxOffset);

            // --- ROTATION SPRING BASED ON ROTATION DELTA ---
            Quaternion deltaRot = cameraTransform.rotation * Quaternion.Inverse(prevCamRot);
            deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            Vector3 angularVelocity = axis * (angle * Mathf.Deg2Rad) / deltaTime;
            Vector3 targetRotOffset = -angularVelocity * rotationLagMultiplier;

            Vector3 rotSpringForce = (targetRotOffset - rotationOffset) * rotationSpringStrength;
            Vector3 rotDampingForce = -rotationVelocity * rotationDamping;
            Vector3 rotForce = rotSpringForce + rotDampingForce;

            rotationVelocity += rotForce * deltaTime;
            rotationOffset += rotationVelocity * deltaTime;

            // Clamp max rotation
            rotationOffset = Vector3.ClampMagnitude(rotationOffset, maxRotation);

            // Apply to viewmodel
            transform.localPosition = positionOffset;
            transform.localRotation = Quaternion.Euler(rotationOffset);

            // Store for next frame
            prevCamPos = cameraTransform.position;
            prevCamRot = cameraTransform.rotation;
        }
    }
}