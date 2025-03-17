using Gameplay.Shared;
using Managers;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Player
{
    [RequireComponent(typeof(CharacterController), typeof(RecoverablePlayerHealth), typeof(Damageable))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera playerCamera;

        [Header("General")] [Tooltip("Force applied downward when in the air")]
        public float gravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        public LayerMask groundCheckLayers = -1;

        [Tooltip("Distance from the bottom of the character controller capsule to test for grounded")]
        public float groundCheckDistance = 0.05f;

        [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float maxSpeedOnGround = 10f;

        [Tooltip(
            "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float movementSharpnessOnGround = 15;

        [Tooltip("Max movement speed when not grounded")]
        public float maxSpeedInAir = 10f;

        [Tooltip("Acceleration speed when in the air")]
        public float accelerationSpeedInAir = 25f;

        [Tooltip("Whether the player can sprint")]
        public bool canSprint = true;

        [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
        public float sprintSpeedModifier = 2f;

        [Tooltip("Height at which the player dies instantly when falling off the map")]
        public float killHeight = -50f;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float rotationSpeed = 200f;

        [Header("Jump")] [Tooltip("Force applied upward when jumping")]
        public float jumpForce = 9f;

        [Tooltip("SFX played when the player is hit")]
        public AudioClip onHitSfx;

        [Header("Jump")] [Tooltip("Max amount of jumps")]
        public int maxJumpCount = 2;

        [Header("Dash Settings")] [Tooltip("Speed of the dash")]
        public float dashSpeed = 30f;

        [Tooltip("Duration of the dash in seconds")]
        public float dashDuration = 0.2f;

        [Tooltip("Cooldown time before another dash can be used")]
        public float dashCooldown = 1f;

        [Tooltip("Dash sound effect")] public AudioClip dashSfx;

        private Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public RecoverablePlayerHealth Health { get; private set; }
        public Damageable Damageable { get; private set; }
        private PlayerInputHandler _playerInputHandler;
        private CharacterController _controller;

        private Vector3 _groundNormal;
        private float _lastTimeJumped;
        private float _cameraVerticalAngle;
        private float _footstepDistanceCounter;
        private float _currentJumpCount;

        private bool _isDashing;
        private float _dashStartTime;
        private float _lastDashTime;
        private Vector3 _dashDirection;

        private const float JumpGroundingPreventionTime = 0.2f;
        private const float GroundCheckDistanceInAir = 0.07f;

        private void Awake()
        {
            Application.targetFrameRate = 165;

            Health = GetComponent<RecoverablePlayerHealth>();
            Damageable = GetComponent<Damageable>();

            _controller = GetComponent<CharacterController>();
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _currentJumpCount = maxJumpCount;
        }

        private void Start()
        {
            _controller.enableOverlapRecovery = true;
            Health.OnDie += OnDie;
            Health.OnDamaged += OnDamaged;
        }

        private void OnDamaged(float damageAmount, GameObject damageSource)
        {
            PlayerDamageEvent evt = Events.PlayerDamageEvent;
            EventManager.Broadcast(evt);

            AudioUtility.CreateSfx(onHitSfx, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);
        }


        private void Update()
        {
            // check for Y kill
            if (!Health.IsDead && transform.position.y < killHeight)
            {
                Health.Kill();
            }

            // TODO Move to the input handler
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= _lastDashTime + dashCooldown)
            {
                StartDash();
            }

            GroundCheck();

            HandleCameraRotation();

            HandleCharacterMovement();
        }

        private void OnDie()
        {
            //EventManager.Broadcast(Events.PlayerDeathEvent);
            // reload current level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void StartDash()
        {
            // Get movement input direction
            Vector3 moveInput = _playerInputHandler.GetMoveInput();

            _dashDirection = moveInput.sqrMagnitude > 0f
                ? transform.TransformVector(moveInput).normalized
                : transform.forward; // Fallback to facing direction if no input
            // Ensure there's movement input
            AudioUtility.CreateSfx(dashSfx, transform.position, AudioUtility.AudioGroups.PlayerMovement, 0f);
            _isDashing = true;
            _dashStartTime = Time.time;
            _lastDashTime = Time.time;
            Health.SetInvincibility(true);
        }

        private void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance =
                IsGrounded ? (_controller.skinWidth + groundCheckDistance) : GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            _groundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= _lastTimeJumped + JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(_controller.height),
                        _controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance,
                        groundCheckLayers,
                        QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    _groundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(_groundNormal))
                    {
                        IsGrounded = true;
                        _currentJumpCount = maxJumpCount;

                        // handle snapping to the ground
                        if (hit.distance > _controller.skinWidth)
                        {
                            _controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }


        private void HandleCharacterMovement()
        {
            if (_isDashing)
            {
                // Move the player in the current movement direction at a fixed dash speed
                CharacterVelocity = _dashDirection * dashSpeed;

                // Stop dashing after dashDuration expires
                if (Time.time >= _dashStartTime + dashDuration)
                {
                    _isDashing = false;
                    Health.SetInvincibility(false);
                }

                _controller.Move(CharacterVelocity * Time.deltaTime);

                return;
            }


            // character movement handling
            bool isSprinting = _playerInputHandler.GetSprintInputHeld() && canSprint;
            {
                float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                Vector3 worldspaceMoveInput = transform.TransformVector(_playerInputHandler.GetMoveInput());

                // jumping
                if ((IsGrounded || _currentJumpCount > 0) && _playerInputHandler.GetJumpInputDown())
                {
                    // start by canceling out the vertical component of our velocity
                    CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                    // then, add the jumpSpeed value upwards
                    CharacterVelocity += Vector3.up * jumpForce;

                    // remember last time we jumped because we need to prevent snapping to ground for a short time
                    _lastTimeJumped = Time.time;

                    // decrement jump count
                    _currentJumpCount--;

                    // Force grounding to false
                    IsGrounded = false;
                    _groundNormal = Vector3.up;
                }
                // handle grounded movement
                else if (IsGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldspaceMoveInput * (maxSpeedOnGround * speedModifier);
                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, _groundNormal) *
                                     targetVelocity.magnitude;

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                        movementSharpnessOnGround * Time.deltaTime);
                }
                // handle air movement
                else
                {
                    // add air acceleration
                    CharacterVelocity += worldspaceMoveInput * (accelerationSpeedInAir * Time.deltaTime);

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    horizontalVelocity =
                        Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * sprintSpeedModifier);
                    CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    if (!_isDashing)
                    {
                        CharacterVelocity += Vector3.down * (gravityDownForce * Time.deltaTime);
                    }
                }
            }

            _controller.Move(CharacterVelocity * Time.deltaTime);
        }

        private void HandleCameraRotation()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f,
                        (_playerInputHandler.GetLookInputsHorizontal() * rotationSpeed),
                        0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                _cameraVerticalAngle +=
                    _playerInputHandler.GetLookInputsVertical() * rotationSpeed;

                // limit the camera's vertical angle to min/max
                _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
            }
        }

        // Gets a reoriented direction that is tangent to a given slope
        private Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }


        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        private bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= _controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        private Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * _controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        private Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - _controller.radius));
        }

        // Show an arrow pointing forward from the character
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward);
        }
    }
}