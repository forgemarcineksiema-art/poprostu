using UnityEngine;

namespace ValleDePlata.Prototype
{
    [RequireComponent(typeof(CharacterController))]
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

        private CharacterController characterController;
        private PrototypeCameraRig cameraRig;
        private PrototypeVehicleController currentVehicle;
        private Vector3 horizontalVelocity;
        private Vector3 verticalVelocity;

        public bool IsDriving => currentVehicle != null;
        public Transform CameraPivot => currentVehicle != null ? currentVehicle.CameraPivot : cameraPivot;

        public void EnterVehicle(PrototypeVehicleController vehicle)
        {
            if (vehicle == null || currentVehicle != null)
            {
                return;
            }

            EnsureInitialized();
            currentVehicle = vehicle;
            horizontalVelocity = Vector3.zero;
            verticalVelocity = Vector3.zero;
            characterController.enabled = false;
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
            characterController.enabled = true;
            currentVehicle = null;
            horizontalVelocity = Vector3.zero;
            verticalVelocity = Vector3.zero;
            PrototypeRunMetrics.Active?.RecordVehicleExit();
            PrototypeDebugState.Mode = "OnFoot";
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
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

            if (desiredMove.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(desiredMove, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSharpness * deltaTime);
            }

            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -1f;
            }

            verticalVelocity.y += gravity * deltaTime;

            var speed = sprintHeld ? sprintSpeed : walkSpeed;
            var targetHorizontal = desiredMove * speed;
            var rate = desiredMove.sqrMagnitude > 0.01f ? acceleration : deceleration;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetHorizontal, rate * deltaTime);
            characterController.Move((horizontalVelocity + verticalVelocity) * deltaTime);

            PrototypeDebugState.Speed = horizontalVelocity.magnitude;
            PrototypeDebugState.Focus = "On foot";
        }

        private void UpdateInteraction()
        {
            var nearestVehicle = FindNearest<PrototypeVehicleController>(interactRadius);
            var nearestInteractable = FindNearest<PrototypeInteractable>(interactRadius);

            if (nearestVehicle != null)
            {
                PrototypeDebugState.Interaction = "E / South Button: enter car";
                if (PrototypeInput.InteractPressedThisFrame)
                {
                    EnterVehicle(nearestVehicle);
                }

                return;
            }

            if (nearestInteractable != null)
            {
                PrototypeDebugState.Interaction = "E / South Button: " + nearestInteractable.Prompt;
                if (PrototypeInput.InteractPressedThisFrame)
                {
                    nearestInteractable.Interact();
                }

                return;
            }

            PrototypeDebugState.Interaction = "None";
        }

        private T FindNearest<T>(float radius) where T : Component
        {
            var hits = Physics.OverlapSphere(transform.position, radius, interactMask, QueryTriggerInteraction.Collide);
            T nearest = null;
            var nearestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                var candidate = hit.GetComponentInParent<T>();
                if (candidate == null)
                {
                    continue;
                }

                var distance = Vector3.SqrMagnitude(candidate.transform.position - transform.position);
                if (distance < nearestDistance)
                {
                    nearest = candidate;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }
    }
}
