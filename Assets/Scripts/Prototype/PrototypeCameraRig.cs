using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeCameraRig : MonoBehaviour
    {
        [SerializeField] private PrototypePlayerController player;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Vector3 followOffset = new(0f, 1.55f, 0f);
        [SerializeField] private float distance = 5.5f;
        [SerializeField] private float vehicleDistance = 7.5f;
        [SerializeField] private float height = 1.1f;
        [SerializeField] private float lookSensitivity = 0.12f;
        [SerializeField] private float followSharpness = 12f;
        [SerializeField] private float minPitch = -18f;
        [SerializeField] private float maxPitch = 54f;
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch = 16f;

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

            var look = PrototypeInput.Look;
            yaw += look.x * lookSensitivity;
            pitch = Mathf.Clamp(pitch - look.y * lookSensitivity, minPitch, maxPitch);

            var pivot = player.CameraPivot;
            var pivotPosition = pivot.position + followOffset;
            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var desiredDistance = player.IsDriving ? vehicleDistance : distance;
            var desiredPosition = pivotPosition - rotation * Vector3.forward * desiredDistance + Vector3.up * height;
            var correctedPosition = ResolveCollision(pivotPosition, desiredPosition);

            transform.position = Vector3.Lerp(transform.position, correctedPosition, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(pivotPosition - transform.position, Vector3.up);
            targetCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        private Vector3 ResolveCollision(Vector3 pivotPosition, Vector3 desiredPosition)
        {
            var direction = desiredPosition - pivotPosition;
            var distanceToTarget = direction.magnitude;
            if (distanceToTarget <= 0.01f)
            {
                return desiredPosition;
            }

            if (Physics.SphereCast(pivotPosition, collisionRadius, direction.normalized, out var hit, distanceToTarget, collisionMask, QueryTriggerInteraction.Ignore))
            {
                return pivotPosition + direction.normalized * Mathf.Max(0.75f, hit.distance - collisionRadius);
            }

            return desiredPosition;
        }
    }
}
