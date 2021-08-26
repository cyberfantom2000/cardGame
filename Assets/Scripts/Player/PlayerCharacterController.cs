using Unity.FPS;
using UnityEngine;

namespace Unity.FPS
{
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("References")][Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Header("General")][Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        public LayerMask GroundCheckLayers = -1;

        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        public float GroundCheckDistance = 0.05f;

        [Header("Movement")][Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float MaxSpeedOnGround = 10f;

        [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15;

        [Header("Rotation")][Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;

        [Header("Jump")][Tooltip("Force applied upward when jumping")]
        public float JumpForce = 9f;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }

        PlayerInputHandler m_inputHandler;
        CharacterController m_controller;
        Vector3 m_GroundNormal;
        Vector3 m_CharacterVelocity;
        float m_LastTimeJumped = 0f;
        float m_CameraVerticalAngle = 0f;

        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        // Start is called before the first frame update
        void Start()
        {
            m_controller = GetComponent<CharacterController>();
            m_inputHandler = GetComponent<PlayerInputHandler>();

            m_controller.enableOverlapRecovery = true;
        }

        // Update is called once per frame
        void Update()
        {
            bool wasGrounded = IsGrounded;
            GroundCheck();

            HandleCharacterMovement();
        }

        void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance = IsGrounded ? (m_controller.skinWidth + GroundCheckDistance) 
                                                         : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            m_GroundNormal = Vector3.up;

            if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_controller.height),
                    m_controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    m_GroundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(m_GroundNormal))
                    {
                        IsGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > m_controller.skinWidth)
                        {
                            m_controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }
    }
}
