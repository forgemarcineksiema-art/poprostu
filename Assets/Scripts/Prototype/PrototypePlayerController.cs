using UnityEngine;

namespace ValleDePlata.Prototype
{
    [RequireComponent(typeof(PrototypeCharacterMotor))]
    public sealed class PrototypePlayerController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float walkSpeed = 4.2f;
        [SerializeField] private float sprintSpeed = 6.4f;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float deceleration = 22f;
        [SerializeField] private float turnSharpness = 14f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float interactRadius = 2.25f;
        [SerializeField] private LayerMask interactMask = ~0;

        private PrototypeCharacterMotor characterMotor;
        private PrototypeCameraRig cameraRig;
        private PrototypeVehicleController currentVehicle;
        private bool hasInteractionTarget;
        private PrototypeInteractionCandidate currentInteractionTarget;

        public bool IsDriving => currentVehicle != null;
        public Transform CameraPivot => currentVehicle != null ? currentVehicle.CameraPivot : cameraPivot;
        public bool HasInteractionFocus
        {
            get
            {
                if (currentVehicle != null || !isActiveAndEnabled)
                {
                    return false;
                }

                RefreshInteractionTarget();
                return hasInteractionTarget && !currentInteractionTarget.Blocked;
            }
        }

        public void EnterVehicle(PrototypeVehicleController vehicle)
        {
            if (vehicle == null || currentVehicle != null)
            {
                return;
            }

            EnsureInitialized();
            currentVehicle = vehicle;
            characterMotor.ResetVelocity();
            characterMotor.enabled = false;
            gameObject.SetActive(false);
            vehicle.Enter(this);
            PrototypeRunMetrics.Active?.RecordVehicleEnter();
            PrototypeDebugState.Mode = "Driving";
        }

        public void ExitVehicle(Vector3 position, Quaternion rotation)
        {
            EnsureInitialized();
            transform.SetPositionAndRotation(position, rotation);
            gameObject.SetActive(true);
            characterMotor.enabled = true;
            currentVehicle = null;
            characterMotor.ResetVelocity();
            PrototypeRunMetrics.Active?.RecordVehicleExit();
            PrototypeDebugState.Mode = "OnFoot";
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (characterMotor == null)
            {
                characterMotor = GetComponent<PrototypeCharacterMotor>();
            }

            if (cameraPivot == null)
            {
                cameraPivot = transform;
            }

            if (cameraRig == null)
            {
                cameraRig = FindAnyObjectByType<PrototypeCameraRig>();
            }
        }

        private void Update()
        {
            if (currentVehicle != null)
            {
                return;
            }

            UpdateMovement();
            UpdateInteraction();
        }

        private void UpdateMovement()
        {
            ApplyMovement(PrototypeInput.Move, PrototypeInput.SprintHeld, Time.deltaTime);
        }

        public void ApplyMovementForTests(Vector2 moveInput, bool sprintHeld, float deltaTime)
        {
            EnsureInitialized();
            ApplyMovement(moveInput, sprintHeld, deltaTime);
        }

        public static Vector3 BuildCameraRelativeMove(Vector2 moveInput, Vector3 planarForward, Vector3 planarRight)
        {
            var cameraForward = Vector3.ProjectOnPlane(planarForward, Vector3.up);
            var cameraRight = Vector3.ProjectOnPlane(planarRight, Vector3.up);

            if (cameraForward.sqrMagnitude <= 0.001f)
            {
                cameraForward = Vector3.forward;
            }

            if (cameraRight.sqrMagnitude <= 0.001f)
            {
                cameraRight = Vector3.right;
            }

            var desiredMove = cameraForward.normalized * moveInput.y + cameraRight.normalized * moveInput.x;
            return Vector3.ClampMagnitude(desiredMove, 1f);
        }

        private void ApplyMovement(Vector2 moveInput, bool sprintHeld, float deltaTime)
        {
            var cameraForward = cameraRig != null ? cameraRig.PlanarForward : Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
            var cameraRight = cameraRig != null ? cameraRig.PlanarRight : Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;
            var desiredMove = BuildCameraRelativeMove(moveInput, cameraForward, cameraRight);
            var speed = sprintHeld ? sprintSpeed : walkSpeed;
            characterMotor.Move(desiredMove, speed, acceleration, deceleration, gravity, turnSharpness, deltaTime);

            PrototypeDebugState.Speed = characterMotor.HorizontalVelocity.magnitude;
            PrototypeDebugState.Focus = "On foot";
        }

        private void UpdateInteraction()
        {
            RefreshInteractionTarget();

            if (!hasInteractionTarget || currentInteractionTarget.Blocked || !PrototypeInput.InteractPressedThisFrame)
            {
                return;
            }

            if (currentInteractionTarget.Kind == PrototypeInteractionKind.Vehicle)
            {
                var vehicle = currentInteractionTarget.Component as PrototypeVehicleController;
                if (vehicle != null)
                {
                    EnterVehicle(vehicle);
                }

                return;
            }

            var interactable = currentInteractionTarget.Component as PrototypeInteractable;
            if (interactable != null)
            {
                interactable.Interact();
            }
        }

        private void RefreshInteractionTarget()
        {
            var queryMask = PrototypeLayers.InteractionQueryMask != 0 ? PrototypeLayers.InteractionQueryMask : interactMask.value;
            hasInteractionTarget = PrototypeInteractionTargeting.TryFindBest(
                transform.position,
                transform.forward,
                interactRadius,
                queryMask,
                PrototypeLayers.WorldCollisionMask,
                out currentInteractionTarget);

            if (!hasInteractionTarget)
            {
                PrototypeDebugState.Interaction = "None";
                return;
            }

            PrototypeDebugState.Interaction = currentInteractionTarget.Blocked
                ? "Interaction blocked"
                : currentInteractionTarget.Prompt;
        }
    }
}
