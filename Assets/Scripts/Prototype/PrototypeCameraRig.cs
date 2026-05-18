using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeCameraMode
    {
        OnFootFree,
        OnFootInteractionFocus,
        DrivingChase,
        TightSpaceRecovery
    }

    public readonly struct PrototypeCameraProfile
    {
        public PrototypeCameraProfile(
            float distance,
            float height,
            float followSharpness,
            float recenterDelay,
            float recenterSpeed,
            float collisionRestoreSpeed,
            float shoulderBias,
            float blendSharpness = 14f)
        {
            Distance = distance;
            Height = height;
            FollowSharpness = followSharpness;
            RecenterDelay = recenterDelay;
            RecenterSpeed = recenterSpeed;
            CollisionRestoreSpeed = collisionRestoreSpeed;
            ShoulderBias = shoulderBias;
            BlendSharpness = blendSharpness;
        }

        public float Distance { get; }
        public float Height { get; }
        public float FollowSharpness { get; }
        public float RecenterDelay { get; }
        public float RecenterSpeed { get; }
        public float CollisionRestoreSpeed { get; }
        public float ShoulderBias { get; }
        public float BlendSharpness { get; }

        public static PrototypeCameraProfile Lerp(PrototypeCameraProfile from, PrototypeCameraProfile to, float t)
        {
            var clamped = Mathf.Clamp01(t);
            return new PrototypeCameraProfile(
                Mathf.Lerp(from.Distance, to.Distance, clamped),
                Mathf.Lerp(from.Height, to.Height, clamped),
                Mathf.Lerp(from.FollowSharpness, to.FollowSharpness, clamped),
                Mathf.Lerp(from.RecenterDelay, to.RecenterDelay, clamped),
                Mathf.Lerp(from.RecenterSpeed, to.RecenterSpeed, clamped),
                Mathf.Lerp(from.CollisionRestoreSpeed, to.CollisionRestoreSpeed, clamped),
                Mathf.Lerp(from.ShoulderBias, to.ShoulderBias, clamped),
                Mathf.Lerp(from.BlendSharpness, to.BlendSharpness, clamped));
        }
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
        public PrototypeCameraMode CurrentMode { get; private set; } = PrototypeCameraMode.OnFootFree;
        public PrototypeCameraProfile CurrentProfile { get; private set; } = ResolveProfile(PrototypeCameraMode.OnFootFree);

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

            var targetMode = ResolveTargetMode();
            var targetProfile = ResolveProfile(targetMode);
            CurrentMode = targetMode;
            CurrentProfile = BlendProfile(CurrentProfile, targetProfile, Time.deltaTime);

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
                ApplyRecenter(Time.deltaTime, CurrentProfile);
            }

            var pivot = player.CameraPivot;
            var pivotPosition = pivot.position + followOffset;
            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var desiredPosition =
                pivotPosition
                - rotation * Vector3.forward * CurrentProfile.Distance
                + Vector3.up * CurrentProfile.Height
                + rotation * Vector3.right * CurrentProfile.ShoulderBias;
            var correctedPosition = ResolveCollision(pivotPosition, desiredPosition);
            var collisionOffset = Vector3.Distance(correctedPosition, desiredPosition);
            if (collisionOffset > 0.05f)
            {
                CurrentMode = PrototypeCameraMode.TightSpaceRecovery;
                CurrentProfile = BlendProfile(CurrentProfile, ResolveProfile(PrototypeCameraMode.TightSpaceRecovery), Time.deltaTime);
            }

            var followSharpness = collisionOffset > 0.05f ? CurrentProfile.CollisionRestoreSpeed : CurrentProfile.FollowSharpness;
            transform.position = Vector3.Lerp(transform.position, correctedPosition, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
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

        public static PrototypeCameraProfile ResolveProfile(PrototypeCameraMode mode)
        {
            return mode switch
            {
                PrototypeCameraMode.OnFootInteractionFocus => new PrototypeCameraProfile(4.8f, 1.15f, 14f, 1.4f, 55f, 20f, 0.35f, 12f),
                PrototypeCameraMode.DrivingChase => new PrototypeCameraProfile(7.5f, 1.25f, 9f, 0.6f, 105f, 18f, 0.18f, 10f),
                PrototypeCameraMode.TightSpaceRecovery => new PrototypeCameraProfile(4.6f, 1.05f, 16f, 0.8f, 90f, 28f, 0.12f, 18f),
                _ => new PrototypeCameraProfile(5.5f, 1.1f, 12f, 1.25f, 70f, 16f, 0f, 14f)
            };
        }

        private static PrototypeCameraProfile BlendProfile(PrototypeCameraProfile current, PrototypeCameraProfile target, float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return current;
            }

            var blend = 1f - Mathf.Exp(-target.BlendSharpness * deltaTime);
            return PrototypeCameraProfile.Lerp(current, target, blend);
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

        private PrototypeCameraMode ResolveTargetMode()
        {
            if (player.IsDriving)
            {
                return PrototypeCameraMode.DrivingChase;
            }

            return player.HasInteractionFocus
                ? PrototypeCameraMode.OnFootInteractionFocus
                : PrototypeCameraMode.OnFootFree;
        }

        private void ApplyRecenter(float deltaTime, PrototypeCameraProfile profile)
        {
            if (player == null)
            {
                return;
            }

            if (lookIdleTime < profile.RecenterDelay)
            {
                return;
            }

            var pivotForward = Vector3.ProjectOnPlane(player.CameraPivot.forward, Vector3.up);
            if (pivotForward.sqrMagnitude <= 0.001f)
            {
                return;
            }

            var targetYaw = Mathf.Atan2(pivotForward.x, pivotForward.z) * Mathf.Rad2Deg;
            yaw = Mathf.MoveTowardsAngle(yaw, targetYaw, profile.RecenterSpeed * deltaTime);
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
            var mask = collisionMask.value != 0 ? collisionMask.value : PrototypeLayers.CameraCollisionMask;
            var hits = Physics.SphereCastAll(
                pivotPosition,
                collisionRadius,
                direction.normalized,
                distanceToTarget,
                mask,
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
