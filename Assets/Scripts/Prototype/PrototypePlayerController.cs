using UnityEngine;

namespace ValleDePlata.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypePlayerController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float walkSpeed = 4.2f;
        [SerializeField] private float sprintSpeed = 6.4f;
        [SerializeField] private float turnSharpness = 14f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float interactRadius = 2.25f;
        [SerializeField] private LayerMask interactMask = ~0;

        private CharacterController characterController;
        private PrototypeVehicleController currentVehicle;
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
            var moveInput = PrototypeInput.Move;
            var cameraForward = Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
            var cameraRight = Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;
            var desiredMove = cameraForward * moveInput.y + cameraRight * moveInput.x;

            if (desiredMove.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(desiredMove, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSharpness * Time.deltaTime);
            }

            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -1f;
            }

            verticalVelocity.y += gravity * Time.deltaTime;

            var speed = PrototypeInput.SprintHeld ? sprintSpeed : walkSpeed;
            var horizontal = Vector3.ClampMagnitude(desiredMove, 1f) * speed;
            characterController.Move((horizontal + verticalVelocity) * Time.deltaTime);

            PrototypeDebugState.Speed = horizontal.magnitude;
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
