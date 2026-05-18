using UnityEngine;

namespace ValleDePlata.Prototype
{
    public readonly struct PrototypeDriveIntent
    {
        public PrototypeDriveIntent(float throttle, float brake, float reverse)
        {
            Throttle = throttle;
            Brake = brake;
            Reverse = reverse;
        }

        public float Throttle { get; }
        public float Brake { get; }
        public float Reverse { get; }
    }

    [RequireComponent(typeof(Rigidbody))]
    public sealed class PrototypeVehicleController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private Transform fallbackExitPoint;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float reverseAcceleration = 8f;
        [SerializeField] private float brakeDeceleration = 24f;
        [SerializeField] private float maxSpeed = 17f;
        [SerializeField] private float turnRate = 92f;
        [SerializeField] private float drag = 1.2f;
        [SerializeField] private float lateralGrip = 7f;
        [SerializeField] private float handbrakeLateralGrip = 2.2f;
        [SerializeField] private float handbrakeTurnMultiplier = 1.35f;
        [SerializeField] private float reverseSpeedThreshold = 0.35f;
        [SerializeField] private float exitCheckRadius = 0.42f;
        [SerializeField] private float exitCheckHeight = 1.75f;
        [SerializeField] private LayerMask exitBlockMask;

        private Rigidbody body;
        private PrototypePlayerController driver;

        public bool HasDriver => driver != null;
        public Transform CameraPivot => cameraPivot != null ? cameraPivot : transform;

        public void Enter(PrototypePlayerController player)
        {
            driver = player;
            PrototypeDebugState.Focus = "Vehicle";
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void Update()
        {
            if (driver == null)
            {
                return;
            }

            PrototypeDebugState.Mode = "Driving";
            PrototypeDebugState.Focus = "Vehicle";
            PrototypeDebugState.Speed = body.linearVelocity.magnitude;
            PrototypeDebugState.Interaction = "E / South Button: exit car";
            PrototypeRunMetrics.Active?.RecordSpeed(body.linearVelocity.magnitude);

            if (PrototypeInput.InteractPressedThisFrame)
            {
                ExitDriver();
            }
        }

        private void FixedUpdate()
        {
            if (driver == null)
            {
                ApplyDrag(drag);
                return;
            }

            var move = PrototypeInput.Move;
            ApplyDriveInput(move, PrototypeInput.HandbrakeHeld);
        }

        public void ApplyDriveInput(Vector2 move, bool handbrake)
        {
            EnsureInitialized();

            var forwardInput = Mathf.Clamp(move.y, -1f, 1f);
            var steerInput = move.x;
            var forwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward);
            var driveIntent = ResolveDriveIntent(forwardInput, forwardSpeed, reverseSpeedThreshold);

            if (driveIntent.Throttle > 0f && body.linearVelocity.magnitude < maxSpeed)
            {
                body.AddForce(transform.forward * (driveIntent.Throttle * acceleration), ForceMode.Acceleration);
            }

            if (driveIntent.Brake > 0f && Mathf.Abs(forwardSpeed) > 0.01f)
            {
                body.AddForce(-transform.forward * (Mathf.Sign(forwardSpeed) * brakeDeceleration * driveIntent.Brake), ForceMode.Acceleration);
            }

            if (driveIntent.Reverse > 0f && body.linearVelocity.magnitude < maxSpeed * 0.45f)
            {
                body.AddForce(-transform.forward * (driveIntent.Reverse * reverseAcceleration), ForceMode.Acceleration);
            }

            var speedFactor = Mathf.InverseLerp(0.5f, maxSpeed, body.linearVelocity.magnitude);
            var steeringMultiplier = handbrake ? handbrakeTurnMultiplier : 1f;
            var steering = steerInput * turnRate * steeringMultiplier * Mathf.Clamp01(speedFactor + 0.25f) * Time.fixedDeltaTime;
            body.MoveRotation(body.rotation * Quaternion.Euler(0f, steering, 0f));

            ApplyGrip(handbrake ? handbrakeLateralGrip : lateralGrip);
            ApplyDrag(drag);
        }

        public void ExitDriver()
        {
            if (driver == null)
            {
                return;
            }

            if (!TryResolveExitPose(out var exitPosition, out var exitRotation))
            {
                PrototypeDebugState.Interaction = "Exit blocked";
                return;
            }

            var exitingDriver = driver;
            driver = null;
            exitingDriver.ExitVehicle(exitPosition, exitRotation);
        }

        public void SetExitPointsForTests(Transform primaryExit, Transform fallbackExit)
        {
            exitPoint = primaryExit;
            fallbackExitPoint = fallbackExit;
        }

        public bool TryResolveExitPose(out Vector3 position, out Quaternion rotation)
        {
            EnsureInitialized();
            rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

            if (IsExitCandidateClear(exitPoint))
            {
                position = exitPoint.position;
                return true;
            }

            if (IsExitCandidateClear(fallbackExitPoint))
            {
                position = fallbackExitPoint.position;
                return true;
            }

            if (exitPoint == null && fallbackExitPoint == null)
            {
                position = transform.position + transform.right * -1.8f + Vector3.up * 0.2f;
                return IsExitPositionClear(position);
            }

            position = Vector3.zero;
            return false;
        }

        public static PrototypeDriveIntent ResolveDriveIntent(float forwardInput, float forwardSpeed, float reverseThreshold)
        {
            var input = Mathf.Clamp(forwardInput, -1f, 1f);
            if (input > 0f)
            {
                return new PrototypeDriveIntent(input, 0f, 0f);
            }

            if (input < 0f)
            {
                var brakeOrReverse = -input;
                return forwardSpeed > reverseThreshold
                    ? new PrototypeDriveIntent(0f, brakeOrReverse, 0f)
                    : new PrototypeDriveIntent(0f, 0f, brakeOrReverse);
            }

            return new PrototypeDriveIntent(0f, 0f, 0f);
        }

        private void ApplyDrag(float dragAmount)
        {
            EnsureInitialized();
            var horizontalVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
            var dragForce = -horizontalVelocity * dragAmount;
            body.AddForce(dragForce, ForceMode.Acceleration);
        }

        private void ApplyGrip(float grip)
        {
            var lateralVelocity = Vector3.Project(body.linearVelocity, transform.right);
            body.AddForce(-lateralVelocity * grip, ForceMode.Acceleration);
        }

        private bool IsExitCandidateClear(Transform candidate)
        {
            return candidate != null && IsExitPositionClear(candidate.position);
        }

        private bool IsExitPositionClear(Vector3 candidatePosition)
        {
            Physics.SyncTransforms();

            var capsuleBottom = candidatePosition + Vector3.up * 0.45f;
            var capsuleTop = candidatePosition + Vector3.up * Mathf.Max(exitCheckHeight - exitCheckRadius, exitCheckRadius);
            var mask = exitBlockMask.value != 0 ? exitBlockMask.value : PrototypeLayers.ExitBlockMask;
            var blockers = Physics.OverlapCapsule(capsuleBottom, capsuleTop, exitCheckRadius, mask, QueryTriggerInteraction.Ignore);

            foreach (var blocker in blockers)
            {
                if (blocker == null || blocker.transform.IsChildOf(transform))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        private void EnsureInitialized()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody>();
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            if (cameraPivot == null)
            {
                cameraPivot = transform;
            }
        }
    }
}
