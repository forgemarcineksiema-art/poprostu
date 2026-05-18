using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeCameraMode
    {
        OnFoot,
        Driving
    }

    public sealed class PrototypeCameraRig : MonoBehaviour
    {
        [SerializeField] private PrototypePlayerController player;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Vector3 followOffset = new(0f, 1.55f, 0f);
        [SerializeField] private float distance = 5.5f;
        [SerializeField] private float vehicleDistance = 7.5f;
        [SerializeField] private float height = 1.1f;
        [SerializeField] private float vehicleHeight = 1.25f;
        [SerializeField] private float lookSensitivity = 0.12f;
        [SerializeField] private float gamepadYawDegreesPerSecond = 150f;
        [SerializeField] private float gamepadPitchDegreesPerSecond = 120f;
        [SerializeField] private float followSharpness = 12f;
        [SerializeField] private float vehicleFollowSharpness = 9f;
        [SerializeField] private float recenterDelay = 1.25f;
        [SerializeField] private float vehicleRecenterDelay = 0.6f;
        [SerializeField] private float recenterSpeed = 70f;
        [SerializeField] private float vehicleRecenterSpeed = 105f;
        [SerializeField] private float minPitch = -18f;
        [SerializeField] private float maxPitch = 54f;
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch = 16f;
        private float lookIdleTime;

        public Vector3 PlanarForward => (Quaternion.Euler(0f, yaw, 0f) * Vector3.forward).normalized;
        public Vector3 PlanarRight => (Quaternion.Euler(0f, yaw, 0f) * Vector3.right).normalized;
        public float Yaw => yaw;
        public PrototypeCameraMode CurrentMode { get; private set; } = PrototypeCameraMode.OnFoot;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponentInChildren<Camera>();
            }

            if (player == null)
            {
                player = FindAnyObjectByType<PrototypePlayerController>();
            }
        }

        private void LateUpdate()
        {
            if (player == null || targetCamera == null)
            {
                return;
            }

            CurrentMode = player.IsDriving ? PrototypeCameraMode.Driving : PrototypeCameraMode.OnFoot;

            var mouseDelta = PrototypeInput.LookMouseDelta;
            var gamepadLook = PrototypeInput.LookGamepad;
            var hasLookInput = mouseDelta.sqrMagnitude > 0.001f || gamepadLook.sqrMagnitude > 0.001f;
            yaw += CalculateYawDelta(mouseDelta, gamepadLook, lookSensitivity, gamepadYawDegreesPerSecond, Time.deltaTime);
            pitch = Mathf.Clamp(
                pitch - CalculatePitchDelta(mouseDelta, gamepadLook, lookSensitivity, gamepadPitchDegreesPerSecond, Time.deltaTime),
                minPitch,
                maxPitch);

            if (hasLookInput)
            {
                lookIdleTime = 0f;
            }
            else
            {
                lookIdleTime += Time.deltaTime;
                ApplyRecenter(Time.deltaTime);
            }

            var pivot = player.CameraPivot;
            var pivotPosition = pivot.position + followOffset;
            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var desiredDistance = CurrentMode == PrototypeCameraMode.Driving ? vehicleDistance : distance;
            var desiredHeight = CurrentMode == PrototypeCameraMode.Driving ? vehicleHeight : height;
            var desiredFollowSharpness = CurrentMode == PrototypeCameraMode.Driving ? vehicleFollowSharpness : followSharpness;
            var desiredPosition = pivotPosition - rotation * Vector3.forward * desiredDistance + Vector3.up * desiredHeight;
            var correctedPosition = ResolveCollision(pivotPosition, desiredPosition);

            transform.position = Vector3.Lerp(transform.position, correctedPosition, 1f - Mathf.Exp(-desiredFollowSharpness * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(pivotPosition - transform.position, Vector3.up);
            targetCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        public void SetYawForTests(float newYaw)
        {
            yaw = newYaw;
        }

        public Vector3 ResolveCollisionForTests(Vector3 pivotPosition, Vector3 desiredPosition)
        {
            return ResolveCollision(pivotPosition, desiredPosition);
        }

        public static float CalculateYawDelta(
            Vector2 mouseDelta,
            Vector2 gamepadLook,
            float mouseSensitivity,
            float gamepadYawDegreesPerSecond,
            float deltaTime)
        {
            return mouseDelta.x * mouseSensitivity + gamepadLook.x * gamepadYawDegreesPerSecond * deltaTime;
        }

        private static float CalculatePitchDelta(
            Vector2 mouseDelta,
            Vector2 gamepadLook,
            float mouseSensitivity,
            float gamepadPitchDegreesPerSecond,
            float deltaTime)
        {
            return mouseDelta.y * mouseSensitivity + gamepadLook.y * gamepadPitchDegreesPerSecond * deltaTime;
        }

        private void ApplyRecenter(float deltaTime)
        {
            if (player == null)
            {
                return;
            }

            var delay = CurrentMode == PrototypeCameraMode.Driving ? vehicleRecenterDelay : recenterDelay;
            if (lookIdleTime < delay)
            {
                return;
            }

            var pivotForward = Vector3.ProjectOnPlane(player.CameraPivot.forward, Vector3.up);
            if (pivotForward.sqrMagnitude <= 0.001f)
            {
                return;
            }

            var targetYaw = Mathf.Atan2(pivotForward.x, pivotForward.z) * Mathf.Rad2Deg;
            var speed = CurrentMode == PrototypeCameraMode.Driving ? vehicleRecenterSpeed : recenterSpeed;
            yaw = Mathf.MoveTowardsAngle(yaw, targetYaw, speed * deltaTime);
        }

        private Vector3 ResolveCollision(Vector3 pivotPosition, Vector3 desiredPosition)
        {
            var direction = desiredPosition - pivotPosition;
            var distanceToTarget = direction.magnitude;
            if (distanceToTarget <= 0.01f)
            {
                return desiredPosition;
            }

            var closestDistance = float.MaxValue;
            var hasHit = false;
            var hits = Physics.SphereCastAll(
                pivotPosition,
                collisionRadius,
                direction.normalized,
                distanceToTarget,
                collisionMask,
                QueryTriggerInteraction.Ignore);

            foreach (var hit in hits)
            {
                if (hit.collider == null || hit.distance <= collisionRadius + 0.01f || IsIgnoredCameraHit(hit.collider))
                {
                    continue;
                }

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    hasHit = true;
                }
            }

            if (hasHit)
            {
                return pivotPosition + direction.normalized * Mathf.Max(0.75f, closestDistance - collisionRadius);
            }

            return desiredPosition;
        }

        private static bool IsIgnoredCameraHit(Collider collider)
        {
            return collider.GetComponentInParent<PrototypeRouteCheckpoint>() != null
                || collider.GetComponentInParent<PrototypeWorldReactionMarker>() != null
                || collider.GetComponentInParent<PrototypeInteractable>() != null
                || collider.GetComponentInParent<PrototypePlayerController>() != null
                || collider.GetComponentInParent<PrototypeVehicleController>() != null;
        }
    }
}
