using UnityEngine;

namespace ValleDePlata.Prototype
{
    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class PrototypeCharacterMotor : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider capsule;
        [SerializeField] private float height = 1.85f;
        [SerializeField] private float radius = 0.36f;
        [SerializeField] private Vector3 center = new(0f, 0.92f, 0f);
        [SerializeField] private float skinWidth = 0.04f;
        [SerializeField] private float groundProbeDistance = 0.18f;
        [SerializeField] private float slopeLimit = 50f;
        [SerializeField] private LayerMask collisionMask;

        private Vector3 verticalVelocity;

        public Vector3 HorizontalVelocity { get; private set; }
        public bool IsGrounded { get; private set; }
        public Vector3 GroundNormal { get; private set; } = Vector3.up;
        public int CollisionMask => collisionMask.value != 0 ? collisionMask.value : PrototypeLayers.WorldCollisionMask;

        private void Awake()
        {
            EnsureInitialized();
        }

        public void Move(
            Vector3 desiredDirection,
            float targetSpeed,
            float acceleration,
            float deceleration,
            float gravity,
            float turnSharpness,
            float deltaTime)
        {
            EnsureInitialized();
            UpdateGrounded();

            var planarDirection = Vector3.ProjectOnPlane(desiredDirection, Vector3.up);
            if (planarDirection.sqrMagnitude > 1f)
            {
                planarDirection.Normalize();
            }

            HorizontalVelocity = CalculateTargetHorizontalVelocity(
                planarDirection,
                HorizontalVelocity,
                targetSpeed,
                acceleration,
                deceleration,
                deltaTime);

            if (HorizontalVelocity.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(HorizontalVelocity.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSharpness * deltaTime);
            }

            if (IsGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -1f;
            }

            verticalVelocity.y += gravity * deltaTime;

            MoveWithCollision(HorizontalVelocity * deltaTime);
            if (MoveWithCollision(verticalVelocity * deltaTime) && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -1f;
            }

            ResolvePenetration();
            UpdateGrounded();
        }

        public void ResetVelocity()
        {
            HorizontalVelocity = Vector3.zero;
            verticalVelocity = Vector3.zero;
        }

        public static Vector3 CalculateTargetHorizontalVelocity(
            Vector3 desiredDirection,
            Vector3 currentVelocity,
            float targetSpeed,
            float acceleration,
            float deceleration,
            float deltaTime)
        {
            var planarDirection = Vector3.ProjectOnPlane(desiredDirection, Vector3.up);
            if (planarDirection.sqrMagnitude > 1f)
            {
                planarDirection.Normalize();
            }

            var targetVelocity = planarDirection.sqrMagnitude > 0.001f
                ? planarDirection.normalized * targetSpeed
                : Vector3.zero;
            var rate = targetVelocity.sqrMagnitude > 0.001f ? acceleration : deceleration;
            return Vector3.MoveTowards(currentVelocity, targetVelocity, rate * deltaTime);
        }

        public static bool IsSlopeWalkable(Vector3 normal, float slopeLimitDegrees)
        {
            if (normal.sqrMagnitude <= 0.001f)
            {
                return false;
            }

            return Vector3.Angle(normal.normalized, Vector3.up) <= slopeLimitDegrees;
        }

        private bool MoveWithCollision(Vector3 displacement)
        {
            var distance = displacement.magnitude;
            if (distance <= 0.0001f)
            {
                return false;
            }

            var direction = displacement / distance;
            if (TryFindClosestCastHit(direction, distance + skinWidth, out var hit))
            {
                var moveDistance = Mathf.Max(0f, hit.distance - skinWidth);
                transform.position += direction * moveDistance;

                var remaining = displacement - direction * moveDistance;
                var slide = Vector3.ProjectOnPlane(remaining, hit.normal);
                if (slide.sqrMagnitude > 0.0001f && Vector3.Dot(slide.normalized, direction) > 0f)
                {
                    transform.position += slide * 0.5f;
                }

                return true;
            }

            transform.position += displacement;
            return false;
        }

        private bool TryFindClosestCastHit(Vector3 direction, float distance, out RaycastHit closestHit)
        {
            GetCapsulePoints(out var top, out var bottom);
            var hits = Physics.CapsuleCastAll(
                top,
                bottom,
                Mathf.Max(0.01f, capsule.radius - skinWidth),
                direction,
                distance,
                CollisionMask,
                QueryTriggerInteraction.Ignore);

            closestHit = default;
            var closestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                if (hit.collider == null || IsSelfCollider(hit.collider) || hit.distance < 0f)
                {
                    continue;
                }

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestHit = hit;
                }
            }

            return closestDistance < float.MaxValue;
        }

        private void UpdateGrounded()
        {
            GetCapsulePoints(out _, out var bottom);
            var hits = Physics.SphereCastAll(
                bottom + Vector3.up * 0.05f,
                Mathf.Max(0.01f, capsule.radius - skinWidth),
                Vector3.down,
                groundProbeDistance + 0.08f,
                CollisionMask,
                QueryTriggerInteraction.Ignore);

            IsGrounded = false;
            GroundNormal = Vector3.up;
            foreach (var hit in hits)
            {
                if (hit.collider == null || IsSelfCollider(hit.collider))
                {
                    continue;
                }

                if (IsSlopeWalkable(hit.normal, slopeLimit))
                {
                    IsGrounded = true;
                    GroundNormal = hit.normal;
                    return;
                }
            }
        }

        private void ResolvePenetration()
        {
            GetCapsulePoints(out var top, out var bottom);
            var overlaps = Physics.OverlapCapsule(top, bottom, capsule.radius, CollisionMask, QueryTriggerInteraction.Ignore);
            foreach (var overlap in overlaps)
            {
                if (overlap == null || IsSelfCollider(overlap))
                {
                    continue;
                }

                if (Physics.ComputePenetration(
                    capsule,
                    transform.position,
                    transform.rotation,
                    overlap,
                    overlap.transform.position,
                    overlap.transform.rotation,
                    out var direction,
                    out var distance))
                {
                    transform.position += direction * (distance + skinWidth);
                }
            }
        }

        private void GetCapsulePoints(out Vector3 top, out Vector3 bottom)
        {
            var worldCenter = transform.TransformPoint(capsule.center);
            var halfSegment = Mathf.Max(0f, capsule.height * 0.5f - capsule.radius);
            top = worldCenter + transform.up * halfSegment;
            bottom = worldCenter - transform.up * halfSegment;
        }

        private bool IsSelfCollider(Collider candidate)
        {
            return candidate != null && candidate.transform.IsChildOf(transform);
        }

        private void EnsureInitialized()
        {
            if (capsule == null)
            {
                capsule = GetComponent<CapsuleCollider>();
            }

            capsule.height = height;
            capsule.radius = radius;
            capsule.center = center;
            capsule.isTrigger = false;
        }
    }
}
